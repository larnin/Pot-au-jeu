using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShowScoreWindowEvent : EventArgs
{
    public ShowScoreWindowEvent(string _title, string _text, TextAlignment _alignment, float _width)
    {
        title = _title;
        text = _text;
        alignment = _alignment;
        width = _width;
    }

    public string text;
    public string title;
    public TextAlignment alignment;
    public float width;
}