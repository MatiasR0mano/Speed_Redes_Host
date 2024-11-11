using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShowNickname : NetworkBehaviour
{
    [Networked]
    private NetworkString<_16> Nickname { get; set; }

    private ChangeDetector _changeDetector;

    private NicknameItem _nicknameItem;

    public event Action OnDespawn = delegate { };

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        _nicknameItem = NicknamesHandler.Instance.CreateNewNicknameItem(this);
        
        if (HasInputAuthority)
        {
            RPC_SetNickname(PlayerPrefs.GetString("Nickname"));
        }
        else if (!HasStateAuthority)
        {
            UpdateNickname();
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Nickname):
                    UpdateNickname();
                    break;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SetNickname(NetworkString<_16> nick)
    {
        Nickname = nick;
    }
    
    void UpdateNickname()
    {
        _nicknameItem.UpdateText(Nickname.Value);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnDespawn();
    }
}
