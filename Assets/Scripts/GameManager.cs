using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Frog[] frogsData;
    
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
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioSource[] musicPlayer;
    [SerializeField] private int currentMusicPlayer=0;
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

    void DestroySomeWindow(int frameDataindex = -1)
    {
        if(frameDataindex ==-1) frameDataindex = Random.Range(0, windowsFramesData.Length);
        WindowsFrames winFrameData = windowsFramesData[frameDataindex];
        int frameindex = Random.Range(0, winFrameData.windowsFrame.Length);
        winFrameData.windowsFrame[frameindex].DestroyWindow();
    }
    
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; //no es lo que quiero hacer, pero por mientras.
        musicVolumeSlider.value=musicPlayer[0].volume;
        sfxVolumeSlider.value=sfxPlayer[0].volume;
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

    public void UpdateMusicVolume()
    {
        foreach (AudioSource player in musicPlayer)
        {
            player.volume = musicVolumeSlider.value;
        }
        Debug.Log("volume: "+ musicVolumeSlider.value.ToString());
    }

    public void PlayMusic(AudioClip clip, bool mustFade =true)
    {
        if (currentMusicPlayer == 0)
        {
            musicPlayer[1].clip = clip;
            if (mustFade)
            {
                musicPlayer[1].volume = 0f;
                musicPlayer[1].Play();
                musicPlayer[1].time = musicPlayer[0].time;
                musicPlayer[1].loop = true;
                musicHandle = StartCoroutine(PlayingAudio(musicPlayer[1],fadeMusicTime, musicPlayer[0].volume));
            }
            else
            {
                musicPlayer[1].Play();
            }
            currentMusicPlayer = 1;
        }
        else
        {
            musicPlayer[0].clip = clip;
            if (mustFade)
            {
                musicPlayer[0].volume = 0f;
                musicPlayer[0].Play();
                musicPlayer[0].time = musicPlayer[1].time;
                musicPlayer[0].loop = true;
                musicHandle = StartCoroutine(PlayingAudio( musicPlayer[0],fadeMusicTime, musicPlayer[1].volume));
            }
            else
            {
                musicPlayer[0].Play();
            }
            currentMusicPlayer = 0;
        }
    }

    public void StopMusic(bool mustFade =true)
    {
        if (mustFade)
        {
            musicHandle = StartCoroutine(StopingAudio(musicPlayer[currentMusicPlayer],fadeMusicTime/2, musicPlayer[currentMusicPlayer].volume));
        }
        else
        {
            musicPlayer[currentMusicPlayer].Stop();
        }
    }
    public void PlaySFX(AudioClip clip=null)
    {
        if (currentSfxPlayer == 0)
        {
            sfxPlayer[1].volume = 0;
            sfxPlayer[1].clip = clip;
            sfxHandle = StartCoroutine(PlayingAudio( sfxPlayer[1],fadeMusicTime, sfxPlayer[0].volume));
            currentSfxPlayer = 1;
        }
        else
        {
            sfxPlayer[0].volume = 0;
            sfxPlayer[0].clip = clip;
            sfxHandle = StartCoroutine(PlayingAudio(sfxPlayer[0], fadeMusicTime, sfxPlayer[1].volume));
            currentSfxPlayer = 0;
        }
    }
    
    public void StopSFX(bool mustFade =true)
    {
        if (mustFade)
        {
            sfxHandle = StartCoroutine(StopingAudio(sfxPlayer[currentSfxPlayer],fadeMusicTime/2, sfxPlayer[currentSfxPlayer].volume));
        }
        else
        {
            sfxPlayer[currentSfxPlayer].Stop();
        }
    }
    public void UpdateSFXVolume()
    {
        foreach (AudioSource player in sfxPlayer)
        {
            player.volume = sfxVolumeSlider.value;
        }
    }

    public void Exit()
    {
        Debug.Log("Has exited");
    }

    private IEnumerator PlayingAudio( AudioSource AToPlay,float time, float currentVolume)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / time;
            AToPlay.volume = Mathf.Lerp(0f, currentVolume, progress);
            yield return null;
        }
        AToPlay.volume = currentVolume;
        
    }
    private IEnumerator StopingAudio(AudioSource APlaying,float time, float currentVolume)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / time;
            APlaying.volume = Mathf.Lerp(currentVolume, 0f, progress);
            yield return null;
        }
        APlaying.Stop();
        APlaying.volume = currentVolume;
        
    }
}
