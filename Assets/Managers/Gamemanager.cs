using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : NetworkBehaviour
{
    public List<PlayerRef> _players;
    public List<NetworkPlayer> players2;
    public NetworkPlayer player_net;
    public static Gamemanager instance { get; private set; }
    [SerializeField] GameObject _comenzar, _menu, _start, _winImage, _loseImage, _mensaje_abandono, one, two, three;
    [SerializeField] Button _aceptar;
    bool _pausa;
    public bool comenzar, start_game, teleport;
    int index;
    public Transform[] _spawnTransforms;
    public List<Toggle> _aceptar_toggle;
    void Awake()
    {
        instance = this;
        _players = new List<PlayerRef>();
        //_start.SetActive(true);
        _aceptar.onClick.AddListener(RPC_Aceptar);
        _aceptar.onClick.AddListener(Desactivar);
        _aceptar.interactable = false;
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
            start_game = false;
            StartCoroutine(Cuenta_atras());
        }
        if (players2[0].pos != null && players2[1].pos != null && !teleport)
        {
            RPC_Teleport();
            teleport = true;
        }
    }

    #region todo lo que funca bien

    public void Add_player(NetworkPlayer player) => player_net = player;
    public void RPC_AddToList(NetworkPlayer player)
    {
        var playerRef = player.Object.StateAuthority;
        //if (_players.Contains(playerRef)) return;

        _players.Add(playerRef);
        players2.Add(player);
        //_start.SetActive(false);
        //player.controlEnabled = false;
        if (players2.Count == 2) _aceptar.interactable = true;
        else _aceptar.interactable = false;
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Teleport()
    {
        _start.SetActive(false);
        Debug.Log("teleport");
        start_game = true;
    }

    public IEnumerator Cuenta_atras()
    {
        one.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        one.SetActive(false);
        two.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        two.SetActive(false);
        three.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        three.SetActive(false);
        players2[0].controlEnabled = true;
        players2[1].controlEnabled = true;
        comenzar = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Aceptar()
    {
        Ready();
        if (players2[0].pos != null && players2[0].i_am_host)
        {
            if (_aceptar_toggle[0].isOn)
            {
                _aceptar_toggle[1].isOn = true;
                players2[1].pos = _spawnTransforms[1];
            }
            else
            {
                players2[1].controlEnabled = false;

                _aceptar_toggle[0].isOn = true;
            }
        }
        if (players2[0].pos != null && !players2[0].i_am_host)
        {
            if (_aceptar_toggle[1].isOn)
            {
                _aceptar_toggle[0].isOn = true;
                players2[0].pos = _spawnTransforms[0];
            }
            else
            {
                players2[1].controlEnabled = false;

                _aceptar_toggle[1].isOn = true;
            }
        }
    }

    public void Ready()
    {
        if (!players2[index].i_am_host)
        {
            players2[index].pos = _spawnTransforms[0];
            players2[index].controlEnabled = false;
        }
        if (players2[index].i_am_host)
        {
            players2[index].pos = _spawnTransforms[1];
            players2[index].controlEnabled = false;
        }
        index = index < 2 ? index++ : index;
    }

    void Desactivar() => _aceptar.interactable = false;
}
