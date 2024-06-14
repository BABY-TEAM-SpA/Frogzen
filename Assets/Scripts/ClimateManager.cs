using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeatherState{
    
    Cloudy,
    Windy,
    Rainy,
    Snowy,
    Default
}

[Serializable]
public class Climate
{
    public WeatherState weather;
    public float duration;
    public Color removingColorBar =Color.white;
    [Range(.5f,3f)] public float removingMultuplyDelay;
    [Range(1f,5f)] public float frozenMultiply;
    public AudioClip sfx;
    public int vfxIndex;
}

public class ClimateManager : MonoBehaviour
{
    public static ClimateManager Instance { get; private set; }
    [SerializeField] private Climate[] climates;
    [SerializeField] private List<int> mustBreakWindowSeason =new List<int>();
    [SerializeField] public int currentWeatherIndex=-1;
    [SerializeField] private int currentWeatherCounter=-1;
    public Climate currentWeather = null;
    private float currentWeatherTimer=0;

    public List<GameObject> vfx;
    

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

    private void Update()
    {
        if (currentWeatherTimer <= currentWeather.duration)
        {
            currentWeatherTimer += Time.deltaTime;
        }
        else
        {
            ChangeWeather();
        }
    }


    
    public void ChangeWeather(bool mustFade =true)
    {
        UpdateWeatherIndex();
        currentWeather = climates[currentWeatherIndex];
        currentWeatherTimer = 0;
        GameManager.Instance.PlayMusic(mustFade);
        GameManager.Instance.StopSFX();
        GameManager.Instance.PlaySFX(currentWeather.sfx);
        ChangeVFX();
    }

    private void UpdateWeatherIndex()
    {
        currentWeatherIndex += 1;
        currentWeatherCounter += 1;
        if (currentWeatherIndex >= climates.Length)
        {
            currentWeatherIndex = 0;
        }
    }

    private void ChangeVFX()
    {
        foreach (GameObject fx in vfx)
        {
            fx?.SetActive(false);
        }
        
        vfx[currentWeather.vfxIndex]?.SetActive(true);
    }
}
