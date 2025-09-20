using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Almanac_Object", menuName = "Scriptable Objects/Almanac_Object")]
public class Almanac_Object : ScriptableObject
{
    [Header("Deck")]
    [SerializeField] private float _cost;
    [SerializeField] private Sprite _almanacImage;
    [Header("Card")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _ingameImage;
    [SerializeField] private float _recharge;
    [Header("Description")]
    [SerializeField] private string _description;
    [SerializeField] private float _damage;
    [SerializeField] private float _toughness;
    [SerializeField] private float _materialProduction;
    [SerializeField] private string _special;

    public float Cost => _cost;
    public Sprite AlmanacImage => _almanacImage;
    public string Name => _name;
    public Sprite InGameImage => _ingameImage;
    public string Description => _description;
    public float Recharge => _recharge;
    public float Damage => _damage;
    public float ToughNess => _toughness;
    public float MaterialProduction => _materialProduction;
    public string Special => _special;
}
