using UnityEngine;

public class SwitchCanvas : MonoBehaviour
{
    [SerializeField] private Canvas[] _canvasList;
    private int _isActive = 0;

    public void SwitchCanvasById(int id)
    {
        _canvasList[_isActive].gameObject.SetActive(false);
        _canvasList[id].gameObject.SetActive(true);
        _isActive = id;
    }
}
