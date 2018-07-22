using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PacSnakeLogic : MonoBehaviour
{
    const string dieTrigger = "Die";

    [SerializeField] float m_powerupDuration = 5;
    [SerializeField] float m_powerupSpeed = 8;

    float m_baseSpeed;
    PacmanLogic m_pacman;
    Animator m_animator;
    bool m_dead = false;

    private void Awake()
    {
        m_pacman = GetComponent<PacmanLogic>();
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_baseSpeed = m_pacman.speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionPowerup(collision.gameObject.GetComponent<SnakePowerupLogic>());
        collisionSnakeHead(collision.gameObject.GetComponent<SnakeLogic>());
        collisonSnakeNode(collision.gameObject.GetComponent<SnakeNodeLogic>());
    }

    void collisionPowerup(SnakePowerupLogic p)
    {
        if (p == null || m_dead)
            return;

        m_pacman.speed = m_powerupSpeed;
        Destroy(p.gameObject);
        DOVirtual.DelayedCall(m_powerupDuration, () => m_pacman.speed = m_baseSpeed);
    }

    void collisonSnakeNode(SnakeNodeLogic s)
    {
        if (s == null || m_dead)
            return;

        if (s.lastNode)
            Destroy(s.gameObject);
        else onDie();
    }

    void collisionSnakeHead(SnakeLogic s)
    {
        if (s == null || m_dead)
            return;

        if (s.isLastNode())
            s.kill();
        else onDie();
    }

    void onDie()
    {
        if (m_dead)
            return;
        m_animator.SetTrigger(dieTrigger);
        Event<DieEvent>.Broadcast(new DieEvent());
        m_dead = true;
    }
}
