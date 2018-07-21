using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NRand;

public class OrangeGhostLogic : StartableLogic
{
    [SerializeField] float m_updateTime = 3;
    [SerializeField] int m_radius = 10;

    GhostLogic m_ghost;
    PacmanLogic m_pacman;
    float m_time;

    protected override void onAwake()
    {
        m_ghost = GetComponent<GhostLogic>();
        m_pacman = FindObjectOfType<PacmanLogic>();
        m_time = 0;
    }

    protected override void onUpdate()
    {
        if (m_ghost.haveFinishedPath())
            updateTarget();

        m_time -= Time.deltaTime;
        if (m_time < 0)
            updateTarget();
    }

    void updateTarget()
    {
        m_time = m_updateTime;

        int x = Mathf.RoundToInt(m_pacman.transform.position.x);
        int y = Mathf.RoundToInt(m_pacman.transform.position.y);

        var gen = new StaticRandomGenerator<DefaultRandomGenerator>();
        if(new BernoulliDistribution().Next(gen))
        {
            var dir = new UniformVector2SquareDistribution(m_radius).Next(gen);

            x = Mathf.RoundToInt(transform.position.x + dir.x);
            y = Mathf.RoundToInt(transform.position.y + dir.y);
        }

        m_ghost.setTarget(x, y);
    }
}