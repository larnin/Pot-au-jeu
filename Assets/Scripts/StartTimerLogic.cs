using UnityEngine;
using System.Collections;

public class StartTimerLogic : MonoBehaviour
{
    [SerializeField] float m_timeBeforeTimer = 1;
    [SerializeField] float m_timer = 3;

    float m_currentTime;

    void Start()
    {
        m_currentTime = -m_timeBeforeTimer;
    }
    
    void Update()
    {
        m_currentTime += Time.deltaTime;
        m_currentTime = Mathf.Min(m_currentTime, m_timer);
        if (m_currentTime > 0)
            Event<StartTimerUpdatedEvent>.Broadcast(new StartTimerUpdatedEvent(m_currentTime, m_timer));
        if (m_currentTime >= m_timer)
        {
            Event<StartEvent>.Broadcast(new StartEvent());
            enabled = false;
        }
    }
}
