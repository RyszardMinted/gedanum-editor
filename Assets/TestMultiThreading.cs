using System;
using System.Threading;
using UnityEngine;

public class TestMultiThreading : MonoBehaviour
{
    private Thread thread;
    bool goingLeft;
    private bool isStopped;
    private DateTime time;
    private AutoResetEvent resetEvent;
    private Vector3 lastPosition;
    
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
            
         thread = new Thread(Run);
         isStopped = false;
        thread.Start();  
        resetEvent = new AutoResetEvent(false);

    }
    
    private void OnDestroy()  
    {  
        thread.Abort();  
        isStopped = true;  
    }

    void Run()  
    {  
        while (!isStopped)  
        {  
            time = DateTime.Now;  
            MoveObject();  
        }  
    }
    
    void MoveObject()  
    {  
        resetEvent.WaitOne();  
        DateTime now = DateTime.Now;  
        TimeSpan deltaTime = now - time;  
        time = now;  
        lastPosition += Vector3.right * (goingLeft ? -1f : 1f)   
            * (float)deltaTime.TotalSeconds;if ((goingLeft && lastPosition.x < -3) || (!goingLeft && lastPosition.x > 3))  
            goingLeft = !goingLeft;  
    }
    
    private void Update()  
    {  
        resetEvent.Set();  
        transform.localPosition = lastPosition;  
    }
}
