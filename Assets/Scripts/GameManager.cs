using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;




public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Frog[] frogsData;
    
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

    [Header("Audio configurations")] 
    
    private Coroutine musicHandle;
    private Coroutine sfxHandle;
    [SerializeField] private float fadeMusicTime;
    private float currentMusicVolume = 1f;  
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioPlayers[] musicPlayers;
    private float currentSFXVolume = 1f;  
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioSource[] sfxPlayer;
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
        SetUpAllWindows();
        DestroySomeWindow(0);
        DestroySomeWindow(1);
        DestroySomeWindow(2);
        ClimateManager.Instance.ChangeWeather(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Pause();
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

    public void DestroySomeWindow(int frameDataindex = -1)
    {
        if(frameDataindex ==-1) frameDataindex = Random.Range(0, windowsFramesData.Length);
        WindowsFrames winFrameData = windowsFramesData[frameDataindex];
        int frameindex = Random.Range(0, winFrameData.windowsFrame.Length);
        winFrameData.windowsFrame[frameindex].DestroyWindow();
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
        foreach (AudioSource player in sfxPlayer)        
        {                                                
            player.volume = sfxVolumeSlider.value;       
            currentSFXVolume =  sfxVolumeSlider.value;   
        }                                                
    }                                                    

    
    public void PlayMusic(bool mustFade =true)
    {
        for (int i = 0; i < musicPlayers.Length; i++)
        {
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

    public void PlaySFX(AudioClip clip=null)
    {
        if (currentSfxPlayer == 0)
        {   
            currentSfxPlayer = 1;
        }
        else
        {   
            currentSfxPlayer = 0;                                                             
        }
        sfxPlayer[currentSfxPlayer].volume = 0;                                                                      
        sfxPlayer[currentSfxPlayer].clip = clip;                                                                     
        sfxPlayer[currentSfxPlayer].Play();                                                                          
        sfxHandle = StartCoroutine(ChangeVolume(sfxPlayer[currentSfxPlayer], fadeMusicTime, 0f,currentSFXVolume));   
    }
    
    public void StopSFX(bool mustFade =true)
    {
        int actualindex = currentSfxPlayer;
        sfxHandle = StartCoroutine(ChangeVolume(sfxPlayer[actualindex],fadeMusicTime/2,currentSFXVolume, 0f, () => sfxPlayer[actualindex].Stop() ));
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
        sfxPlayer[currentSfxPlayer].Pause();                                                 
        pauseMenu.SetActive(true);                                                           
    }                                                                                        
                                                                                          
    public void UnPause()                                                                    
    {                                                                                        
        isPaused = false;                                                                    
        Time.timeScale = 1f; //no es lo que quiero hacer, pero por mientras.                 
        sfxPlayer[currentSfxPlayer].Play();                                                  
        pauseMenu.SetActive(false);                                                          
    }                                                                                        
    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
