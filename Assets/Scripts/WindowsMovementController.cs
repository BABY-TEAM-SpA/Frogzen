using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowsMovementController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] public WindowsType windowsType;
    [SerializeField] public WindowsDropArea originalParent;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private bool canDrag;
    private bool hasDrag;
    [SerializeField] private float holdTime = 3f;
    [SerializeField] private GameObject loadingParent;
    [SerializeField] private Image loadingBar;
    private Coroutine holdCoroutine;
        
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

    public void OnPointerDown(PointerEventData eventData)
    {
        holdCoroutine = StartCoroutine(HoldRoutine());
        hasDrag = false;
        canDrag = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!canDrag)
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                
                loadingParent.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canDrag)
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                loadingParent.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!hasDrag)
            {
                PlaceWindow(originalParent);
                AnimateDown();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            hasDrag = true;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            //canvasGroup.alpha = 1.0f;
            PlaceWindow(originalParent);
            AnimateDown();
            canDrag = false;
        }
        
    }
    public void PlaceWindow(WindowsDropArea parent)
    {
        originalParent.hasWindow = false;
        transform.SetParent(parent.transform);
        originalParent = parent;
        rectTransform.anchoredPosition = Vector2.zero;
        originalParent.hasWindow = true;
    }

    private IEnumerator HoldRoutine()
    {
        loadingBar.fillAmount = 0f;
        loadingParent.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < holdTime)
        {
            elapsedTime += Time.deltaTime;
            loadingBar.fillAmount = elapsedTime / holdTime;
            yield return null;
        }
        transform.SetParent(upperObject);
        canDrag = true;
        loadingParent.SetActive(false);
        AnimateUP();
    }

    private void AnimateUP()
    {
        canvasGroup.blocksRaycasts = false;
        LeanTween.pause(this.gameObject);
        LeanTween.size(rectTransform, animSize*scaleSizeBig, animUpTime).setEase(animCurve);
    }

    private void AnimateDown()
    {
        canvasGroup.blocksRaycasts = true;
        LeanTween.pause(this.gameObject);
        LeanTween.size(rectTransform, animSize, animDownTime).setEase(animCurve);
    }
    
}