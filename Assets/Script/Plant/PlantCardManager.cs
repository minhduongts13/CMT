using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlantCardManager : MonoBehaviour
{
    [Header("Cards Parameters")]
    public int amOfCards;
    public PlantCardScriptableObject[] plantCardSO;
    public GameObject CardPrefab;
    public Transform cardHolderTransform;

    [Header("Plant Parameters")]
    public GameObject[] plantCards;
    // public int plantCost;
    // public float cooldown;
    // public Sprite plantIcon;
    private void Start()
    {
        amOfCards = plantCardSO.Length;
        plantCards = new GameObject[amOfCards];
        for (int i = 0; i < amOfCards; i++)
        {
            AddPlantCard(i);
        }
    }
    public void AddPlantCard(int index)
    {
        GameObject newCard = Instantiate(CardPrefab, cardHolderTransform);
        plantCards[index] = newCard;


        // plantIcon = plantCardSO[index].plantIcon;
        // plantCost = plantCardSO[index].cost;
        // cooldown = plantCardSO[index].cooldown;


        newCard.transform.Find("Plant Icon").GetComponent<Image>().sprite = plantCardSO[index].CardIcon;
        newCard.transform.Find("Plant Cost").GetComponentInChildren<TMP_Text>().text = "" + plantCardSO[index].Cost;
        PlantCardController cardManager = newCard.GetComponent<PlantCardController>();
        if (cardManager != null)
        {
            cardManager.myPlantCardSO = plantCardSO[index];
        }
    }
}
