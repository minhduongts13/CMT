using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Almanac_Object", menuName = "Scriptable Objects/Almanac_Object")]
public class Almanac_Object : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _cardIcon;
    [SerializeField] private float _cost;

    [Header("Combat Stats")]
    [SerializeField] private float _damage;
    [SerializeField] private float _toughness;
    [SerializeField] private float _recharge;
    [SerializeField] private float _range;        // Plant: shoot range, Zombie: attack range
    [SerializeField] private float _actionRate;   // Plant: fire rate, Zombie: attack rate  
    [SerializeField] private float _speed;        // Plant: bullet speed, Zombie: move speed

    [Header("Almanac Specific")]
    [SerializeField] private Sprite _almanacImage;
    [SerializeField] private Sprite _ingameImage;
    [SerializeField] private string _description;
    [SerializeField] private float _materialProduction;
    [SerializeField] private string _special;
    [SerializeField] private List<Sprite> _mergeMaterial;


    // Properties
    public string Name => _name;
    public Sprite CardIcon => _cardIcon;
    public float Cost => _cost;
    public float Damage => _damage;
    public float ToughNess => _toughness;
    public float Recharge => _recharge;
    public float Range => _range;
    public float ActionRate => _actionRate;
    public float Speed => _speed;
    public Sprite AlmanacImage => _almanacImage;
    public Sprite InGameImage => _ingameImage;
    public string Description => _description;
    public float MaterialProduction => _materialProduction;
    public string Special => _special;
    public List<Sprite> MergeMaterial => _mergeMaterial;
}