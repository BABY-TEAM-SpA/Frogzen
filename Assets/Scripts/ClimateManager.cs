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
    [Range(.5f,10f)] public float removingMultuplyDelay;
    [Range(1f,10f)] public float frozenMultiply;

}

public class ClimateManager : MonoBehaviour
{
    public static ClimateManager Instance { get; private set; }
    [SerializeField] private Climate[] climates;
    private int currentWeatherIndex;
    public Climate currentWeather;

    public GameObject wind;
    public GameObject rain;
    public GameObject snow;

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
        
    }


    
    private void ChangeWeather()
    {
        
    }
}
