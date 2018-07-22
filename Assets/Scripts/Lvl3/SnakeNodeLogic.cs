using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SnakeNodeLogic : MonoBehaviour
{
    const string endProperty = "isEnd";

    SnakeLogic m_head;
    public SnakeLogic head { get { return m_head; } set { m_head = value; } }

    bool m_lastNode = false;

    Animator m_animator;

    public bool lastNode
    {
        get
        {
            return m_lastNode;
        }
        set
        {
            m_lastNode = value;
            m_animator.SetBool(endProperty, m_lastNode);
        }
    }

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (head != null)
            head.onNodeKilled(this);
    }
}
