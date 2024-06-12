using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private ClimateManager climateManager;
    [SerializeField] public Frog[] frogsData;
    
    [Serializable]
    internal class WindowsFrames
    {
        public WindowsType windowsType;
        public WindowsDropArea[] windowsFrame = new WindowsDropArea[8];
        
    }
    [SerializeField] public Transform upperObject;

    [SerializeField] private WindowsFrames[] windowsFramesData = new WindowsFrames[3];
    [SerializeField] private WindowsMovementController[] windowsPrefabs = new WindowsMovementController[3];

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
        
        if(TryGetComponent(out ClimateManager manage))
        {
            climateManager = manage;
        }

    }
    void Start()
    {
        SetUpAllWindows();
        DestroySomeWindow(0);
        DestroySomeWindow(1);
        DestroySomeWindow(2);
    }

    void SetUpAllWindows()
    {
        foreach (WindowsFrames frame in windowsFramesData)
        {
            WindowsType frametype = frame.windowsType;
            WindowsMovementController prefab = windowsPrefabs.First(x => x.windowsType == frametype);
            foreach (WindowsDropArea frameObject in frame.windowsFrame)
            {
                int randomFrogIndex = Random.Range(0, frogsData.Length);
                if(frameObject.TryGetComponent(out FrogController frog)){frog.PrepareFrog(frogsData[randomFrogIndex], frameObject);}
                frameObject.SpawnWindow(prefab);
            }
        }
    }

    void DestroySomeWindow(int frameDataindex = -1)
    {
        if(frameDataindex ==-1) frameDataindex = Random.Range(0, windowsFramesData.Length);
        WindowsFrames winFrameData = windowsFramesData[frameDataindex];
        int frameindex = Random.Range(0, winFrameData.windowsFrame.Length);
        winFrameData.windowsFrame[frameindex].DestroyWindow();
    }
    
    
}
