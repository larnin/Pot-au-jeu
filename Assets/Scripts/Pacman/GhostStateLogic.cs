using UnityEngine;
using System.Collections;

public class GhostStateLogic : MonoBehaviour
{
    const string vulnerableProperty = "IsVulnerable";
    const string eatenProperty = "IsEaten";

    SubscriberList m_subscriberList = new SubscriberList();

    float  m_vulnerabilityTime = 0;
    bool m_eaten = false;

    Animator m_animator;
    BaseColoredGhostLogic m_coloredGhost;
    VulnerableGhostLogic m_vulnerableGhost;
    EatenGhostLogic m_eatenGhost;
    GhostLogic m_ghost;

    private void Awake()
    {
        m_subscriberList.Add(new Event<TakePowerupEvent>.Subscriber(onPickPowerup));
        m_subscriberList.Subscribe();

        m_animator = GetComponent<Animator>();
        m_coloredGhost = GetComponent<BaseColoredGhostLogic>();
        m_vulnerableGhost = GetComponent<VulnerableGhostLogic>();
        m_eatenGhost = GetComponent<EatenGhostLogic>();
        m_ghost = GetComponent<GhostLogic>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        m_vulnerabilityTime -= Time.deltaTime;
        if (m_vulnerabilityTime <= 0)
        {
            m_animator.SetBool(vulnerableProperty, false);
            changeState();
        }

            if (isEaten() && m_ghost.haveFinishedPath())
        {
            m_eaten = false;
            m_animator.SetBool(eatenProperty, false);
            changeState();
        }
    }

    public bool isVulnerable()
    {
        return m_vulnerabilityTime > 0;
    }

    public void eat()
    {
        m_eaten = true;
        m_animator.SetBool(eatenProperty, true);

        changeState();
    }

    void onPickPowerup(TakePowerupEvent e)
    {
        m_animator.SetBool(vulnerableProperty, true);
        m_vulnerabilityTime = e.time;
        changeState();
    }

    public bool isEaten()
    {
        return m_eaten;
    }

    void changeState()
    {
        if(isEaten())
        {
            m_eatenGhost.enabled = true;
            m_vulnerableGhost.enabled = false;
            m_coloredGhost.enabled = false;
        }
        else if(isVulnerable())
        {
            m_eatenGhost.enabled = false;
            m_vulnerableGhost.enabled = true;
            m_coloredGhost.enabled = false;
        }
        else
        {
            m_eatenGhost.enabled = false;
            m_vulnerableGhost.enabled = false;
            m_coloredGhost.enabled = true;
        }
    }
}
