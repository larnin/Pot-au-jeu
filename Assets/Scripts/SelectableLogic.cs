using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SelectableLogic : MonoBehaviour
{
    [SerializeField] string m_name;
    [SerializeField] UnityEvent m_event;

    Collider2D m_collider;
    TextMesh m_text;
    SpriteOutline m_outline;

    void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_text = GetComponentInChildren<TextMesh>();
        m_outline = GetComponent<SpriteOutline>();
        m_outline.enabled = false;
        m_text.gameObject.SetActive(false);

        SelectableManagerLogic.add(this);
    }

    private void OnDestroy()
    {
        SelectableManagerLogic.remove(this);
    }

    private void OnDisable()
    {
        SelectableManagerLogic.remove(this);
        onExit();
    }

    private void OnMouseEnter()
    {
        if (!enabled)
            return;

        SelectableManagerLogic.enter(this);
        onEnter();
    }

    private void OnMouseExit()
    {
        if (!enabled)
            return;

        SelectableManagerLogic.exit(this);
        onExit();
    }

    private void OnMouseDown()
    {
        if (!enabled)
            return;

        click();
    }

    public void select()
    {
        onEnter();
    }

    public void deselect()
    {
        onExit();
    }

    public void click()
    {
        if (ScoreWindowLogic.isOpen)
            return;
        if (m_event != null)
            m_event.Invoke();
    }

    void onEnter()
    {
        m_text.gameObject.SetActive(true);
        m_outline.enabled = true;
    }

    void onExit()
    {
        m_text.gameObject.SetActive(false);
        m_outline.enabled = false;
    }
}
