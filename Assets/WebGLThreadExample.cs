using System;
using System.Threading;
using UnityEngine;


public class WebGLThreadExample : MonoBehaviour
{
    void Start()
    {
        StartBackgroundTask();
    }

    void StartBackgroundTask()
    {
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            Debug.Log("Background task started");
            int result = HeavyCalculation();
            Debug.Log($"Calculation result - {result}");
        });

        thread.Start();
    }

    int HeavyCalculation()
    {
        int sum = 0;
        for (int i = 0; i < 1000000; i++)
        {
            sum += i;
        }
        return sum;
    }
}