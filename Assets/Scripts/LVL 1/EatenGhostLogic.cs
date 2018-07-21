using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EatenGhostLogic : StartableLogic
{
    [SerializeField] int m_targetX;
    [SerializeField] int m_targetY;

    GhostLogic m_ghost;

    private void Awake()
    {
        m_ghost = GetComponent<GhostLogic>();
    }

    protected override void onEnable()
    {
        m_ghost.setTarget(m_targetX, m_targetY);
    }
}
