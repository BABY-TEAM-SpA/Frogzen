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
                frozenBarFill.color = fillColor;
                if (temperature >= initialTemperature)
                {
                    frozenBarObject.gameObject.SetActive(false);
                }
                else
                {
                    temperature += Time.deltaTime;
                    frogInstance.sprite = currentFrog.frogSprites[0];
                }
            }
            else
            {
                temperature -= Time.deltaTime * currentFrog.coldnessFrogMultiplier * ClimateManager.Instance.currentWeather.frozenMultiply;
                frozenBarObject.gameObject.SetActive(true);
                if (temperature > 2*(initialTemperature / 3))
                {
                    frogInstance.sprite = currentFrog.frogSprites[0];
                    frogInstance.SetNativeSize();
                    frozenBarFill.color = warmColor;
                }
                else
                {
                    if (temperature > initialTemperature / 2)
                    {
                        frogInstance.sprite = currentFrog.frogSprites[1];
                        frogInstance.SetNativeSize();
                        frozenBarFill.color = coldColor;
                    }
                    else
                    {
                        if (temperature > 0)
                        {
                            frogInstance.sprite = currentFrog.frogSprites[2];
                            frogInstance.SetNativeSize();
                            frozenBarFill.color = frozenColor;
                        }
                        else
                        {
                            Debug.Log(this.gameObject.name+" has been Frozen");
                            frogInstance.color = Color.cyan;//currentFrog.frogSprites[3];
                            frogInstance.SetNativeSize();
                            frozenBarObject.gameObject.SetActive(false);
                            isFrozen = true;
                        }
                    }
                } 
            }
            frozenBarFill.fillAmount = temperature / initialTemperature;  
        }

    }
}
