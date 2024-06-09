using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowsMovementController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public WindowsType windowsType;
    [SerializeField] public WindowsDropArea originalParent;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    [Header("Animation Variables")]
    [SerializeField] private LeanTweenType animCurve;
    [SerializeField] [Range(0f,5f)]private float animUpTime;
    [SerializeField] [Range(1f,2f)] private float scaleSizeBig;
    [SerializeField] [Range(0f,5f)]private float animDownTime;
    [SerializeField] private Transform upperObject;
    private Vector2 animSize;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animSize = rectTransform.sizeDelta;
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
    }

    private void Start()
    {
        PlaceWindow(originalParent);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; 
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(upperObject);
        LeanTween.pause(this.gameObject);
        LeanTween.size(rectTransform, animSize*scaleSizeBig, animUpTime).setEase(animCurve);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        PlaceWindow(originalParent);
        LeanTween.pause(this.gameObject);
        LeanTween.size(rectTransform, animSize, animDownTime).setEase(animCurve);
    }
    public void PlaceWindow(WindowsDropArea parent)
    {
        originalParent.hasWindow = false;
        transform.SetParent(parent.transform);
        originalParent = parent;
        rectTransform.anchoredPosition = Vector2.zero;
        originalParent.hasWindow = true;
    }


}