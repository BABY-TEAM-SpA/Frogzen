using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        windowFrame = this.GetComponent<WindowsDropArea>();
    }

    public void Start()
    {
        temperature = initialTemperature;
    }

    public void PrepareFrog()
    {
        
        // currentFrog = seleccionar una rana random
        frogInstance.sprite = currentFrog.frogSprites[0];
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
                }
            }
            else
            {
                temperature -= Time.deltaTime * currentFrog.coldnessFrogMultiplier * ClimateManager.Instance.currentWeather.frozenMultiply;
                frozenBarObject.gameObject.SetActive(true);
                if (temperature > 2*(initialTemperature / 3))
                {
                    frogInstance.sprite = currentFrog.frogSprites[0];
                    frozenBarFill.color = warmColor;
                }
                else
                {
                    if (temperature > initialTemperature / 2)
                    {
                        frogInstance.sprite = currentFrog.frogSprites[1];
                        frozenBarFill.color = coldColor;
                    }
                    else
                    {
                        if (temperature != 0)
                        {
                            frogInstance.sprite = currentFrog.frogSprites[2];
                            frozenBarFill.color = frozenColor;
                        }
                        else
                        {
                            //Frozen.... frogInstance.sprite = currentFrog.frogSprites[3];
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
