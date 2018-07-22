using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VulnerableGhostLogic : StartableLogic
{
    GhostLogic m_ghost;
    PacmanLogic m_pacman;
    float m_fleeDIstance = 10;

    protected override void onAwake()
    {
        m_ghost = GetComponent<GhostLogic>();
        m_pacman = FindObjectOfType<PacmanLogic>();
    }

    protected override void onEnable()
    {
        updatePath();
    }

    protected override void onUpdate()
    {
        if (m_ghost.haveFinishedPath())
            updatePath();
    }

    void updatePath()
    {
        int x = Mathf.RoundToInt(m_pacman.transform.position.x);
        int y = Mathf.RoundToInt(m_pacman.transform.position.y);

        var dir = (new Vector2(transform.position.x, transform.position.y) - new Vector2(x, y)) * m_fleeDIstance;

        int newX = Mathf.RoundToInt(transform.position.x + dir.x);
        int newY = Mathf.RoundToInt(transform.position.y + dir.y);

        m_ghost.setTarget(newX, newY);

        if (!m_ghost.haveFinishedPath())
            return;

        newX = Mathf.RoundToInt(transform.position.x - dir.y);
        newY = Mathf.RoundToInt(transform.position.y + dir.x);

        m_ghost.setTarget(newX, newY);

        if (!m_ghost.haveFinishedPath())
            return;

        newX = Mathf.RoundToInt(transform.position.x + dir.y);
        newY = Mathf.RoundToInt(transform.position.y - dir.x);

        m_ghost.setTarget(newX, newY);
    }

}
