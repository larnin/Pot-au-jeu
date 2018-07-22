using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainLogic : MonoBehaviour
{
    [SerializeField] string m_lobbyScene = "Lobby";
    [SerializeField] float m_duration = 2;
    bool m_pressed = false;

    private void Awake()
    {
        DOVirtual.DelayedCall(0.01f, () =>
        {
            var button = GetComponentInChildren<Button>();
            if (button != null)
                button.Select();
        });

    }

    public void onStartPress()
    {
        if (m_pressed)
            return;

        m_pressed = true;

        foreach (var r in GetComponentsInChildren<Graphic>())
        {
            r.DOColor(new Color(r.color.r, r.color.g, r.color.b, 0), m_duration);
        }

        DOVirtual.DelayedCall(m_duration + 0.2f, () => SceneSystem.changeScene(m_lobbyScene, true));
    }
}