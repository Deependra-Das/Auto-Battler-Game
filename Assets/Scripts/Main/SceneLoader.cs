using AutoBattler.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : GenericMonoSingleton<SceneLoader>
{
    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

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
        UIManager.Instance.ToggleMainMenuUIContainer(false);
        UIManager.Instance.ToggleStageSelectionUIContainer(false);
        UIManager.Instance.ToggleGameplayUIContainer(false);

        switch (scene.name)
        {
            case nameof(SceneNameEnum.MainMenuScene):
                UIManager.Instance.ToggleMainMenuUIContainer(true);
                break;

            case nameof(SceneNameEnum.StageSelectionScene):
                UIManager.Instance.ToggleStageSelectionUIContainer(true);
                break;

            case nameof(SceneNameEnum.GameplayScene):
                UIManager.Instance.ToggleGameplayUIContainer(true);
                break;
        }
    }
}
