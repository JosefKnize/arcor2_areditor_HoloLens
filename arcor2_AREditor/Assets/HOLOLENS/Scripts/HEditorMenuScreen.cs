using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Base;
using IO.Swagger.Model;
using Newtonsoft.Json;
using System.Linq;
using MixedReality.Toolkit;

public class HEditorMenuScreen : Singleton<HEditorMenuScreen>
{
    // public StatefulInteractable closeSceneButton;
    // public StatefulInteractable notificationButton;
    public StatefulInteractable switchSceneState;

    // Start is called before the first frame update
    void Start()
    {
        SceneManagerH.Instance.OnSceneStateEvent += OnSceneStateEvent;
    }

    public async void SaveScene()
    {
        //SceneManagerH.Instance.UnlockAllObjects();
        IO.Swagger.Model.SaveSceneResponse saveSceneResponse = await GameManagerH.Instance.SaveScene();
        if (!saveSceneResponse.Result)
        {
            saveSceneResponse.Messages.ForEach(Debug.LogError);
            HNotificationManager.Instance.ShowNotification("Scene save failed: " + (saveSceneResponse.Messages.Count > 0 ? saveSceneResponse.Messages[0] : "Failed to save scene"));
            return;
        }
        else
        {
            HNotificationManager.Instance.ShowNotification("There are no unsaved changes");
        }
    }

    public async void SaveProject()
    {
        IO.Swagger.Model.SaveProjectResponse saveProjectResponse = await WebSocketManagerH.Instance.SaveProject();
        if (saveProjectResponse != null && !saveProjectResponse.Result)
        {
            saveProjectResponse.Messages.ForEach(Debug.LogError);
            HNotificationManager.Instance.ShowNotification("Failed to save project " + (saveProjectResponse.Messages.Count > 0 ? saveProjectResponse.Messages[0] : ""));
            return;
        }
    }


    public async void CloseScene()
    {

        if (GameManagerH.Instance.GetGameState() == GameManagerH.GameStateEnum.SceneEditor)
        {
            SaveScene();
        }
        else if (GameManagerH.Instance.GetGameState() == GameManagerH.GameStateEnum.ProjectEditor)
        {
            SaveProject();
        }


        if (SceneManagerH.Instance.SceneStarted)
            WebSocketManagerH.Instance.StopScene(false, null);

        bool success = false;
        string message;
        if (GameManagerH.Instance.GetGameState() == GameManagerH.GameStateEnum.SceneEditor)
        {
            (success, message) = await GameManagerH.Instance.CloseScene(true);
        }
        else if (GameManagerH.Instance.GetGameState() == GameManagerH.GameStateEnum.ProjectEditor)
        {
            (success, message) = await GameManagerH.Instance.CloseProject(true);
        }
    }

    private void OnSceneStateEvent(object sender, SceneStateEventArgs args)
    {
        if (args.Event.State == IO.Swagger.Model.SceneStateData.StateEnum.Started)
        {
            switchSceneState.ForceSetToggled(true);
        }
        else
        {
            switchSceneState.ForceSetToggled(false);
        }
    }


    public void SwitchSceneState()
    {
        if (SceneManagerH.Instance.SceneStarted)
            StopScene();
        else
            StartScene();
    }

    public async void StartScene()
    {
        try
        {
            await WebSocketManagerH.Instance.StartScene(false);
        }
        catch (RequestFailedException e)
        {
            HNotificationManager.Instance.ShowNotification("Going online failed " + e.Message);
        }
    }

    private void StopSceneCallback(string _, string data)
    {
        CloseProjectResponse response = JsonConvert.DeserializeObject<CloseProjectResponse>(data);
        if (!response.Result)
            HNotificationManager.Instance.ShowNotification("Going offline failed " + response.Messages.FirstOrDefault());
    }

    public void StopScene()
    {
        WebSocketManagerH.Instance.StopScene(false, StopSceneCallback);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
