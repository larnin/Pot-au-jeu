using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SnakeNodeLogic : MonoBehaviour
{
    SnakeLogic m_head;
    public SnakeLogic head { get { return m_head; } set { m_head = value; } }
    
    bool lastNode { get; set; }

    private void OnDestroy()
    {
        if (head != null)
            head.onNodeKilled(this);
    }
}
