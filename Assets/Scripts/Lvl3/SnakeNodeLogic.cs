using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SnakeNodeLogic : MonoBehaviour
{
    [SerializeField] float m_dheight = 0.01f;

    SnakeNodeLogic m_nextNode;
    public SnakeNodeLogic nextNode { get { return m_nextNode; } set { m_nextNode = value; } }
    SnakeNodeLogic m_previousNode;
    public SnakeNodeLogic previousNode { get { return m_previousNode; } set { m_previousNode = value; } }
    SnakeLogic m_head;
    public SnakeLogic head { get { return m_head; } set { m_head = value; } }

    bool isLastNode()
    {
        return m_nextNode == null;
    }

    private void OnDestroy()
    {
        if (m_previousNode != null)
            m_previousNode.nextNode = null;

        if (m_nextNode != null)
            m_nextNode.previousNode = null;

        if(head != null)
            head.onNodeKilled(this);
    }

    public void setTarget(Vector3 target, float speed, float timeBeforeSendNext)
    {
        target.z += m_dheight;
        float d = (transform.position - target).magnitude;

        transform.DOMove(target, d / speed).SetEase(Ease.Linear);
        DOVirtual.DelayedCall(timeBeforeSendNext, () =>
        {
            if (m_nextNode != null)
                m_nextNode.setTarget(target, speed, timeBeforeSendNext);
        });
    }
}
