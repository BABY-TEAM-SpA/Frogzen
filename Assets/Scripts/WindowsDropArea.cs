using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum WindowsType
{
    Circular,
    Curve,
    Rectangular
}
public class WindowsDropArea : MonoBehaviour, IDropHandler
{
    
    [SerializeField] private WindowsType windowFrameType;
    public bool hasWindow;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !hasWindow)
        {
            WindowsMovementController window = eventData.pointerDrag.GetComponent<WindowsMovementController>();
            if (window.windowsType == windowFrameType && window.hasDrag)
            {
                window.PlaceWindow(this);
            }
            
        }
    }

    public void SpawnWindow(WindowsMovementController prefab)
    {
        WindowsMovementController window = Instantiate<WindowsMovementController>(prefab,this.transform);
        window.upperObject = GameManager.Instance.upperObject;
        hasWindow = true;
    }

    public bool DestroyWindow()
    {
        if (hasWindow)
        {
            WindowsMovementController window = transform.GetComponentInChildren<WindowsMovementController>();
            window.DestroyWindow();
            hasWindow = false;
            return true;
        }
        return false;

    }

}
