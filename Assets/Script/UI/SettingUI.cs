using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button _mainmenuButton;

    private void OnEnable()
    {
        _mainmenuButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
    }

    private void OnDisable()
    {
        _mainmenuButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
    }
}
