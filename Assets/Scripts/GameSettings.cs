using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DificulyClimates
{
    public Climate[] climates;
    public int lifes;
}

[Serializable]
public enum Dificulty
{
    Easy =0,
    Medium =1,
    Hard=2,
}
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [SerializeField] private DificulyClimates[] difficultSetting;
    private Dificulty currentDifficulty;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public Climate[] GetClimates()
    {
        return difficultSetting[(int)currentDifficulty].climates;
    }

    public int GetLifes()
    {
        return difficultSetting[(int)currentDifficulty].lifes;
    }

    public void SetDificult(int value)
    {
        currentDifficulty = (Dificulty)value;
    }
}
