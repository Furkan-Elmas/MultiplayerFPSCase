using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] NetworkManagerLobby _networkManager;

    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;

    public void HostLobby()
    {
        _networkManager.StartHost();

        _landingPagePanel.SetActive(false);
    }
}
