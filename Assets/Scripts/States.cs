using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class States
{
    static States m_instance;

    public static States instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new States();
            return m_instance;
        }
    }

    public float time { get; set; }
    public int deaths { get; set; }
    public int plays { get; set; }

    Dictionary<string, bool> m_finishedScenes = new Dictionary<string, bool>();
    public void setFiniedScene(string name)
    {
        if(!isFinishedScene(name))
            m_finishedScenes.Add(name, true);
    }

    public bool isFinishedScene(string name)
    {
        return m_finishedScenes.ContainsKey(name);
    }

    public int finishedSceneCount()
    {
        return m_finishedScenes.Count();
    }

    public void reset()
    {
        time = 0;
        deaths = 0;
        plays = 0;
        m_finishedScenes.Clear();
    }
}
