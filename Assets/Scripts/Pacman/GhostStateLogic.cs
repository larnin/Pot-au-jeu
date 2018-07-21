using UnityEngine;
using System.Collections;

public class GhostStateLogic : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    float  m_vulnerabilityTime = 0;

    private void Awake()
    {
        m_subscriberList.Add(new Event<TakePowerupEvent>.Subscriber(onPickPowerup));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        m_vulnerabilityTime -= Time.deltaTime;
    }

    public bool isVulnerable()
    {
        return m_vulnerabilityTime > 0;
    }

    public void eat()
    {

    }

    void onPickPowerup(TakePowerupEvent e)
    {
        m_vulnerabilityTime = e.time;
    }
}
