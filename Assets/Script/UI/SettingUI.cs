using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button _mainmenuButton;
    [SerializeField] private Button _almanacButton;
    [SerializeField] private Button _gameoverButton;
    [SerializeField] private Button _backButton;

    private void OnEnable()
    {
        _mainmenuButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
        _gameoverButton.onClick.AddListener(() => SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameOverCanvas));
        _backButton.onClick.AddListener(() => SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameCanvas));
    }

    private void OnDisable()
    {
        _mainmenuButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
        _gameoverButton.onClick.RemoveListener(() => SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameOverCanvas));
        _backButton.onClick.RemoveListener(() => SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameCanvas));
    }
}
