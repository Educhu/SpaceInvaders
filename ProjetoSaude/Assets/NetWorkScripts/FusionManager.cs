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

    public static NetworkRunner runnerInstance;
    public string lobbyName = "Default";
    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUiDictionary = new Dictionary<String, GameObject>();
    public string lobbyGameSceneName;
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

        runnerInstance.StartGame(new StartGameArgs() { SessionName = randomSessionName, GameMode = GameMode.Shared });


    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runnerInstance.LocalPlayer)
        {
            SceneManager.LoadScene(lobbyGameSceneName);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Player conected the server");
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
                if (!isContained)
                {
                    uiToDelete = kvp.Value;
                    sessionListUiDictionary.Remove(sessionKey);
                    Destroy(uiToDelete);
                }


            }

        }


    }

    private void CompareList(List<SessionInfo> sessionlist)
    {
        foreach (SessionInfo session in sessionlist)
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

        SessionListPrefeb entryScript = newEntry.GetComponent<SessionListPrefeb>();
        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButto.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }



    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }



    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }


    void Update()
    {

    }
}
