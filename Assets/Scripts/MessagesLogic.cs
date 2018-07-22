using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MessagesLogic : MonoBehaviour
{
    [SerializeField] GameObject m_startMessage;
    [SerializeField] GameObject m_endWinMessage;
    [SerializeField] GameObject m_dieMessage;
    [SerializeField] float m_startMessageTime = 1.0f;
    [SerializeField] float m_timeBeforeEndMessage = 1.0f;
    [SerializeField] float m_timeBeforeBackLobby = 3.0f;
    [SerializeField] string m_lobbyName = "Lobby";
    [SerializeField] AudioClip m_startClip;
    [SerializeField] AudioClip m_winClip;
    [SerializeField] AudioClip m_looseClip;

    SubscriberList m_subscriberList = new SubscriberList();
    bool m_dead = false;

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartEvent>.Subscriber(onStart));
        m_subscriberList.Add(new Event<DieEvent>.Subscriber(onDie));
        m_subscriberList.Add(new Event<BossDieEvent>.Subscriber(onBossDieEvent));
        m_subscriberList.Subscribe();

        m_startMessage.SetActive(false);
        m_endWinMessage.SetActive(false);
        m_dieMessage.SetActive(false);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void onStart(StartEvent e)
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_startClip));
        m_startMessage.SetActive(true);
        DOVirtual.DelayedCall(m_startMessageTime, () => m_startMessage.SetActive(false));
    }

    void onDie(DieEvent e)
    {
        if (m_dead)
            return;
        m_dead = true;

        DOVirtual.DelayedCall(m_timeBeforeEndMessage, () =>
        {
            Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_looseClip));
            m_dieMessage.SetActive(true);
        });
        DOVirtual.DelayedCall(m_timeBeforeBackLobby, () => SceneSystem.changeScene(m_lobbyName));
        States.instance.deaths++;
    }

    void onBossDieEvent(BossDieEvent e)
    {
        if (m_dead)
            return;
        m_dead = true;

        DOVirtual.DelayedCall(m_timeBeforeEndMessage, () =>
        {
            Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_winClip));
            m_endWinMessage.SetActive(true);
        });
        DOVirtual.DelayedCall(m_timeBeforeBackLobby, () => SceneSystem.changeScene(m_lobbyName));

        States.instance.setFiniedScene(SceneManager.GetActiveScene().name);
    }
}

