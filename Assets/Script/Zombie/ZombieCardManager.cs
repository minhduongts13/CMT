using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZombieCardManager : MonoBehaviour
{
    [Header("Cards Parameters")]
    public int amOfCards;
    public ZombieCardScriptableObject[] zombieCardSO;
    public GameObject CardPrefab;
    public Transform cardHolderTransform;

    [Header("Zombie Parameters")]
    public GameObject[] zombieCards;

    void Start()
    {
        amOfCards = zombieCardSO.Length;
        zombieCards = new GameObject[amOfCards];
        for (int i = 0; i < amOfCards; i++)
        {
            AddZombieCard(i);
        }
    }

    public void AddZombieCard(int index)
    {
        GameObject newCard = Instantiate(CardPrefab, cardHolderTransform);
        Debug.Log("Instantiated card prefab at index: " + index);
        zombieCards[index] = newCard;

        newCard.transform.Find("Zombie Icon").GetComponent<Image>().sprite = zombieCardSO[index].zombieIcon;
        newCard.transform.Find("Zombie Cost").GetComponentInChildren<TMP_Text>().text = "" + zombieCardSO[index].cost;

        ZombieCardController cardController = newCard.GetComponent<ZombieCardController>();
        if (cardController != null)
        {
            cardController.myZombieCardSO = zombieCardSO[index];
        }
    }
}