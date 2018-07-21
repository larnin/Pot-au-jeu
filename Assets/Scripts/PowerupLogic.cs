using UnityEngine;
using System.Collections;

public class PowerupLogic : MonoBehaviour
{
    [SerializeField] float m_popTime;
    [SerializeField] float m_showTime;

    float m_timer;
    SpriteRenderer m_render;

    void Start()
    {
        m_timer = m_popTime;
        m_render = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        m_timer += Time.deltaTime;

        float alpha = 1;
        if (m_timer > m_popTime && m_timer < m_popTime + m_showTime)
            alpha = (m_timer - m_popTime) / m_showTime;
        else if (m_timer < m_popTime)
            alpha = 0;
        var c = m_render.color;
        c.a = alpha;
        m_render.color = c;
    }

    public void pickup()
    {
        m_timer = 0;
    }

    public bool isPickable()
    {
        return m_timer >= m_popTime;
    }
}
