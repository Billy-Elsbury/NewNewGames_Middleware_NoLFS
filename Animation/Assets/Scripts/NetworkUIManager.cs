using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        // Bind buttons to their respective methods
        hostButton.onClick.AddListener(StartHost);
        serverButton.onClick.AddListener(StartServer);
        clientButton.onClick.AddListener(StartClient);
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started.");
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }

    private void StartServer()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server started.");
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }

    private void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started.");
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }
}
