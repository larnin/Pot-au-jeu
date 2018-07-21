using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BlueGhostLogic : BaseColoredGhostLogic
{
    [SerializeField] float m_updateTime = 1;
    [SerializeField] int m_backDistance = 5;

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

        var dir = m_pacman.getDirection() * m_backDistance;
        x -= dir.x;
        y -= dir.y;

        m_ghost.setTarget(x, y);
    }
}