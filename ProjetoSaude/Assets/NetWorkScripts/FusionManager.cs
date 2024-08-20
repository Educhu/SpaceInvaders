using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using UnityEngine.SceneManagement;

public class FusionManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public static NetworkRunner runnerInstance;
    public string lobbyName = "Default";
    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUiDictionary = new Dictionary<String, GameObject>();

    public string lobbyGameSceneName;
    public GameObject botaoCriacao;

    private void Awake()
    {
        runnerInstance = gameObject.GetComponent<NetworkRunner>();

        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }

    private void Start()
    {
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }

    public void CreatedRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Room" + randomInt.ToString();

        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(lobbyGameSceneName)),
            SessionName = randomSessionName,
            GameMode = GameMode.Host // O servidor (host) deve criar a sess�o e instanciar o PlayerPrefab
        });
    }

    public void JoinSession(string sessionName)
    {
        if (runnerInstance != null)
        {
            Debug.Log($"Tentando entrar na sess�o: {sessionName}");

            runnerInstance.StartGame(new StartGameArgs()
            {
                SessionName = sessionName,
                GameMode = GameMode.Client, // Define como Client para entrar em uma sess�o existente.
                Scene = SceneRef.None // Use None se a cena j� estiver carregada, ou use SceneRef se precisar carregar uma cena espec�fica.
            });
        }
        else
        {
            Debug.LogError("runnerInstance n�o est� inicializado.");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Defina os limites da �rea onde voc� deseja que o jogador apare�a
            float minX = -5f; // M�nimo valor de X
            float maxX = 5f;  // M�ximo valor de X
            float minZ = -5f; // M�nimo valor de Z
            float maxZ = 5f;  // M�ximo valor de Z

            // Gera uma posi��o aleat�ria dentro dos limites especificados
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomZ = UnityEngine.Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, 1, randomZ);

            // Instancia o objeto de rede do jogador
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            // Atualiza a posi��o do jogador (acessa o Transform do NetworkObject)
            UpdatePlayerPosition(networkPlayerObject, spawnPosition);

            // Mant�m o controle dos avatares dos jogadores para acesso f�cil
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    private void UpdatePlayerPosition(NetworkObject networkPlayerObject, Vector3 position)
    {
        // Acessa o Transform do NetworkObject e atualiza a posi��o
        Transform playerTransform = networkPlayerObject.transform;
        playerTransform.position = position;
    }

    public int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Player connected to the server");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        DeleteOldSessionsFromUI(sessionList);
        CompareList(sessionList);
    }

    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;

        foreach (KeyValuePair<string, GameObject> kvp in sessionListUiDictionary)
        {
            string sessionKey = kvp.Key;

            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.Name == sessionKey)
                {
                    isContained = true;
                    break;
                }
            }

            if (!isContained)
            {
                uiToDelete = kvp.Value;
                sessionListUiDictionary.Remove(sessionKey);
                Destroy(uiToDelete);
            }
        }
    }

    private void CompareList(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (sessionListUiDictionary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CrateEntryUi(session);
            }
        }
    }

    private void CrateEntryUi(SessionInfo session)
    {
        GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParent;
        SessionListPrefeb entryScript = newEntry.GetComponent<SessionListPrefeb>();
        sessionListUiDictionary.Add(session.Name, newEntry);

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButto.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }

    private void UpdateEntryUI(SessionInfo session)
    {
        sessionListUiDictionary.TryGetValue(session.Name, out GameObject newEntry);

        if (newEntry != null)
        {
            SessionListPrefeb entryScript = newEntry.GetComponent<SessionListPrefeb>();
            entryScript.roomName.text = session.Name;
            entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
            entryScript.joinButto.interactable = session.IsOpen;

            newEntry.SetActive(session.IsVisible);
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Failed to connect to server: {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Handle custom authentication if needed
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // Handle custom authentication response if needed
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server: {reason}");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // Handle host migration if needed
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Handle network input if needed
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Handle missing input if needed
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Handle object entering Area of Interest if needed
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Handle object exiting Area of Interest if needed
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // Handle reliable data progress if needed
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // Handle reliable data received if needed
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Ensure the player prefab is instantiated only when necessary
        if (!runner.IsServer)
        {
            if (runner.GetPlayerObject(runner.LocalPlayer) == null)
            {
                Vector3 spawnPosition = new Vector3(0, 1, 0); // Position as needed
                runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, runner.LocalPlayer);
            }
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // Handle scene load start if needed
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // Handle shutdown if needed
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // Handle user simulation messages if needed
    }


    void Update()
    {
        if (runnerInstance.IsCloudReady)
        {
            Debug.Log("NetworkRunner est� pronto para a nuvem.");

            // Ativa o GameObject "botaoCriacao" se estiver atribu�do
            if (botaoCriacao != null)
            {
                botaoCriacao.SetActive(true);
            }
            else
            {
                Debug.LogWarning("O GameObject 'botaoCriacao' n�o foi atribu�do no inspetor.");
            }
        }
        else
        {
            Debug.LogWarning("NetworkRunner n�o est� pronto para a nuvem.");
        }
    }
}
