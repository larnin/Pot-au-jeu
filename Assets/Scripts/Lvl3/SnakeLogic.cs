using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NRand;

public class SnakeLogic : StartableLogic
{
    [SerializeField] float m_pacmanRadius = 5;
    [SerializeField] float m_speed = 3;
    [SerializeField] List<int> m_walkableIDs;
    [SerializeField] float m_updateTime = 1.0f;
    [SerializeField] float m_randomRange = 5;
    [SerializeField] float m_timeBeforeSendNext = 0.7f;
    [SerializeField] int m_initialLenght = 5;
    [SerializeField] GameObject m_nodePrefab;

    PacmanLogic m_pacman;
    Tween m_tween;
    Animator m_animator;
    List<Position> m_path = new List<Position>();
    List<SnakeNodeLogic> m_nodes = new List<SnakeNodeLogic>();
    bool m_bossDie = false;
    float m_time = 0;

    class Position
    {
        public Position(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public int x;
        public int y;
    }

    class Node
    {
        public Node(Node _parent, int _x, int _y)
        {
            parent = _parent;
            x = _x;
            y = _y;
        }
        public Node parent;
        public int x;
        public int y;
    }

    protected override void onAwake()
    {
        m_pacman = FindObjectOfType<PacmanLogic>();
        m_animator = GetComponent<Animator>();
        for (int i = 0; i < m_initialLenght; i++)
            spawnNode();
    }

    public bool haveNode(int x, int y)
    {
        foreach(var n in m_nodes)
        {
            var i = Mathf.RoundToInt(n.transform.position.x);
            var j = Mathf.RoundToInt(n.transform.position.y);
            if (x == i || y == j)
                return true;
        }
        return false;
    }

    protected override void onUpdate()
    {
        m_time -= Time.deltaTime;
        if(m_time <= 0)
        {
            m_time = m_updateTime;

            if ((transform.position - m_pacman.transform.position).magnitude < m_pacmanRadius)
                setTarget(Mathf.RoundToInt(m_pacman.transform.position.x), Mathf.RoundToInt(m_pacman.transform.position.y));
            else if (SnakePowerupPopulatorLogic.powerup != null)
                setTarget(Mathf.RoundToInt(SnakePowerupPopulatorLogic.powerup.transform.position.x), Mathf.RoundToInt(SnakePowerupPopulatorLogic.powerup.transform.position.y));
            else
            {
                var target = new UniformVector2SquareDistribution(m_randomRange).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
                setTarget(Mathf.RoundToInt(transform.position.x + target.x), Mathf.RoundToInt(transform.position.y + target.y));
            }
        }
    }

    public void onNodeKilled(SnakeNodeLogic node)
    {
        m_nodes.Remove(node);
    }

    void spawnNode()
    {
        GameObject obj = Instantiate(m_nodePrefab);
        var node = obj.GetComponent<SnakeNodeLogic>();
        if(m_nodes.Count > 0)
        {
            var lastNode = m_nodes[m_nodes.Count - 1];
            obj.transform.position = lastNode.transform.position;
            lastNode.nextNode = node;
            node.previousNode = lastNode;
        }
        else obj.transform.position = transform.position;

        m_nodes.Add(node);
    }

    void setTarget(int x, int y, bool ignoreNodes = false)
    {
        m_path.Clear();

        int px = Mathf.RoundToInt(transform.position.x);
        int py = Mathf.RoundToInt(transform.position.y);

        List<Node> nextPos = new List<Node>();
        List<Node> visited = new List<Node>();

        nextPos.Add(new Node(null, px, py));

        bool foundEnd = false;

        while (nextPos.Count > 0)
        {
            var pos = nextPos[0];
            nextPos.RemoveAt(0);

            visited.Add(pos);

            if (pos.x == x && pos.y == y)
            {
                foundEnd = true;
                break;
            }

            List<Node> nextNodes = new List<Node>();
            nextNodes.Add(new Node(pos, pos.x + 1, pos.y));
            nextNodes.Add(new Node(pos, pos.x - 1, pos.y));
            nextNodes.Add(new Node(pos, pos.x, pos.y + 1));
            nextNodes.Add(new Node(pos, pos.x, pos.y - 1));

            foreach (var n in nextNodes)
            {
                var id = TilemapInfos.instance.tileID(n.x, n.y);
                if (!m_walkableIDs.Contains(id))
                    continue;
                if (visited.Find(v => { return v.x == n.x && v.y == n.y; }) != null)
                    continue;
                if (nextPos.Find(v => { return v.x == n.x && v.y == n.y; }) != null)
                    continue;
                if (!ignoreNodes && haveNode(n.x, n.y))
                    continue;
                nextPos.Add(n);
            }
        }

        int currentPos = visited.Count() - 1;
        float bestDist = float.MaxValue;
        if (!foundEnd)
        {
            for (int i = 0; i < visited.Count; i++)
            {
                var d = (new Vector2(visited[i].x, visited[i].y) - new Vector2(x, y)).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    currentPos = i;
                }
            }
        }

        var current = visited[currentPos];

        m_path.Add(new Position(current.x, current.y));

        for (int i = currentPos - 1; i >= 0; i--)
        {
            if (current.parent == visited[i])
            {
                current = visited[i];
                m_path.Insert(0, new Position(current.x, current.y));
            }
            if (current.parent == null)
                break;
        }

        if (m_path.Count() > 0 && m_path[0].x == px && m_path[0].y == py)
            m_path.RemoveAt(0);

        if(m_path.Count() == 0 && !ignoreNodes)
        {
            setTarget(x, y, true);
            return;
        }

        if (m_tween == null || m_tween.IsComplete() || !m_tween.IsPlaying() || !m_tween.IsActive())
            startNextMove();
    }

    void startNextMove()
    {
        if (m_bossDie)
            return;

        if (m_tween != null)
        {
            m_tween.Kill();
            m_tween = null;
        }

        if (m_path.Count == 0)
            return;

        var pos = m_path[0];
        m_path.RemoveAt(0);

        var dir = new Vector2(pos.x, pos.y) - new Vector2(transform.position.x, transform.position.y);
        // Up,      0
        // Down,    1
        // Left,    2
        // Right    3
        const string propertyName = "Direction";
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                m_animator.SetInteger(propertyName, 3);
            else m_animator.SetInteger(propertyName, 2);
        }
        else if (dir.y > 0)
            m_animator.SetInteger(propertyName, 1);
        else m_animator.SetInteger(propertyName, 0);

        float d = dir.magnitude;

        m_tween = transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), 1 / m_speed * d).SetEase(Ease.Linear).OnComplete(startNextMove);
        DOVirtual.DelayedCall(1 / m_speed * d * m_timeBeforeSendNext, () =>
        {
            if(m_nodes.Count() > 0)
            {
                m_nodes[0].setTarget(new Vector3(pos.x, pos.y, transform.position.z), m_speed, 1 / m_speed * d * m_timeBeforeSendNext);
            }
        });
    }
}
