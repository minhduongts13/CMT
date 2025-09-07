using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    private void OnEnable()
    {
        _continueButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("SelectDeck"));
    }

    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("SelectDeck"));
    }
}
