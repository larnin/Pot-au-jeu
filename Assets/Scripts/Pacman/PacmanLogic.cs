using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class PacmanLogic : StartableLogic
{
    const string horizontalAxis = "Horizontal";
    const string verticalAxis = "Vertical";

    enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] float m_speed = 1;
    [SerializeField] List<int> m_walkableIDs;
    [SerializeField] float m_powerupTimeEffect = 5;

    Tween m_tween = null;
    Direction m_direction = Direction.None;
    Direction m_currentDirection = Direction.None;

    public Vector2Int getDirection()
    {
        switch(m_currentDirection)
        {
            case Direction.Down:
                return new Vector2Int(0, 1);
            case Direction.Up:
                return new Vector2Int(0, -1);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Right:
                return new Vector2Int(1, 0);
            default:
                return new Vector2Int(0, 0);
        }
    }

    protected override void onUpdate()
    {
        computeInputs();
        if (m_tween == null)
            startNextMove();
        if (m_currentDirection == Direction.Left && m_direction == Direction.Right ||
            m_currentDirection == Direction.Right && m_direction == Direction.Left ||
            m_currentDirection == Direction.Up && m_direction == Direction.Down ||
            m_currentDirection == Direction.Down && m_direction == Direction.Up)
            startNextMove();
    }

    void computeInputs()
    {
        float x = Input.GetAxisRaw(horizontalAxis);
        float y = Input.GetAxisRaw(verticalAxis);

        if (Mathf.Abs(x) < 0.5f && Mathf.Abs(y) < 0.5f)
            return;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0)
                m_direction = Direction.Left;
            else m_direction = Direction.Right;
        }
        else if (y < 0)
            m_direction = Direction.Up;
        else m_direction = Direction.Down;
    }

    protected override void onLateStart()
    {
        startNextMove();
    }

    void startNextMove()
    {
        if (m_tween != null)
        {
            m_tween.Kill();
            m_tween = null;
        }

        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        int nextX = x + (m_direction == Direction.Left ? -1 : m_direction == Direction.Right ? 1 : 0);
        int nextY = y + (m_direction == Direction.Down ? 1 : m_direction == Direction.Up ? -1 : 0);

        float d = (new Vector2(transform.position.x, transform.position.y) - new Vector2(nextX, nextY)).magnitude;
        
        if (m_direction == Direction.None)
            return;

        var nextID = TilemapInfos.instance.tileID(nextX, nextY);

        if (!m_walkableIDs.Contains(nextID))
        {
            if(m_currentDirection == m_direction)
                return;
            var tempDir = m_direction;
            m_direction = m_currentDirection;
            startNextMove();
            if (m_tween != null)
                m_direction = tempDir;
            return;
        }

        m_tween = transform.DOMove(new Vector3(nextX, nextY, transform.position.z), 1 / m_speed * d).SetEase(Ease.Linear).OnComplete(startNextMove);
        m_currentDirection = m_direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionPowerup(collision.gameObject.GetComponent<PowerupLogic>());
        collisionGhost(collision.gameObject.GetComponent<GhostStateLogic>());
    }

    void collisionPowerup(PowerupLogic powerup)
    {
        if (powerup == null)
            return;

        if(powerup.isPickable())
        {
            powerup.pickup();
            Event<TakePowerupEvent>.Broadcast(new TakePowerupEvent(m_powerupTimeEffect));
        }
    }

    void collisionGhost(GhostStateLogic ghost)
    {
        if (ghost == null)
            return;

        if (ghost.isVulnerable() && !ghost.isEaten())
            ghost.eat();
        if(!ghost.isEaten() && !ghost.isVulnerable())
        {
            Event<DieEvent>.Broadcast(new DieEvent());
        }
    }
}
