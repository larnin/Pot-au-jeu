using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using NRand;

public class SnakePowerupPopulatorLogic : StartableLogic
{
    [SerializeField] RectInt m_region;
    [SerializeField] GameObject m_powerupPrefab;
    [SerializeField] float m_delayBeforePowerup = 1;
    [SerializeField] List<int> m_populatableZone;

    static SnakePowerupLogic m_currentPowerup;
    static SnakePowerupPopulatorLogic m_instance;

    SnakeLogic m_snake;

    bool m_populateRetry = false;

    public static SnakePowerupLogic powerup
    {
        get { return m_currentPowerup; }
        set
        {
            m_currentPowerup = value;
            m_instance.onPowerupSet();
        }
    }

    protected override void onAwake()
    {
        m_instance = this;
        m_snake = FindObjectOfType<SnakeLogic>();
    }

    protected override void onStart()
    {
        populate();
    }

    void onPowerupSet()
    {
        if(m_currentPowerup == null)
        {
            DOVirtual.DelayedCall(m_delayBeforePowerup, populate);
        }
    }

    private void Update()
    {
        if (m_populateRetry)
            populate();
    }

    void populate()
    {
        m_populateRetry = true;

        var dx = new UniformIntDistribution(m_region.min.x, m_region.max.x);
        var dy = new UniformIntDistribution(m_region.min.y, m_region.max.y);
        var gen = new StaticRandomGenerator<DefaultRandomGenerator>();
        
        var x = dx.Next(gen);
        var y = dy.Next(gen);

        if (m_snake != null && m_snake.haveNode(x, y))
            return;

        if (!m_populatableZone.Contains(TilemapInfos.instance.tileID(x, y)))
            return;

        var obj = Instantiate(m_powerupPrefab, new Vector3(x, y, -0.5f), Quaternion.identity);
        m_currentPowerup = obj.GetComponent<SnakePowerupLogic>();

        m_populateRetry = false;
    }
}
