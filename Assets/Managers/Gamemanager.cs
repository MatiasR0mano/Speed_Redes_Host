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
    public static Gamemanager instance { get; private set; }
    [SerializeField] GameObject _menu, _start, _winImage, _loseImage, _mensaje_abandono, one, two, three, _volver_al_tittle;
    [SerializeField] Button _aceptar;
    public List<Button> _cancelar;
    public bool comenzar, start_game, teleport;
    int index;
    public Transform[] _spawnTransforms;
    public Transform[] spawn_initial;
    public List<Toggle> _aceptar_toggle;
    public List<Toggle> _irte;


    void Awake()
    {
        instance = this;
        _players = new List<PlayerRef>();
        _aceptar.onClick.AddListener(RPC_Aceptar);
        _aceptar.onClick.AddListener(Desactivar);
        _aceptar.interactable = false;

        //_cancelar[0].onClick.AddListener(RPC_Anular_Partida2);



        _cancelar[0].onClick.AddListener(RPC_mevoy);
        _cancelar[1].onClick.AddListener(Runner_cosa);
        _cancelar[2].onClick.AddListener(Runner_cosa);

    }

    private void Update()
    {
        if (comenzar)
        {
            if (_players[0] == null || _players[1] == null)
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
        if (_irte[0].isOn || _irte[1].isOn)
        {
            RPC_Anular_Partida2();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_aceptar_toggle[0].isOn && _aceptar_toggle[1].isOn && !teleport)
        {
            RPC_Start();
            for (int i = 0; i < players2.Count; i++) players2[i].TeleportPlayer();
            teleport = true;
        }

    }



    public void RPC_AddToList(NetworkPlayer player)
    {
        var playerRef = player.Object.StateAuthority;
        _players.Add(playerRef);
        players2.Add(player);
        if (players2.Count == 2) _aceptar.interactable = true;
        else _aceptar.interactable = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Defeat(PlayerRef player, NetworkPlayer pla)
    {
        _menu.SetActive(true);
        if (players2[0].i_am_host && players2[1].ganador)
        {
            Debug.Log("perdio el host");
            Defeat();
        }
        if (!players2[0].i_am_host && players2[1].ganador)
        {
            Debug.Log("perdio el host");
            Defeat();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Win([RpcTarget] PlayerRef player, NetworkPlayer pla)
    {
        _menu.SetActive(true);
        if (players2[0].i_am_host && players2[0].ganador) Winner();
        if (!players2[0].i_am_host && players2[0].ganador) Winner();
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Start()
    {
        _start.SetActive(false);
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
            if (_aceptar_toggle[0].isOn) _aceptar_toggle[1].isOn = true;
            else
            {
                players2[1].controlEnabled = false;
                _aceptar_toggle[0].isOn = true;
            }
        }
        if (players2[0].pos != null && !players2[0].i_am_host)
        {
            if (_aceptar_toggle[1].isOn) _aceptar_toggle[0].isOn = true;
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
            players2[1].pos = _spawnTransforms[1];
            players2[index].controlEnabled = false;
        }
        if (players2[index].i_am_host)
        {
            players2[index].pos = _spawnTransforms[1];
            players2[1].pos = _spawnTransforms[0];
            players2[index].controlEnabled = false;
        }
        index = index < 2 ? index++ : index;
    }

    void Desactivar() => _aceptar.interactable = false;


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_mevoy()
    {
        if (players2[0].i_am_host)
        {
            if (_irte[0].isOn) _irte[1].isOn = true;
            else
            {
                RPC_Anular_Partida2();
                _irte[0].isOn = true;
            }
        }
        if (!players2[0].i_am_host)
        {
            if (_irte[1].isOn) _irte[0].isOn = true;
            else
            {
                RPC_Anular_Partida2();
                _irte[1].isOn = true;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Anular_Partida2()
    {
        _volver_al_tittle.SetActive(true);

    }

    public void Runner_cosa()
    {
        players2[0].Runner.Shutdown();
    }

}
