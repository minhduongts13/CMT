using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _gachaBuffs;
    [SerializeField] private RectTransform _parentContainer;

    [Serializable]
    private class GachaSlot
    {
        public Button reloadButton;
        public Animator reloadAnimator;
        public Vector2 anchorMins;
        public Vector2 anchorMaxs;
    }

    [SerializeField] private List<GachaSlot> _gachaSlots;
    public static GachaManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void ReloadBuff()
    {
        for (int i = 0; i < _gachaSlots.Count; i++)
        {
            SetupReloadButton(i);
        }
    }

    private void SetupReloadButton(int slotIndex)
    {
        _gachaSlots[slotIndex].reloadButton.interactable = true;
        _gachaSlots[slotIndex].reloadButton.onClick.RemoveAllListeners();
        _gachaSlots[slotIndex].reloadButton.onClick.AddListener(() => OnReloadButtonClick(slotIndex));
    }

    private void OnReloadButtonClick(int slotIndex)
    {
        _gachaSlots[slotIndex].reloadButton.interactable = false;
        _gachaSlots[slotIndex].reloadAnimator.SetBool("reload", true);

        int prefabIndex = UnityEngine.Random.Range(0, _gachaBuffs.Count);
        InstantiateAndSetup(prefabIndex, _gachaSlots[slotIndex].anchorMins, _gachaSlots[slotIndex].anchorMaxs);

        StartCoroutine(ResetReloadAnim(_gachaSlots[slotIndex].reloadAnimator, 0.6f));
    }

    private IEnumerator ResetReloadAnim(Animator anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim?.SetBool("reload", false);
    }

    public void GachaBuff()
    {
        int[] indices = GetRandomIndices(_gachaBuffs.Count, 3);

        for (int i = 0; i < indices.Length; i++)
        {
            InstantiateAndSetup(indices[i], _gachaSlots[i].anchorMins, _gachaSlots[i].anchorMaxs);
        }

        ReloadBuff();
    }

    private void InstantiateAndSetup(int prefabIndex, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject prefab = _gachaBuffs[prefabIndex];
        GameObject instance = Instantiate(prefab, _parentContainer, worldPositionStays: false);
        RectTransform rt = instance.GetComponent<RectTransform>();

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Button btn = instance.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnPrefabButtonClicked(prefabIndex));
    }

    private int[] GetRandomIndices(int n, int count)
    {
        if (n <= 0 || count <= 0) return new int[0];

        count = Mathf.Min(n, count);
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = i;

        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        int[] result = new int[count];
        Array.Copy(arr, 0, result, 0, count);

        return result;
    }

    private void OnPrefabButtonClicked(int prefabIndex)
    {
        SwitchCanvas.Instance.SwitchCanvasById((int)InGameCanvas.GameCanvas);
        TimeManager.Instance.Pause = false;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _gachaSlots.Count; i++)
        {
            _gachaSlots[i].reloadButton?.onClick.RemoveAllListeners();
        }
    }
}
