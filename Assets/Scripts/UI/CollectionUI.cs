using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CollectionUI : MonoBehaviour
{
    public Transform cardsGrid;
    public GameObject cardItemPrefab;
    public CardData[] allCards;
    public Button backButton;

    public void Start()
    {
        allCards = Resources.LoadAll<CardData>("Cards").OrderBy(c => c.requiredLevel).ToArray();
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        PopulateCollection();
    }

    public void PopulateCollection()
    {
        int playerLevel = PlayerProgressionSystem.Instance.CurrentLevel;

        foreach (var card in allCards)
        {
            GameObject item = Instantiate(cardItemPrefab, cardsGrid);

            TextMeshProUGUI nameText = item.transform.Find("CardName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI levelText = item.transform.Find("RequiredLevel").GetComponent<TextMeshProUGUI>();
            GameObject lockedOverlay = item.transform.Find("LockedOverlay").gameObject;
            TextMeshProUGUI lockText = lockedOverlay.transform.Find("LockText").GetComponent<TextMeshProUGUI>();

            bool isUnlocked = card.requiredLevel <= playerLevel;

            nameText.text = isUnlocked ? card.cardName : "???";
            levelText.text = $"Niveau {card.requiredLevel}";
            lockText.text = $"Niveau {card.requiredLevel} requis";
            lockedOverlay.SetActive(!isUnlocked);
        }
    }
}