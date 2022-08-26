using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] NetworkManagerLobby _networkManager;

    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;
    [SerializeField] TMP_InputField _ipAddressInputField;
    [SerializeField] Button _joinButton;

    void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = _ipAddressInputField.text;

        _networkManager.StartClient();

        _networkManager.networkAddress = ipAddress;

        _joinButton.interactable = false;
    }

    void HandleClientConnected()
    {
        _joinButton.interactable = true;

        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }

    void HandleClientDisconnected()
    {
        _joinButton.interactable = false;
    }
}
