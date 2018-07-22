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
    [SerializeField] float m_randomRange = 5;
    [SerializeField] int m_nodeDeltaPos = 3;
    [SerializeField] int m_initialLenght = 5;
    [SerializeField] GameObject m_nodePrefab;
    [SerializeField] int m_sizeIncreasePowerUp = 2;
    [SerializeField] float m_dheight = 0.01f;

    PacmanLogic m_pacman;
    Animator m_animator;
    List<Position> m_path = new List<Position>();
    List<SnakeNodeLogic> m_nodes = new List<SnakeNodeLogic>();
    bool m_bossDie = false;

    List<Vector3> m_positions = new List<Vector3>();

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
        for (int i = 0; i < m_nodeDeltaPos; i++)
            m_positions.Add(transform.position);
        m_pacman = FindObjectOfType<PacmanLogic>();
        m_animator = GetComponent<Animator>();
        for (int i = 0; i < m_initialLenght; i++)
            spawnNode();
    }

    protected override void onLateStart()
    {
        startNextMove();
    }

    public bool haveNode(int x, int y)
    {
        foreach(var n in m_nodes)
        {
            var i = Mathf.RoundToInt(n.transform.position.x);
            var j = Mathf.RoundToInt(n.transform.position.y);
            
            if (x == i && y == j)
                return true;
        }
        return false;
    }

    protected override void onFixedUpdate()
    {
        for (int i = m_positions.Count - 1; i > 0; i--)
            m_positions[i] = m_positions[i - 1];
        m_positions[0] = transform.position;

        for(int i = 0; i < m_nodes.Count; i++)
            m_nodes[i].transform.position = m_positions[(i + 1) * m_nodeDeltaPos] + new Vector3(0, 0, m_dheight * i);
    }

    public void onNodeKilled(SnakeNodeLogic node)
    {
        m_nodes.Remove(node);

        for (int i = 0; i < m_nodeDeltaPos; i++)
            m_positions.RemoveAt(m_positions.Count - 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<SnakePowerupLogic>() != null)
        {
            Destroy(collision.gameObject);

            for (int i = 0; i < m_sizeIncreasePowerUp; i++)
                spawnNode();
        }
    }

    void spawnNode()
    {
        GameObject obj = Instantiate(m_nodePrefab);
        var node = obj.GetComponent<SnakeNodeLogic>();
        if(m_nodes.Count > 0)
        {
            var lastNode = m_nodes[m_nodes.Count - 1];
            obj.transform.position = lastNode.transform.position;
        }
        else obj.transform.position = transform.position;
        node.head = this;

        m_nodes.Add(node);

        for (int i = 0; i < m_nodeDeltaPos; i++)
            m_positions.Add(m_positions[m_positions.Count - 1]);
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
    }

    void startNextMove()
    {
        if (m_bossDie)
            return;

        if (m_path.Count == 0)
        {
            execIA();
            if(m_path.Count == 0)
            {
                DOVirtual.DelayedCall(0.1f, startNextMove);
                return;
            }
        }

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
        if (d < 0.1f)
        {
            startNextMove();
            return;
        }

        transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), 1 / m_speed * d).SetEase(Ease.Linear).OnComplete(() =>
        {
            startNextMove();
        });
        
    }

    void execIA()
    {
        if ((new Vector2(transform.position.x, transform.position.y) - new Vector2(m_pacman.transform.position.x, m_pacman.transform.position.y)).magnitude<m_pacmanRadius)
            setTarget(Mathf.RoundToInt(m_pacman.transform.position.x), Mathf.RoundToInt(m_pacman.transform.position.y));
        else if (SnakePowerupPopulatorLogic.powerup != null)
            setTarget(Mathf.RoundToInt(SnakePowerupPopulatorLogic.powerup.transform.position.x), Mathf.RoundToInt(SnakePowerupPopulatorLogic.powerup.transform.position.y));
        else
        {
            var target = new UniformVector2CircleSurfaceDistribution(m_randomRange).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
            setTarget(Mathf.RoundToInt(transform.position.x + target.x), Mathf.RoundToInt(transform.position.y + target.y));
        }
    }
}
