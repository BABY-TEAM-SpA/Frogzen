using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private bool adminMode;
    [SerializeField] private bool doesUseLifes;
    [SerializeField] private List<Image> lifesImages = new List<Image>();
    [SerializeField] private int lifes;
    [SerializeField] private Image lifePrefab;
    [SerializeField] private Transform lifeContainer; 
    
    
    [SerializeField] public Frog[] frogsData;
    [SerializeField] private float gameTime;
    [SerializeField] private int frogsDead;
    [SerializeField] private int windowsBroken;
 
    private readonly Queue<int> windowsTypesToDestroyNext = new();

    [Serializable]                                        
    internal class AudioPlayers                           
    {                                                     
        public AudioSource audioSource;                    
        public bool isActive;                              
                                                      
    }                                                     
    [Serializable]
    internal class WindowsFrames
    {
        public WindowsType windowsType;
        public WindowsDropArea[] windowsFrame = new WindowsDropArea[8];
        
    }
    [SerializeField] public Transform upperObject;

    [SerializeField] private WindowsFrames[] windowsFramesData = new WindowsFrames[3];
    [SerializeField] private WindowsMovementController[] windowsPrefabs = new WindowsMovementController[3];

    [Header("Menus Configuration")]
    public bool isPaused;
    public GameObject pauseMenu;
    public GameObject endMenu;
    public TMP_Text windowsEndText;
    public TMP_Text timeEndText;

    [Header("Audio configurations")] 
    
    private Coroutine musicHandle;
    private Coroutine sfxHandle;
    [SerializeField] private float fadeMusicTime;
    private float currentMusicVolume = 1f;  
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioPlayers[] musicPlayers;
    private float currentSFXVolume = 1f;  
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioPlayers[] sfxPlayer;
    [SerializeField] private int currentSfxPlayer=0;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
        }

    }
    void Start()
    {
        if(GameSettings.Instance !=null) lifes = GameSettings.Instance.GetLifes();
        if (lifes > 0) doesUseLifes = true;
        ClimateManager.Instance.Prepare();
        StartGameplay();
    }

    void StartGameplay()
    {
        SetUpAllWindows();
        DestroySomeWindow(0,0,true);
        DestroySomeWindow(1,0,true);
        DestroySomeWindow(2,0,true);
        if (doesUseLifes)
        {
            for (int i = 0; i < lifes; i++)
            {
                AddNewLife(false);
            }
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Pause(); 
            }
            else
            {
                UnPause();
            }
            
        }
    }

    void SetUpAllWindows()
    {
        foreach (WindowsFrames frame in windowsFramesData)
        {
            WindowsType frametype = frame.windowsType;
            WindowsMovementController prefab = windowsPrefabs.First(x => x.windowsType == frametype);
            foreach (WindowsDropArea frameObject in frame.windowsFrame)
            {
                int randomFrogIndex = Random.Range(0, frogsData.Length);
                if(frameObject.TryGetComponent(out FrogController frog)){frog.PrepareFrog(frogsData[randomFrogIndex], frameObject);}
                frameObject.SpawnWindow(prefab);
            }
        }
    }

    public void DestroySomeWindow(int frameDataindex = -1, int hasRepeat = 0, bool mute = false)
    {
        if (frameDataindex == -1)
        {
            frameDataindex = PopWindowTypeToDestroy();
            Debug.Log(frameDataindex);
        }
        WindowsFrames winFrameData = windowsFramesData[frameDataindex];
        int frameindex = Random.Range(0, winFrameData.windowsFrame.Length);
        if (winFrameData.windowsFrame[frameindex].DestroyWindow(mute))
        {
            windowsBroken += 1;
        }
        else
        {
            hasRepeat += 1;
            Debug.Log("repeat on trying destroying");
            DestroySomeWindow(frameDataindex,hasRepeat);
        }
    }
    

    public void UpdateMusicVolume()
    {
        currentMusicVolume = musicVolumeSlider.value;   
        foreach (AudioPlayers player in musicPlayers)
        {
            if (player.isActive)
            {
                player.audioSource.volume = musicVolumeSlider.value;   
            }
        }
        Debug.Log("volume: "+ musicVolumeSlider.value.ToString());
    }
    
    public void UpdateSFXVolume()                        
    {                                                    
        foreach (AudioPlayers player in sfxPlayer)        
        {                                                
            player.audioSource.volume = sfxVolumeSlider.value;       
            currentSFXVolume =  sfxVolumeSlider.value;   
        }                                                
    }                                                    

    private int PopWindowTypeToDestroy()
    {
        if (windowsTypesToDestroyNext.Count == 0)
        {
            int nFrames = windowsFramesData.Length;

            // indices from 0 to nFrames-1 with random order
            int[] windowsTypes = Enumerable
                .Range(0, nFrames)
                .OrderBy(_ => Random.value)
                .ToArray();
            foreach (int type in windowsTypes)
            {
                windowsTypesToDestroyNext.Enqueue(type);
            }

        }

        return windowsTypesToDestroyNext.Dequeue();
    }
    
    public void PlayMusic()
    {
        for (int i = 1; i < musicPlayers.Length; i++)
        {
            musicPlayers[i].audioSource.time = musicPlayers[0].audioSource.time;
            if (i <= ClimateManager.Instance.currentWeatherIndex)
            {
                if (!musicPlayers[i].isActive)
                {
                    musicHandle = StartCoroutine(ChangeVolume(musicPlayers[i].audioSource,fadeMusicTime, 0f,currentMusicVolume));
                    musicPlayers[i].isActive = true;
                }
            }
            else
            {
                if (musicPlayers[i].isActive)                                                                                     
                {                                                                                                                  
                    musicHandle = StartCoroutine(ChangeVolume(musicPlayers[i].audioSource,fadeMusicTime/2,currentMusicVolume, 0f));
                    musicPlayers[i].isActive = false;
                }                                                                                                                   
            }
        }
    }

    public void PlaySFX(AudioClip clip,int index=4, bool mustFade=false)
    {
        if (mustFade)
        {
            sfxPlayer[index].audioSource.volume = 0;                                                                      
            sfxPlayer[index].audioSource.clip = clip;                                                                     
            sfxPlayer[index].audioSource.Play();                                                                          
            sfxHandle = StartCoroutine(ChangeVolume(sfxPlayer[index].audioSource, fadeMusicTime, 0f,currentSFXVolume));  
        }
        else
        {
            sfxPlayer[index].audioSource.volume = currentSFXVolume;                                                                      
            sfxPlayer[index].audioSource.clip = clip;                                                                     
            sfxPlayer[index].audioSource.Play();
        }
    }
    
    public void StopSFX(int index)
    {
        
        sfxHandle = StartCoroutine(ChangeVolume(sfxPlayer[index].audioSource,fadeMusicTime/2,currentSFXVolume, 0f, () =>
        {
            sfxPlayer[index]?.audioSource.Stop();
        }));
    }
    
    private IEnumerator ChangeVolume( AudioSource AToPlay,float time, float start, float end, Action onComplete=null)                 
    {                                                                                                                                 
        float elapsedTime = 0f;                                                                                                       
        while (elapsedTime < time)                                                                                                    
        {                                                                                                                             
            elapsedTime += Time.deltaTime;                                                                                            
            float progress = elapsedTime / time;                                                                                      
            AToPlay.volume = Mathf.Lerp(start, end, progress);                                                                        
            yield return null;                                                                                                        
        }                                                                                                                             
                                                                                                                                      
        //AToPlay.volume = end;                                                                                                         
        if (onComplete != null)                                                                                                       
        {                                                                                                                             
            onComplete();                                                                                                       
        }                                                                                                                             
    }
    public void Pause()                                                                      
    {                                                                                        
        isPaused = true;                                                                     
        Time.timeScale = 0f; //no es lo que quiero hacer, pero por mientras.                 
        musicVolumeSlider.value=currentMusicVolume;                                          
        sfxVolumeSlider.value=currentSFXVolume;                                              
        sfxPlayer[currentSfxPlayer].audioSource.Pause();                                                 
        pauseMenu.SetActive(true);                                                           
    }                                                                                        
                                                                                          
    public void UnPause()                                                                    
    {                                                                                        
        isPaused = false;                                                                    
        Time.timeScale = 1f; //no es lo que quiero hacer, pero por mientras.                 
        sfxPlayer[currentSfxPlayer].audioSource.Play();                                                  
        pauseMenu.SetActive(false);                                                          
    }                                                                                        
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public string GetGameTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);

        // Formatea el resultado en mm:ss
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void CheckTotalFrogs()
    {
        frogsDead += 1;
        if (!adminMode)
        {
            
            if (doesUseLifes)
            {
                int result = lifes - frogsDead;
                if (result>0)
                {
                    lifesImages[frogsDead-1].color= Color.red;
                    
                }else if(result==0)
                {
                    lifesImages[frogsDead-1].color= Color.red;
                    OnEndingGame();
                }
            }
            else
            {
                if (frogsDead == windowsBroken)
                {
                    OnEndingGame();
                }
            }
        }
    }

    public void AddNewLife(bool mustAddlifeCounter=true)
    {
        if (doesUseLifes)
        {
            if(mustAddlifeCounter) lifes += 1;
            Image instanceSprite= Instantiate(lifePrefab, lifeContainer);
            lifesImages.Add(instanceSprite);
        }
    }
    
    public void OnEndingGame()
    {
        Time.timeScale = 0f;
        endMenu.SetActive(true);
        windowsEndText.text = windowsBroken.ToString();
        timeEndText.text = GetGameTime();
        foreach (AudioPlayers player in musicPlayers)
        {
            player.audioSource.Stop();
        }
        foreach (AudioPlayers player in sfxPlayer)
        {
            player.audioSource.Stop();
        }
    }
}
