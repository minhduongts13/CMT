using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _startPvPButton;
    [SerializeField] private Button _startPvEButton;

    private void OnEnable()
    {
        _startPvPButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvPGameScene"));
        _startPvEButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvEGameScene"));
    }

    private void OnDisable()
    {
        _startPvPButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvPGameScene"));
        _startPvEButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvEGameScene"));
    }
}
