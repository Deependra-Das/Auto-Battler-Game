using AutoBattler.Event;
using AutoBattler.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : GenericMonoSingleton<SceneLoader>
{
    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
       
        RaiseSceneLoadEventForInitialScene();
    }

    void SubscribeToEvents()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void UnsubscribeToEvents()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(SceneNameEnum sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(OnSceneLoadedCoroutine(scene));
    }

    private IEnumerator OnSceneLoadedCoroutine(Scene scene)
    {
        yield return null;

        RaiseSceneLoadEvent(scene);
    }

    private void RaiseSceneLoadEventForInitialScene()
    {
        RaiseSceneLoadEvent(SceneManager.GetActiveScene());
    }

    private void RaiseSceneLoadEvent(Scene scene)
    {
        if (Enum.TryParse(scene.name, out SceneNameEnum result))
        {
            EventBusManager.Instance.Raise(EventNameEnum.SceneLoaded, result);
        }
    }
}
