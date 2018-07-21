using UnityEngine;
using System.Collections;

public abstract class StartableLogic : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    bool m_started = false;
    bool m_haveAwake = false;

    private void Awake()
    {
        if (m_haveAwake)
            return;
        m_haveAwake = true;

        m_subscriberList.Add(new Event<StartEvent>.Subscriber(onStartEvent));
        m_subscriberList.Subscribe();

        onAwake();
    }

    protected virtual void onAwake() { }

    void Start()
    {
        onStart();
    }

    protected virtual void onStart() { }

    private void OnEnable()
    {
        Awake();

        if (!m_started)
            return;
        onEnable();
    }

    protected virtual void onEnable() { }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
        onDestroy();
    }

    protected virtual void onDestroy() { }

    void Update()
    {
        if (m_started)
            onUpdate();
    }

    protected virtual void onUpdate() { }

    private void FixedUpdate()
    {
        if (m_started)
            onFixedUpdate();
    }

    protected virtual void onFixedUpdate() { } 

    private void LateUpdate()
    {
        if (m_started)
            onLateUpdate();
    }

    protected virtual void onLateUpdate() { }

    void onStartEvent(StartEvent e)
    {
        m_started = true;
        onLateStart();
        if(enabled)
            onEnable();
    }

    protected virtual void onLateStart() { }
}
