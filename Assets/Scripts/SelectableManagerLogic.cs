﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectableManagerLogic : MonoBehaviour
{
    const string horizontalAxis = "Horizontal";
    const string submitButton = "Submit";

    [SerializeField] AudioClip m_openArcadeClip;
    [SerializeField] AudioClip m_openLinkClip;

    static List<SelectableLogic> m_selectables = new List<SelectableLogic>();

    static SelectableLogic m_selected;

    float m_lastHorizontal = 0;

    public static void add(SelectableLogic selectable)
    {
        m_selectables.Add(selectable);
    }

    public static void remove(SelectableLogic selectable)
    {
        m_selectables.Remove(selectable);
    }

    public static void enter(SelectableLogic selectable)
    {
        if (m_selected != null)
            m_selected.deselect();
        m_selected = selectable;
    }

    public static void exit(SelectableLogic selectable)
    {
        if(m_selected == selectable)
            m_selected = null;
    }

    void Update()
    {
        float h = Input.GetAxisRaw(horizontalAxis);
        if (Mathf.Abs(h) > 0.5f && Mathf.Abs(m_lastHorizontal) <= 0.5f)
            selectNext(h > 0);

        m_lastHorizontal = h;
        
        if (Input.GetButtonDown(submitButton) && m_selected != null)
            m_selected.click();
    }

    void selectNext(bool right)
    {
        float x = right ? float.MaxValue : float.MinValue;
        float start = m_selected != null ? m_selected.transform.position.x : right ? float.MinValue : float.MaxValue;
        SelectableLogic best = m_selected;
        foreach(var selectable in m_selectables)
        {
            var sX = selectable.transform.position.x;
            if (right && sX > start && sX < x)
            {
                x = sX;
                best = selectable;
            }
            if(!right && sX < start && sX > x)
            {
                x = sX;
                best = selectable;
            }
        }

        if (m_selected != null)
            m_selected.deselect();
        m_selected = best;
        if (m_selected != null)
            m_selected.select();
    }

    public void onQuitClick()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Application.Quit();
    }

    public void onLvlClick(string sceneName)
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openArcadeClip));
        States.instance.plays++;
        SceneSystem.changeScene(sceneName);
    }

    public void onClickPAJ()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Application.OpenURL("https://itch.io/jam/pot-au-jeu");
    }

    public void onClickNico()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Application.OpenURL("http://nicolas.laurent.re/");
    }

    public void onClickIxe()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Application.OpenURL("https://www.twitch.tv/llxll");
    }

    public void onOpenScores()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Event<ShowScoreWindowEvent>.Broadcast(new ShowScoreWindowEvent("Scores", "Not implemented yet", TextAlignment.Center, 15));
    }

    public void onOpenHowToPlay()
    {
        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_openLinkClip));
        Event<ShowScoreWindowEvent>.Broadcast(new ShowScoreWindowEvent("Comment jouer", "C'est tres simple,\nil vous suffis de tuer\nle boss de chaque borne\nd'arcade :)\n\nTouches fléchés \npour se déplacer", TextAlignment.Left, 15));
    }
}
