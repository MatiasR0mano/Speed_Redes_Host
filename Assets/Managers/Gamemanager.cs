using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : NetworkBehaviour
{
    //public List<PlayerRef> _players;
    //public List<Player2> players2, revancha;
    //public static Gamemanager instance { get; private set; }
    //[SerializeField] GameObject _comenzar, _menu, _start, _winImage, _loseImage, _mensaje_abandono;
    //bool _pausa;
    //public bool comenzar;

    //void Awake()
    //{
    //    instance = this;
    //    _players = new List<PlayerRef>();
    //    _start.SetActive(true);
    //}

    //private void Update()
    //{
    //    if (comenzar)
    //    {
    //        if (players2[0] == null || players2[1] == null)
    //        {
    //            _mensaje_abandono.SetActive(true);
    //            players2[0].controlEnabled = false;
    //            players2[1].controlEnabled = false;
    //        }
    //    }
    //}

    //public void AddToList(Player2 player)
    //{
    //    var playerRef = player.Object.StateAuthority;

    //    if (_players.Contains(playerRef)) return;

    //    _players.Add(playerRef);
    //    players2.Add(player);
    //    _start.SetActive(false);
    //    player.controlEnabled = false;
    //}

    //public void RemoveFromList(PlayerRef player) => _players.Remove(player);

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void RPC_Defeat(PlayerRef player, Player2 pla)
    //{
    //    _menu.SetActive(true);
    //    if (!pla.ganador) Defeat();
    //}

    ////[RpcTarget] El llamado del RPC va a ir dirigido a ese jugador
    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void RPC_Win([RpcTarget] PlayerRef player, Player2 pla)
    //{
    //    pla.controlEnabled = false;
    //    _menu.SetActive(true);
    //    if (player == Runner.LocalPlayer && pla.ganador) Winner();
    //}

    //public void Back_to_Tittle_Screen() => RPC_Anular_Partida2(Runner.LocalPlayer);

    //[Rpc]
    //public void RPC_Anular_Partida2([RpcTarget] PlayerRef player)
    //{
    //    RemoveFromList(player);
    //    //SceneManager.LoadScene("Tittle_Screen");
    //    Player_Spawner.instance.networkRunner.Shutdown(true, ShutdownReason.Ok);
    //    SceneManager.LoadSceneAsync("Tittle_Screen");
    //}

    //public void Comenzar()
    //{
    //    _comenzar.SetActive(false);
    //    players2[0].controlEnabled = true;
    //    players2[1].controlEnabled = true;
    //    comenzar = true;
    //}

    //void Winner()
    //{
    //    _winImage.SetActive(true);
    //    players2[0].controlEnabled = false;
    //    players2[1].controlEnabled = false;
    //}

    //void Defeat()
    //{
    //    _loseImage.SetActive(true);
    //    players2[0].controlEnabled = false;
    //    players2[1].controlEnabled = false;
    //}

    //[SerializeField] GameObject _player_prefab;

    //public void Spawn_porqueRunneresnull(Vector2 vector_delCS, Quaternion Quatemala_a_Quatepeor)
    //{
    //    Runner.Spawn(_player_prefab, vector_delCS, Quatemala_a_Quatepeor);
    //}

    //public void RPC_Cambio_scene()
    //{
    //    if (Player_Spawner.instance != null) Player_Spawner.instance.networkRunner.Despawn(Player_Spawner.instance.networkRunner.GetPlayerObject(Player_Spawner.instance.networkRunner.LocalPlayer));

    //    SceneManager.LoadSceneAsync("G_Level1");
    //}



    ////public void Revancha_gamemanager()
    ////{
    ////    if (players2[0].ganador && players2[0].HasStateAuthority) players2[0].Revancha();
    ////    if (!players2[0].ganador && players2[0].HasStateAuthority) players2[0].Revancha();

    ////    if (players2[1].ganador && players2[1].HasStateAuthority) players2[1].Revancha();
    ////    if (!players2[1].ganador && players2[1].HasStateAuthority) players2[1].Revancha();
    ////}

    //public bool Pausa
    //{
    //    set { _pausa = value; }
    //    get { return _pausa; }
    //}
}
