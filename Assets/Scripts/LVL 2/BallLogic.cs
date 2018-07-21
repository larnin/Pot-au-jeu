using UnityEngine;
using System.Collections;
using DG.Tweening;
using NRand;
using System.Collections.Generic;

public class BallLogic : MonoBehaviour
{
    [SerializeField] int m_id;
    [SerializeField] Color m_color;
    [SerializeField] float m_effectTime;
    [SerializeField] float m_speed;
    
    public int id { get { return m_id; } }
    public float effectTime { get { return m_effectTime; } }
    public Color color { get { return m_color; } }

    RectInt m_path;

    public void setPath(RectInt path)
    {
        m_path = path;
        StartCoroutine(run());
    }

    IEnumerator run()
    {
        List<Vector2Int> points = new List<Vector2Int> {new Vector2Int(m_path.min.x, m_path.max.y), m_path.min, new Vector2Int(m_path.max.x, m_path.min.y), m_path.max};
        bool direction = new BernoulliDistribution().Next(new StaticRandomGenerator<DefaultRandomGenerator>());
        if(direction)
        {
            var temp = points[0];
            points[0] = points[3];
            points[3] = temp;
            temp = points[2];
            points[2] = points[1];
            points[1] = temp;
        }

        int currentIndex = 0;

        while (true)
        {
            var d = (new Vector2(transform.position.x, transform.position.y) - points[currentIndex]).magnitude;

            transform.DOMove(new Vector3(points[currentIndex].x, points[currentIndex].y, transform.position.z), d / m_speed).SetEase(Ease.Linear);

            currentIndex = (currentIndex + 1) % points.Count;
            yield return new WaitForSeconds(d / m_speed);
        }
    }
}
