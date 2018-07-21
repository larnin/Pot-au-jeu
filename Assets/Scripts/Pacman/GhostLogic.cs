using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

class GhostLogic : StartableLogic
{
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

    [SerializeField] float m_speed;
    [SerializeField] List<int> m_walkableIDs;

    Tween m_tween;
    List<Position> m_path = new List<Position>();

    protected override void onAwake()
    {
        
    }

    public bool haveFinishedPath()
    {
        return m_path.Count == 0;
    }

    public void setTarget(int x, int y)
    {
        m_path.Clear();

        int px = Mathf.RoundToInt(transform.position.x);
        int py = Mathf.RoundToInt(transform.position.y);

        List<Node> nextPos = new List<Node>();
        List<Node> visited = new List<Node>();

        nextPos.Add(new Node(null, px, py));

        bool foundEnd = false;

        while(nextPos.Count > 0)
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
            nextNodes.Add(new Node(pos, pos.x, pos.y+1));
            nextNodes.Add(new Node(pos, pos.x, pos.y-1));

            foreach(var n in nextNodes)
            {
                var id = TilemapInfos.instance.tileID(n.x, n.y);
                if (!m_walkableIDs.Contains(id))
                    continue;
                if (visited.Find(v => { return v.x == n.x && v.y == n.y; }) != null)
                    continue;
                if (nextPos.Find(v => { return v.x == n.x && v.y == n.y; }) != null)
                    continue;
                nextPos.Add(n);
            }
        }

        int currentPos = visited.Count() - 1;
        float bestDist = float.MaxValue;
        if(!foundEnd)
        {
            for(int i = 0; i < visited.Count; i++)
            {
                var d = (new Vector2(visited[i].x, visited[i].y) - new Vector2(x, y)).sqrMagnitude;
                if(d < bestDist)
                {
                    bestDist = d;
                    currentPos = i;
                }
            }
        }

        var current = visited[currentPos];

        m_path.Add(new Position(current.x, current.y));

        for(int i = currentPos - 1; i >= 0; i--)
        {
            if(current.parent == visited[i])
            {
                current = visited[i];
                m_path.Insert(0, new Position(current.x, current.y));
            }
            if (current.parent == null)
                break;
        }

        if (m_path.Count() > 0 && m_path[0].x == px && m_path[0].y == py)
            m_path.RemoveAt(0);

        if(m_tween == null || m_tween.IsComplete() || !m_tween.IsPlaying() || !m_tween.IsActive())
            startNextMove();
    }

    void startNextMove()
    {
        if (m_tween != null)
        {
            m_tween.Kill();
            m_tween = null;
        }

        if (m_path.Count == 0)
            return;

        var pos = m_path[0];
        m_path.RemoveAt(0);

        float d = (new Vector2(transform.position.x, transform.position.y) - new Vector2(pos.x, pos.y)).magnitude;

        m_tween = transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), 1 / m_speed * d).SetEase(Ease.Linear).OnComplete(startNextMove);
    }
}