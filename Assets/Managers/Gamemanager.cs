using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : NetworkBehaviour
{
    public List<PlayerRef> _players;
    public List<NetworkPlayer> players2, revancha;
    public static Gamemanager instance { get; private set; }
    [SerializeField] GameObject _comenzar, _menu, _start, _winImage, _loseImage, _mensaje_abandono, one, two, three;
    [SerializeField] Button _aceptar;
    bool _pausa;
    public bool comenzar, start_game;
    public Transform[] _spawnTransforms;

    void Awake()
    {
        instance = this;
        _players = new List<PlayerRef>();
        //_start.SetActive(true);
        _aceptar.onClick.AddListener(Aceptar);
    }

    private void Update()
    {
        if (comenzar)
        {
            if (players2[0] == null || players2[1] == null)
            {
                _mensaje_abandono.SetActive(true);
                players2[0].controlEnabled = false;
                players2[1].controlEnabled = false;
            }
        }
        if (!comenzar && start_game)
        {
            StartCoroutine(Cuenta_atras());
        }
        if (players2[0] != null && players2[1] != null)
        {
            if (players2[0].pos != null && players2[1].pos != null && !start_game)
            {
                Teleport();
            }
        }

    }

    #region todo lo que funca bien


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddToList(NetworkPlayer player)
    {
        var playerRef = player.Object.StateAuthority;

        if (_players.Contains(playerRef)) return;

        _players.Add(playerRef);
        players2.Add(player);
        //_start.SetActive(false);
        //player.controlEnabled = false;
    }

    public void RemoveFromList(PlayerRef player)
    {
        _players.Remove(player);
        if (!Object.HasInputAuthority) Runner.Disconnect(Object.InputAuthority);

        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Defeat(PlayerRef player, NetworkPlayer pla)
    {
        _menu.SetActive(true);
        if (!pla.ganador) Defeat();
    }

    //[RpcTarget] El llamado del RPC va a ir dirigido a ese jugador para que no salte error tiene que tener el nombre RPC_
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Win([RpcTarget] PlayerRef player, NetworkPlayer pla)
    {
        pla.controlEnabled = false;
        _menu.SetActive(true);
        if (player == Runner.LocalPlayer && pla.ganador) Winner();
    }

    public void Back_to_Tittle_Screen() => RPC_Anular_Partida2(Runner.LocalPlayer);

    [Rpc]
    public void RPC_Anular_Partida2([RpcTarget] PlayerRef player)
    {
        RemoveFromList(player);
        SceneManager.LoadSceneAsync("Tittle_Screen");
    }
    void Winner()
    {
        _winImage.SetActive(true);
        players2[0].controlEnabled = false;
        players2[1].controlEnabled = false;
    }

    void Defeat()
    {
        _loseImage.SetActive(true);
        players2[0].controlEnabled = false;
        players2[1].controlEnabled = false;
    }
    #endregion

    public void Teleport()
    {
        players2[0].transform.position = players2[0].pos.position;
        players2[1].transform.position = players2[1].pos.position;
        start_game = true;
    }

    public IEnumerator Cuenta_atras()
    {
        start_game = false;
        one.SetActive(true);
        yield return new WaitForSeconds(1f);
        one.SetActive(false);
        two.SetActive(true);
        yield return new WaitForSeconds(1f);
        two.SetActive(false);
        three.SetActive(true);
        yield return new WaitForSeconds(1f);
        three.SetActive(false);
        players2[0].controlEnabled = true;
        players2[1].controlEnabled = true;
        comenzar = true;
    }

    public void Aceptar()
    {
        _aceptar.interactable = false;
        if (players2.Count == 0)
        {
            players2[0].pos = _spawnTransforms[0].transform;
        }
        if (players2.Count == 1)
        {
            players2[0].pos = _spawnTransforms[1].transform;
        }
    }
}
