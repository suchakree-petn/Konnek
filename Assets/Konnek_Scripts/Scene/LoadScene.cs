
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ServerAuth
{
    host,
    client
}
public class LoadScene : MonoBehaviour
{
    [SerializeField] private int buildIndex;
    [SerializeField] private ServerAuth serverAuth;
    public void Load()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(buildIndex);
    }


    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        switch (serverAuth)
        {
            case ServerAuth.host:
                NetworkManager.Singleton.StartHost();
                break;
            case ServerAuth.client:
                NetworkManager.Singleton.StartClient();
                break;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }
}
