using UnityEngine;
using System.Collections;
using DG.Tweening;
using NRand;

public class MechaBossLogic : StartableLogic
{
    const string dieTrigger = "Die";

    [SerializeField] float m_speed = 2;
    [SerializeField] float m_rangeX = 4;
    [SerializeField] GameObject m_explosionPrefab;
    [SerializeField] float m_explosionTime = 3;
    [SerializeField] float m_explosionRadius = 1;
    [SerializeField] float m_explosionDelta = 0.2f;
    [SerializeField] int m_explosionFinalCount = 15;
    [SerializeField] AudioClip m_bossExplosionClip;

    Vector2 m_originalPos;
    Animator m_animator;

    protected override void onAwake()
    {
        m_originalPos = new Vector2(transform.position.x, transform.position.y);
        startXMove();
        m_animator = GetComponent<Animator>();
    }
    
    void startXMove()
    {
        float targetX = m_originalPos.x + new UniformFloatDistribution(-m_rangeX, m_rangeX).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
        float current = transform.position.x;

        transform.DOMoveX(targetX, (Mathf.Abs(targetX - current) / m_speed)).SetEase(Ease.InOutSine).OnComplete(startXMove);
    }

    public void kill()
    {
        m_animator.SetTrigger(dieTrigger);

        Event<BossDieEvent>.Broadcast(new BossDieEvent());

        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();
        var d = new UniformFloatDistribution(-m_explosionRadius, m_explosionRadius);

        for (float i = 0; i <= m_explosionTime; i+= m_explosionDelta)
        {
            DOVirtual.DelayedCall(i, () =>
            {
                var pos = transform.position + new Vector3(d.Next(rand), d.Next(rand), -0.5f);
                Instantiate(m_explosionPrefab, pos, Quaternion.identity);
            });
        }

        DOVirtual.DelayedCall(m_explosionTime, () =>
        {
            DOVirtual.DelayedCall(0.2f, () => gameObject.SetActive(false));

            Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_bossExplosionClip));
            
            for (int i = 0; i < m_explosionFinalCount; i++)
            {
                var pos = transform.position + new Vector3(d.Next(rand), d.Next(rand), -0.5f);
                Instantiate(m_explosionPrefab, pos, Quaternion.identity);
            }
        });
        
    }
}
