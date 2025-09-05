using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public void PauseGameFunc()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGameFunc()
    {
        Time.timeScale = 1f;
    }

}
