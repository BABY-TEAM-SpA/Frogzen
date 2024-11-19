using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isMobilePlatform)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    

}
