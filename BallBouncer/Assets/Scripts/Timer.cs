using UnityEngine;
using System.Collections;

public class Timer
{

    public float startTime;
    public float currentTime;


    public Timer(float _startTime)
    {
        startTime = _startTime;
        currentTime = _startTime;
    }


    public void Tick(float _deltaTime)
    {
        currentTime -= _deltaTime;
    }

    public bool IsFinished()
    {
        return currentTime <= 0;
    }

    public void Reset()
    {
        currentTime = startTime;
    }

    public void SetStartTime(float _newStart)
    {
        startTime = _newStart;
    }
    public void SetCurrentTime(float _newCurrent)
    {
        currentTime = _newCurrent;
    }
    public float GetTimePassed()
    {
        return startTime - currentTime;
    }

}
