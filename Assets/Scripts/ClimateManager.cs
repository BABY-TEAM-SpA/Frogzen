using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
    public Sprite ruedaSprite;
    public string temperatureText;
    public Color temperatureTextColor =Color.white;
    public Sprite casaSprite;
    [Range(.5f,3f)] public float removingMultuplyDelay;
    [Range(1f,3f)] public float frozenMultiply;
    [Range(1f,3f)] public float warmingMultiply=1f;
    public AudioClip sfx;
    public int vfxIndex;
    public int angle;
}

public class ClimateManager : MonoBehaviour
{
    public static ClimateManager Instance { get; private set; }
    [SerializeField] private Climate[] climates;
    [SerializeField] private List<int> mustBreakWindowSeason =new List<int>();
    [SerializeField] private List<int> mustGiveLife =new List<int>();
    [SerializeField] public int currentWeatherIndex=-1;
    [SerializeField] private int currentWeatherCounter=-1;
    public Climate currentWeather = null;
    private float currentWeatherTimer=0;

    public List<GameObject> vfx;

    [SerializeField] private Image casa;
    [SerializeField] private Image rueda;
    [SerializeField] private Image flecha;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text temperatureText;
    
    

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
        timeText.text = GameManager.Instance.GetGameTime();
        if (currentWeatherTimer <= currentWeather.duration)
        {
            currentWeatherTimer += Time.deltaTime;
        }
        else
        {
            ChangeWeather();
        }
    }


    
    public void ChangeWeather(bool isInitial=false)
    {
        if (!isInitial)
        {
            GameManager.Instance.StopSFX(currentWeatherIndex);
        }
        
        UpdateWeatherIndex();
        
        currentWeather = climates[currentWeatherIndex];
        currentWeatherTimer = 0;
        GameManager.Instance.PlayMusic();
        GameManager.Instance.PlaySFX(currentWeather.sfx,currentWeatherIndex,true);
        ChangeVFX();
        if(mustBreakWindowSeason.Contains(currentWeatherCounter))
        {
            GameManager.Instance.DestroySomeWindow(-1);
        }
        if(mustGiveLife.Contains(currentWeatherCounter))
        {
            GameManager.Instance.AddNewLife();
        }
        UpdateUI();
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

    private void UpdateUI()
    {
        temperatureText.text = currentWeather.temperatureText;
        temperatureText.color = currentWeather.temperatureTextColor;
        rueda.sprite = currentWeather.ruedaSprite;
        casa.sprite = currentWeather.casaSprite;
        LeanTween.rotateZ(flecha.gameObject, currentWeather.angle, currentWeather.duration);

    }
}
