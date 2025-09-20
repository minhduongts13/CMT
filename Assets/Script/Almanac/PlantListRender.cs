using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantListRender : MonoBehaviour
{
    [SerializeField] private Almanac_Object _object;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private Button _button;
    [SerializeField] private AlmanacCardRender _plantCardRender;

    private void Start()
    {
        _image.sprite = _object.AlmanacImage;
        _cost.text = _object.Cost.ToString();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(() => _plantCardRender.RenderAlmanacCard(_object));
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(() => _plantCardRender.RenderAlmanacCard(_object));
    }
}
