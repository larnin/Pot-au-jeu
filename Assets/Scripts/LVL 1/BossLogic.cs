using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BossLogic : StartableLogic
{
    const string monsterDieTrigger = "MonsterDie";
    const string dieTrigger = "Die";

    [SerializeField] List<GhostStateLogic> m_ghosts;

    SubscriberList m_subscriberList = new SubscriberList();
    Animator m_animator;

    protected override void onAwake()
    {
        m_subscriberList.Add(new Event<MonsterEatenEvent>.Subscriber(onMonsterDie));
        m_subscriberList.Subscribe();

        m_animator = GetComponent<Animator>();
    }

    protected override void onDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    protected override void onUpdate()
    {
        bool allDie = true;

        foreach (var g in m_ghosts)
            if (!g.isEaten())
                allDie = false;
        if (allDie)
            onAllMonstersDead();
    }

    void onMonsterDie(MonsterEatenEvent e)
    {
        m_animator.SetTrigger(monsterDieTrigger);
    }

    void onAllMonstersDead()
    {
        m_animator.SetTrigger(dieTrigger);
        Event<BossDieEvent>.Broadcast(new BossDieEvent());
    }
}