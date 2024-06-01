using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        LobbyScene,
        GameScene,
        LoadingScene,
        MainMenuScene
    }
    static string TransScene(string scene)
    {
        string returnScene = "LoadingScene";
        if (scene == "LobbyScene")
        {
            returnScene = "Lobby";
        }
        else if (scene == "GameScene")
        {
            returnScene = "KonnekGame";
        }
        else if (scene == "MainMenuScene")
        {
            returnScene = "MainMenu";
        }
        return returnScene;
    }
    private static Scene targetScene;
    public static Scene TargetScene => targetScene;

    public static void Load(Scene _targetScene)
    {
        Loader.targetScene = _targetScene;

        SceneManager.LoadScene(TransScene(Scene.LoadingScene.ToString()));
    }
    public static void LoadNetwork(Scene targetScene)
    {
        Loader.targetScene = targetScene;


        NetworkManager.Singleton.SceneManager.LoadScene(TransScene(targetScene.ToString()), LoadSceneMode.Single);
    }

    public static void LoadingScene()
    {
        SceneManager.LoadScene(TransScene(Scene.LoadingScene.ToString()), LoadSceneMode.Additive);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(TransScene(targetScene.ToString()));
    }
}
