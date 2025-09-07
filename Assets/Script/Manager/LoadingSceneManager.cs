using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TypeGame
{
    PvP,
    PvE
}

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Canvas _loadingCanvas;
    [SerializeField] private Animator _animator;

    private Coroutine _loadSceneCoroutine;
    private AsyncOperation _operation = null;
    private bool _isLoading = false;
    private bool _typeGame;

    public bool TypeGame => _typeGame;

    public static LoadingSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string SceneName)
    {
        if (_isLoading) return;
        if (SceneManager.GetSceneByName(SceneName).isLoaded) return;
        if (SceneName == "PvPGameScene") _typeGame = false;
        else if (SceneName == "PvEGameScene") _typeGame = true;
        if (_loadSceneCoroutine != null) StopCoroutine(_loadSceneCoroutine);
        _loadSceneCoroutine = StartCoroutine(LoadSceneAsync(SceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _isLoading = true;
        _operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (_operation == null)
        {
            _isLoading = false;
            yield break;
        }

        _operation.allowSceneActivation = false;
        _loadingCanvas.gameObject.SetActive(true);
        _animator.SetBool("IsLoading", true);

        while (_operation.progress < 0.9f) yield return null;

        yield return new WaitForSeconds(1f);

        _animator.SetBool("IsLoading", false);

        yield return null;

        _operation.allowSceneActivation = true;

        while (!_operation.isDone) yield return null;

        _loadingCanvas.gameObject.SetActive(false);

        _operation = null;
        _isLoading = false;
        _loadSceneCoroutine = null;
    }
}
