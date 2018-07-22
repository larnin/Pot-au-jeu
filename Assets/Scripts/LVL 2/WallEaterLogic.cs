using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class WallEaterLogic : MonoBehaviour
{
    const string dieTrigger = "Die";

    [SerializeField] Tilemap m_map;
    [SerializeField] AudioClip m_eatRightBallClip;
    [SerializeField] AudioClip m_eatWrongBallClip;
    [SerializeField] AudioClip m_eatWallClip;

    SpriteRenderer m_renderer;
    Animator m_animator;
    PacmanLogic m_pacman;
    float m_time;
    int m_id;
    Color m_color;

    void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        m_pacman = GetComponent<PacmanLogic>();
    }
    
    void LateUpdate()
    {
        m_time -= Time.deltaTime;

        if (m_time <= 0)
        {
            m_id = 0;
            m_color = Color.white;
        }
        else eatWalls();
        m_renderer.color = new Color(m_color.r, m_color.g, m_color.b, m_renderer.color.a);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionBall(collision.gameObject.GetComponent<BallLogic>());
        collisionBoss(collision.gameObject.GetComponent<MechaBossLogic>());
    }

    void collisionBall(BallLogic ball)
    {
        if (ball == null)
            return;

        if(ball.id == 0)
        {
            onDie();
            return;
        }

        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_eatRightBallClip));
        m_id = ball.id;
        m_color = ball.color;
        m_time = ball.effectTime;
        Destroy(ball.gameObject);
    }

    void collisionBoss(MechaBossLogic boss)
    {
        if (boss == null)
            return;

        boss.kill();
    }

    void onDie()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_eatWrongBallClip));
        m_animator.SetTrigger(dieTrigger);
        Event<DieEvent>.Broadcast(new DieEvent());
    }

    void eatWalls()
    {
        var dir = m_pacman.getWantedDirection();
        int x = Mathf.RoundToInt(transform.position.x) + dir.x;
        int y = Mathf.RoundToInt(transform.position.y) + dir.y;

        var tile = m_map.GetTile<TileWIthID>(new Vector3Int(x, y, 0));
        if(tile != null)
        {
            if (tile.id == m_id)
            {
                Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_eatWallClip));
                m_map.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }
}
