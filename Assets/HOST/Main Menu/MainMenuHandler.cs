using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkRunnerHandler;

    [Header("Panels")]
    [SerializeField] GameObject _initialPanel;
    [SerializeField] GameObject _statusPanel;
    [SerializeField] GameObject _sessionBrowserPanel;
    [SerializeField] GameObject _hostGamePanel;

    [Header("Buttons")]
    [SerializeField] Button _joinLobbyBtn;
    [SerializeField] Button _goToHostPanelBtn;
    [SerializeField] Button _hostGameBtn;

    [Header("InputField")]
    [SerializeField] TMP_InputField _sessionNameField;
    [SerializeField] TMP_InputField _nicknameField;

    [Header("Texts")]
    [SerializeField] TMP_Text _statusText;

    private void Start()
    {
        _joinLobbyBtn.onClick.AddListener(Btn_JoinLobby);
        _goToHostPanelBtn.onClick.AddListener(Btn_GoToHostPanel);
        _hostGameBtn.onClick.AddListener(Btn_HostGame);

        _networkRunnerHandler.OnJoinedLobby += () =>
        {
            _statusPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };
    }

    void Btn_JoinLobby()
    {
        _initialPanel.SetActive(false);
        _statusPanel.SetActive(true);
        _networkRunnerHandler.JoinLobby();
        PlayerPrefs.SetString("Nickname", _nicknameField.text);
        _statusText.text = "Joining Lobby...";
    }

    void Btn_GoToHostPanel()
    {
        _sessionBrowserPanel.SetActive(false);
        _hostGamePanel.SetActive(true);
    }

    void Btn_HostGame()
    {
        _hostGameBtn.interactable = false;
        _networkRunnerHandler.HostGame(_sessionNameField.text, "Level_1");
    }
}
