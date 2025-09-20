using UnityEngine;
using UnityEngine.UI;

public enum AlmanacCanvas
{
    AlmanacIndexCanvas,
    AlmanacPlantCanvas,
    AlmanacZombieCanvas
}

public class AlmanacUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(() => LoadingSceneManager.Instance.LoadSceneByName("MainMenuScene"));
    }

    public void ViewPlantAlmanac()
    {
        SwitchCanvas.Instance.SwitchCanvasById((int)AlmanacCanvas.AlmanacPlantCanvas);
    }

    public void ViewZombieAlmanac()
    {
        SwitchCanvas.Instance.SwitchCanvasById((int)AlmanacCanvas.AlmanacZombieCanvas);
    }

    public void ViewIndexAlmanac()
    {
        SwitchCanvas.Instance.SwitchCanvasById((int)AlmanacCanvas.AlmanacIndexCanvas);
    }
}
