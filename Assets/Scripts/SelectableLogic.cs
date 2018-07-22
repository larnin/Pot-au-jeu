using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectableLogic : MonoBehaviour
{
    [SerializeField] string m_name;

    Collider2D m_collider;

    void Awake()
    {
        m_collider = GetComponent<Collider2D>();
    }

    private void OnMouseEnter()
    {
        Debug.Log("poop");
    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseDown()
    {
        
    }
}
