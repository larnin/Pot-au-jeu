using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TakePowerupEvent : EventArgs
{
    public TakePowerupEvent(float _time)
    {
        time = _time;
    }

    public float time;
}
