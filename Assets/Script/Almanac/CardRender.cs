using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardRender : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _recharge;
    [SerializeField] private List<Image> _materials;

    private string DamageText(float dame)
    {
        if (dame == 0f) return "";
        if (dame >= 1000f) return "massive";
        if (dame >= 500f) return "heavy";
        if (dame >= 100f) return "normal";
        return "weak";
    }

    private string MaterialProductionText(float materialProduction)
    {
        if (materialProduction == 0f) return "";
        if (materialProduction < 1f) return "low";
        if (materialProduction < 2f) return "normal";
        if (materialProduction == 2f) return "double";
        return "bug";
    }

    private string ToughNessText(float toughness)
    {
        if (toughness == 0f) return "";
        if (toughness >= 3000f) return "extremely high";
        if (toughness >= 1000f) return "very high";
        if (toughness >= 100f) return "high";
        if (toughness >= 50f) return "medium";
        return "normal";
    }

    private string RechargeText(float recharge)
    {
        if (recharge == 0f) return "none";
        if (recharge < 1f) return "fast";
        if (recharge < 2f) return "slow";
        return "very slow";
    }

    public void RenderAlmanacCard(Almanac_Object almanacObject)
    {
        _name.text = almanacObject.Name;
        _image.sprite = almanacObject.InGameImage;
        _description.text = HandleDescripttion(almanacObject) + Colored(almanacObject.Description, "6B4400");
        _recharge.text = Colored(RechargeText(almanacObject.Recharge), "FF4444");
        if (almanacObject.MergeMaterial.Count > 0)
        {
            for (int i = 0; i < almanacObject.MergeMaterial.Count; i++)
            {
                SetAlpha(_materials[i], 1f);
                _materials[i].sprite = almanacObject.MergeMaterial[i];
            }
        }
        else
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                SetAlpha(_materials[i], 0f);
            }
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        var c = img.color;
        c.a = alpha;      
        img.color = c; 
    }

    private string HandleDescripttion(Almanac_Object plant)
    {
        string result = "";
        string dame = DamageText(plant.Damage);
        if (dame != "")
        {
            result += $"{Colored("Damage:", "6B4400")} {Colored(dame, "FF4444")}\n";
        }

        string materialProduction = MaterialProductionText(plant.MaterialProduction);
        if (materialProduction != "")
        {
            result += $"{Colored("Sun production:", "6B4400")} {Colored(materialProduction, "FF4444")}\n";
        }

        string toughness = ToughNessText(plant.ToughNess);
        if (toughness != "")
        {
            result += $"{Colored("Toughness:", "6B4400")} {Colored(toughness, "FF4444")}\n";
        }
        if (!string.IsNullOrEmpty(plant.Special))
        {
            result += $"{Colored("Special:", "6B4400")} {Colored(plant.Special, "FF4444")}\n";
        }
        return result;
    }

    string Colored(string text, string hexColor) => $"<b><color=#{hexColor}>{text}</color></b>";
}
