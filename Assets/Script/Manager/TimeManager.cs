using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Render")]
    [SerializeField] private TMP_Text _timerLabel;
    [Header("Time")]
    [SerializeField] private float _duration;
    private bool _isPaused = false;
    private bool _hasEnded = false;
    private float _elapsed;
    private int[] _gachaTime = new int[3] { 540, 360, 180 };
    private int totalSeconds = 0;
    private bool[] _gachaTriggered = new bool[3] { false, false, false };

    public bool Pause
    {
        get { return _isPaused; }
        set { _isPaused = value; }
    }

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

        if (!_isPaused) _elapsed += Time.deltaTime;
        float remaining = Mathf.Max(0, _duration - _elapsed);

        totalSeconds = Mathf.CeilToInt(remaining);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (_timerLabel)
        {
            _timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        for (int i = 0; i < 3; i++)
        {
            if (totalSeconds == _gachaTime[i] && !_gachaTriggered[i])
            {
                _gachaTriggered[i] = true;
                _isPaused = true;
                SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GachaCanvas);
                GachaManager.Instance.GachaBuff();
            }
        }

        if (remaining <= 0f)
        {
            _hasEnded = true;
            SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameOverCanvas);
        }
    }
}
