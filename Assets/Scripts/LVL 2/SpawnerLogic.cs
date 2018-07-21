using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NRand;

public class SpawnerLogic : StartableLogic
{
    [SerializeField] GameObject m_redBall;
    [SerializeField] GameObject m_pinkBall;
    [SerializeField] GameObject m_blueBall;
    [SerializeField] GameObject m_greenBall;
    [SerializeField] int m_range = 5;
    [SerializeField] float m_redProbability = 0.5f;
    [SerializeField] float m_spawnTime = 2;
    [SerializeField] float m_spawnDelay = 0.5f;
    [SerializeField] float m_spawnLimit = 10;
    [SerializeField] float m_spawnHeight = 8;
    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] List<RectInt> m_possiblePath; 

    float m_timer = 0;
    bool m_spawning = false;

    bool m_direction = true;
    float m_centerPos;

    int m_totalSpawnCount = 1;

    GameObject m_spawnedPink = null;
    GameObject m_spawnedBlue = null;
    GameObject m_spawnedGreen = null;

    protected override void onAwake()
    {
        m_centerPos = transform.position.x;   
    }

    protected override void onUpdate()
    {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0 && !m_spawning)
            spawn();

        if (!m_spawning)
            move();
    }

    void spawn()
    {
        GameObject objectToSpawn = m_redBall;
        if (m_spawnedPink == null)
            objectToSpawn = m_pinkBall;
        if (m_spawnedGreen == null)
            objectToSpawn = m_greenBall;
        if (m_spawnedBlue == null)
            objectToSpawn = m_blueBall;

        float redProbability = m_totalSpawnCount > m_spawnLimit ? m_redProbability : m_redProbability * ((float)m_totalSpawnCount / m_spawnLimit);
        if (m_spawnedPink != null && m_spawnedGreen != null && m_spawnedBlue != null)
            redProbability = m_redProbability;

        bool toRed = new BernoulliDistribution(redProbability).Next(new StaticRandomGenerator<DefaultRandomGenerator>());

        if (objectToSpawn == m_redBall && !toRed)
        {
            m_timer = m_spawnTime;
            return;
        }
        if (objectToSpawn != m_redBall && toRed)
            objectToSpawn = m_redBall;

        m_spawning = true;
        DOVirtual.DelayedCall(m_spawnDelay, () =>
        {
            var obj = Instantiate(objectToSpawn, new Vector3(transform.position.x, m_spawnHeight, -0.5f), Quaternion.identity);
            var comp = obj.GetComponent<BallLogic>();
            if (comp != null)
                comp.setPath(m_possiblePath[new UniformIntDistribution(m_possiblePath.Count - 1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())]);

            if (objectToSpawn == m_pinkBall)
                m_spawnedPink = obj;
            if (objectToSpawn == m_greenBall)
                m_spawnedGreen = obj;
            if (objectToSpawn == m_blueBall)
                m_spawnedBlue = obj;

            m_timer = m_spawnTime;
            m_totalSpawnCount++;

            DOVirtual.DelayedCall(m_spawnDelay, () => m_spawning = false);
        });
    }

    void move()
    {
        var pos = transform.position.x - m_centerPos;
        if (pos < -m_range)
            m_direction = true;
        if (pos > m_range)
            m_direction = false;

        float move = Time.deltaTime * m_moveSpeed * (m_direction ? 1 : -1);
        transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z);
    }
}
