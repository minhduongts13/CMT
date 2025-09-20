using UnityEngine;

public enum InGameCanvas
{
    GameCanvas,
    MenuCanvas,
    GameOverCanvas,
}

public class SwitchCanvas : MonoBehaviour
{
    [SerializeField] private Canvas[] _canvasList;
    private int _isActive = 0;

    public static SwitchCanvas Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SwitchCanvasById(int id)
    {
        _canvasList[_isActive].gameObject.SetActive(false);
        _canvasList[id].gameObject.SetActive(true);
        _isActive = id;
    }
}
