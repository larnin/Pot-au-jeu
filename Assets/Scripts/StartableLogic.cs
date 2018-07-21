using UnityEngine;
using System.Collections;

public abstract class StartableLogic : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    bool m_started = false;

    private void Awake()
    {
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
    }

    protected virtual void onLateStart() { }
}
