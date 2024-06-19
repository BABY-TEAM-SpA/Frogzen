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
    private bool isShaking = false;
    [SerializeField] private float shakeIntensity = 10f;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private Frog currentFrog;
    private WindowsDropArea windowFrame;
    [SerializeField] private AudioClip freezeSound;
    
    // Referenza al RectTransform dell'immagine
    private RectTransform rectTransform;

    // Posizione iniziale
    private Vector3 initialPosition;
    

    public void Start()
    {
        temperature = initialTemperature;
    }

    public void PrepareFrog(Frog frogData, WindowsDropArea frame)
    {
        windowFrame = frame;
        currentFrog = frogData;
        frogInstance.sprite = currentFrog.frogSprites[4];
        frogInstance.SetNativeSize();
        rectTransform = frogInstance.GetComponent<RectTransform>();
        initialPosition = rectTransform.localPosition;
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
                    temperature += Time.deltaTime* ClimateManager.Instance.currentWeather.warmingMultiply;
                    UpdateSprite(4, fillColor);
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
                            Shake();
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
        if(index!=2)StopShaking();
        frogInstance.sprite = currentFrog.frogSprites[index];
        frogInstance.SetNativeSize();
        frozenBarFill.color = color;
    }

    private void Shake()
    {
        if (isShaking)
        {
            return;
        }
        isShaking = true;
        Vector3 newPos = initialPosition + new Vector3( shakeIntensity,0, 0);
        LeanTween.moveLocal(frogInstance.gameObject, newPos, shakeDuration)
            .setEase(LeanTweenType.easeShake)
            .setLoopClamp()
            .setOnComplete(() =>
            {
                rectTransform.localPosition = initialPosition;
                isShaking = false;
                Shake();
            });
    }
    
    public void StopShaking()
    {
        isShaking = false;
        LeanTween.cancel(frogInstance.gameObject);
        rectTransform.localPosition = initialPosition;
    }
    public void Freeze()
    {
        StopShaking();
        UpdateSprite(3,Color.cyan);
        frogInstance.SetNativeSize();
        frozenBarObject.gameObject.SetActive(false);
        isFrozen = true;
        GameManager.Instance.PlaySFX(freezeSound,6);
        GameManager.Instance.CheckTotalFrogs();
    }
}
