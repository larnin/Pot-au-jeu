using UnityEngine;
using System.Collections;

public class PlayFXLogic : MonoBehaviour
{
    AudioSource m_source;
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_source = GetComponent<AudioSource>();

        m_subscriberList.Add(new Event<PlaySoundEvent>.Subscriber(onPlaySound));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();   
    }

    public void playSound(AudioClip clip)
    {
        if (clip == null)
            return;
        m_source.clip = clip;
        m_source.Play();
    }

    void onPlaySound(PlaySoundEvent e)
    {
        playSound(e.clip);
    }
}
