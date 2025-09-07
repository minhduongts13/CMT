using UnityEngine;
using UnityEngine.UI;

public class SelectDeckUI : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    private void OnEnable()
    {
        if (!LoadingSceneManager.Instance.TypeGame)
        {
            _startButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvPGameScene"));
        }
        else
        {
            _startButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvEGameScene"));
        }
    }

    private void OnDisable()
    {
        if (!LoadingSceneManager.Instance.TypeGame)
        {
            _startButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvPGameScene"));
        }
        else
        {
            _startButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("PvEGameScene"));
        }
    }
}
