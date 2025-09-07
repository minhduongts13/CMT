using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Render")]
    [SerializeField] private TMP_Text _timerLabel;
    [Header("Time")]
    [SerializeField] private float _duration;
    private bool _hasEnded = false;
    private float _elapsed;

    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_hasEnded) return;

        _elapsed += Time.deltaTime;
        float remaining = Mathf.Max(0, _duration - _elapsed);

        int totalSeconds = Mathf.CeilToInt(remaining);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (_timerLabel)
        {
            _timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (remaining <= 0f)
        {
            _hasEnded = true;
            SwitchCanvas.Instance.SwitchCanvasById((int)CanvasID.GameOverCanvas);
        }
    }
}
