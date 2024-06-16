using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum FrogType
{
    Default,
    Kid,
    Young,
    Old
}

[Serializable]
public class Frog
{
    public FrogType type;
    public Sprite[] frogSprites = new Sprite[3];
    [Range(1f,2f)] public float coldnessFrogMultiplier =1f;
} 

public class FrogController : MonoBehaviour
{

    [SerializeField] private Image frogInstance;
    [SerializeField] private CanvasGroup frozenBarObject;
    [SerializeField] private Image frozenBarFill;
    [SerializeField] private float initialTemperature = 40f;
    [SerializeField] private Color fillColor;
    [SerializeField] private Color warmColor;
    [SerializeField] private Color coldColor;
    [SerializeField] private Color frozenColor;
    private float temperature;
    private bool isFrozen;
    [SerializeField] private Frog currentFrog;
    private WindowsDropArea windowFrame;
    [SerializeField] private AudioClip freezeSound;
    

    public void Start()
    {
        temperature = initialTemperature;
    }

    public void PrepareFrog(Frog frogData, WindowsDropArea frame)
    {
        windowFrame = frame;
        currentFrog = frogData;
        frogInstance.sprite = currentFrog.frogSprites[0];
        frogInstance.SetNativeSize();
    }
    
    void Update()
    {
        if (!isFrozen)
        {
            if (windowFrame.hasWindow)
            {
                
                if (temperature >= initialTemperature)
                {
                    frozenBarObject.gameObject.SetActive(false);
                }
                else
                {
                    temperature += Time.deltaTime;
                    UpdateSprite(0, fillColor);
                }
            }
            else
            {
                temperature -= Time.deltaTime * currentFrog.coldnessFrogMultiplier * ClimateManager.Instance.currentWeather.frozenMultiply;
                frozenBarObject.gameObject.SetActive(true);
                if (temperature > 2*(initialTemperature / 3))
                {
                    UpdateSprite(0,warmColor);
                }
                else
                {
                    if (temperature > initialTemperature / 2)
                    {
                        UpdateSprite(1,coldColor);
                    }
                    else
                    {
                        if (temperature > 0)
                        {
                            UpdateSprite(2,frozenColor);
                        }
                        else
                        {
                            Freeze();
                        }
                    }
                } 
            }
            frozenBarFill.fillAmount = temperature / initialTemperature;  
        }

    }

    public void UpdateSprite(int index, Color color)
    {
        frogInstance.sprite = currentFrog.frogSprites[index];
        frogInstance.SetNativeSize();
        frozenBarFill.color = color;
    }
    public void Freeze()
    {
        frogInstance.color = Color.cyan;//currentFrog.frogSprites[3];
        frogInstance.SetNativeSize();
        frozenBarObject.gameObject.SetActive(false);
        isFrozen = true;
        GameManager.Instance.PlaySFX(freezeSound,6);
        GameManager.Instance.CheckTotalFrogs();
    }
}
