using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BrowserHandler : MonoBehaviour
{
    [SerializeField] SessionItem _sessionItemPrefab;

    [SerializeField] NetworkRunnerHandler _networkRunnerHandler;

    [SerializeField] TMP_Text _statusText;

    [SerializeField] VerticalLayoutGroup _verticalLayout;

    private void OnEnable()
    {
        _networkRunnerHandler.OnSessionListUpdate += UpdateList;
    }

    private void OnDisable()
    {
        _networkRunnerHandler.OnSessionListUpdate -= UpdateList;
    }

    void ClearBrowser()
    {
        foreach (Transform session in _verticalLayout.transform)
        {
            Destroy(session.gameObject);
        }

        _statusText.gameObject.SetActive(false);
    }

    private void UpdateList(List<SessionInfo> sessions)
    {
        ClearBrowser();

        if (sessions.Count == 0 ) 
        {
            NoSessionsFound();
            return;
        }

        foreach (SessionInfo session in sessions)
        {
            AddNewSessionToBrowser(session);
        }
    }

    void NoSessionsFound()
    {
        _statusText.text = "No sessions found";
        _statusText.gameObject.SetActive(true);
    }

    void AddNewSessionToBrowser(SessionInfo newSession)
    {
        var sessionItem = Instantiate(_sessionItemPrefab, _verticalLayout.transform);
        sessionItem.Initialize(newSession);
        sessionItem.OnSessionItemClick += JoinSelectedSession;
    }

    void JoinSelectedSession(SessionInfo sessionInfo)
    {
        _networkRunnerHandler.JoinGame(sessionInfo);
    }
}
