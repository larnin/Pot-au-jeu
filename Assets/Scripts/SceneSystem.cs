﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem
{
    const string defaultSceneName = "Main";

    public static void changeScene(string sceneName, bool instant = false, Action finishedCallback = null)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Can't load scene " + sceneName + ". Back to main menu");
            sceneName = defaultSceneName;
        }

        Time.timeScale = 1.0f;

        Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(true));
        DOVirtual.DelayedCall(1.5f, () =>
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            execChangeScene(operation, finishedCallback);
        });

        if(instant)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            execChangeScene(operation, finishedCallback);
        }
    }

    static void execChangeScene(AsyncOperation operation, Action finishedCallback)
    {
        if (!operation.isDone)
            DOVirtual.DelayedCall(0.1f, () => execChangeScene(operation, finishedCallback));
        else
        {
            Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(false));
            if (finishedCallback != null)
                finishedCallback();
        }
    }
}
