using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField] private Boolean _mouseButton0;
    [SerializeField] private Boolean _mouseButton1;
    
    async void StartGame (GameMode gameMode) {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs() {
            GameMode = gameMode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI() {
        if (_runner == null) {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host")) {
                StartGame(GameMode.Host);
            }
            
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join")) {
                StartGame(GameMode.Client);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        if(runner.IsServer) {
            Vector3 spawnPosition = new Vector3(0, 0, 0);
            Quaternion spawnRotation = Quaternion.identity;
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, spawnRotation, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        if(_spawnedCharacters.TryGetValue(player, out NetworkObject networkPlayerObject)) {
            runner.Despawn(networkPlayerObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {
        NetworkInputData data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W)) {
            data.Direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            data.Direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            data.Direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            data.Direction += Vector3.right;
        }
        if (_mouseButton0) {
            data.Buttons |= NetworkInputData.MOUSEBUTTON1;
        }
        _mouseButton0 = false;
        if (_mouseButton1) {
            data.Buttons |= NetworkInputData.MOUSEBUTTON2;
        }
        _mouseButton1 = false;

        input.Set(data);
    }

    private void Update() {
        _mouseButton0 |= Input.GetMouseButton(0);
        _mouseButton1 |= Input.GetMouseButton(1);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}