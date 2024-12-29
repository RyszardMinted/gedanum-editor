using UnityEngine;

public class ResolutionAdjuster : MonoBehaviour
{                 
    private int oldWidth;
    private int oldHeight;
    
    void Start()
    {
        AdjustResolution();
    }

    void Update()
    {
        AdjustResolution();
    }

    private void AdjustResolution()
    {
        var width = Screen.width;
        var height = Screen.height;

        if (oldHeight != height && oldWidth != width)
        {
            Screen.SetResolution(width, height, false);
            oldHeight = height;
            oldWidth = width;
            Debug.Log($"My dude 2 Adjusting resolution to {width}x{height}");
        }
    }
}
