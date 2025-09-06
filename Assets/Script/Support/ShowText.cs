using System.Collections;
using TMPro;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    Coroutine _mCoroutine;
    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Solve();
    }

    private void OnDisable()
    {
        if (_mCoroutine != null)
        {
            StopCoroutine(_mCoroutine);
            _mCoroutine = null;
        }
    }

    private void Solve()
    {
        if (_mCoroutine != null) StopCoroutine(_mCoroutine);
        _mCoroutine = StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        SetAlpha(_textMeshPro, 0f);
        yield return new WaitForSeconds(1f);
        SetAlpha(_textMeshPro, 255f);
    }

    private void SetAlpha(TextMeshProUGUI textMeshPro, float alpha)
    {
        Color c = textMeshPro.color;
        c.a = Mathf.Clamp01(alpha);
        textMeshPro.color = c;
    }
}
