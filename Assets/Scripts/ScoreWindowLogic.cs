using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ScoreWindowLogic : MonoBehaviour
{
    public static bool isOpen = false;

    const string sumbitButton = "Submit";

    TextMesh m_title;
    TextMesh m_text;
    SpriteRenderer m_renderer;
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_title = transform.Find("Title").GetComponent<TextMesh>();
        m_text = transform.Find("Text").GetComponent<TextMesh>();
        m_renderer = GetComponent<SpriteRenderer>();

        m_subscriberList.Add(new Event<ShowScoreWindowEvent>.Subscriber(onShowScore));
        m_subscriberList.Subscribe();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        if (Input.GetButtonDown(sumbitButton))
        {
            gameObject.SetActive(false);
            isOpen = false;
        }
    }

    private void OnMouseDown()
    {
        if (isOpen)
        {
            gameObject.SetActive(false);
            isOpen = false;
        }
    }

    public void open(string title, string data, TextAlignment alignment, float width)
    {
        isOpen = true;
        m_title.text = title;
        m_text.text = data;
        m_text.alignment = alignment;

        m_renderer.size = new Vector2(width, m_renderer.size.y);

        gameObject.SetActive(true);
    }

    void onShowScore(ShowScoreWindowEvent e)
    {
        DOVirtual.DelayedCall(0.01f, () => open(e.title, e.text, e.alignment, e.width));
        
    }
}
