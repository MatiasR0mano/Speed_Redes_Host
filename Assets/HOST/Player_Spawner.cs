using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public class Player_Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPrefabRef _playerPrefab;
    public static Player_Spawner Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 newPosition = new Vector3();
            Quaternion newRotation = new Quaternion();
            if (Gamemanager.instance.players2.Count == 1)
            {
                newPosition = Gamemanager.instance._spawnTransforms[1].position;
                newRotation = Gamemanager.instance._spawnTransforms[1].rotation;
                Debug.Log("spaw1");
            }

            runner.Spawn(_playerPrefab, newPosition, newRotation, player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;

        input.Set(NetworkPlayer.Local.Inputs.GetLocalInputs());
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        runner.Shutdown();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
}
#region anterior
//public static Player_Spawner instance { get; private set; }

//[SerializeField] GameObject _player_prefab, _start;
//[SerializeField] private Transform[] _spawnTransforms;
//[SerializeField] bool _initialized;
//public Button button;

//public NetworkRunner networkRunner;
//private void Awake()
//{
//    instance = this;
//}

//public void PlayerJoined(PlayerRef player)
//{
//    var playersCount = Runner.SessionInfo.PlayerCount;
//    if (_initialized && playersCount >= 2)
//    {
//        //CreatePlayer(0);
//        Debug.Log("Se empezo en el primero");
//        _start.SetActive(false);
//        return;
//    }

//    if (player == Runner.LocalPlayer)
//    {
//        if (playersCount < 2)
//        {
//            _initialized = true;

//        }
//        else
//        {
//            //CreatePlayer(playersCount - 1);
//            _start.SetActive(false);
//            Debug.Log("Se empezo en el segundo");
//        }
//    }

//}

//private void Update()
//{
//    if (Gamemanager.instance.players2.Count == 2 && !Gamemanager.instance.comenzar) Gamemanager.instance.Comenzar();
//}
//public void Ready_or_not()
//{
//    if (Gamemanager.instance._players.Count == 1) CreatePlayer(Gamemanager.instance._players.Count - 1);
//    if (Gamemanager.instance._players.Count == 0) CreatePlayer(0);
//}

//void CreatePlayer(int spawnPointIndex)
//{
//    button.interactable = false;
//    _initialized = false;
//    var newPosition = _spawnTransforms[spawnPointIndex].position;
//    var newRotation = _spawnTransforms[spawnPointIndex].rotation;
//    //Runner.Spawn(_player_prefab, newPosition, newRotation);
//    Gamemanager.instance.Spawn_porqueRunneresnull(newPosition, newRotation);
//}
#endregion