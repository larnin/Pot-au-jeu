using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainLogic : MonoBehaviour
{
    [SerializeField] string lobbyScene = "Lobby";

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
        SceneSystem.changeScene(lobbyScene, true);
    }
}