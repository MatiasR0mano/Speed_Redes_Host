using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionItem : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _playersAmount;
    [SerializeField] Button _joinBtn;

    SessionInfo _mySessionInfo;

    public event Action<SessionInfo> OnSessionItemClick;

    private void Awake()
    {
        _joinBtn.onClick.AddListener(OnClick);
    }

    public void Initialize(SessionInfo session)
    {
        _mySessionInfo = session;

        _name.text = session.Name;
        _playersAmount.text = $"{session.PlayerCount}/{session.MaxPlayers}";
        _joinBtn.interactable = session.PlayerCount < session.MaxPlayers;
    }

    void OnClick()
    {
        OnSessionItemClick(_mySessionInfo);
    }
}
