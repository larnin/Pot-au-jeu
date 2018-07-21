using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StartTimerUpdatedEvent : EventArgs
{
    public StartTimerUpdatedEvent(float _time, float _totalTime)
    {
        time = _time;
        totalTime = _totalTime;
    }

    public float time;
    public float totalTime;
}
