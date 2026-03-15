using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] public Transform cardsGrid;
    [SerializeField] public GameObject cardItemPrefab;
    [SerializeField] public Button backButton;
    [SerializeField] private CardDetailsOverlayUI cardDetailsOverlay;
    [SerializeField] private TextMeshProUGUI collectionCount;

    [Header("Filters")]
    [SerializeField] private Button filterAll;
    [SerializeField] private Button filterLevel1;
    [SerializeField] private Button filterLevel2;
    [SerializeField] private Button filterLevel3;
    [SerializeField] private Button filterLevel4;
    [SerializeField] private Button filterLevel5;


    private static readonly Color FilterActiveColor = new Color(0.55f, 0.36f, 0.96f); // #8B5CF6
    private static readonly Color FilterInactiveColor = new Color(0.06f, 0.09f, 0.16f); // #0F1628
    private int activeFilter = 0; // 0 = all, 1-5 = level
    private Card[] allCards;

    private Dictionary<GameObject, Card> cardItemMap = new Dictionary<GameObject, Card>();

    public void Start()
    {
        allCards = CardApiService.Instance.AllCards.OrderBy(c => c.RequiredLevel).ToArray();
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        collectionCount.text = $"{CardApiService.Instance.GetUnlockedCards().Length} / {allCards.Length} cartes débloquées";

        SetupFilters();
        PopulateCollection();
    }

    private void SetupFilters()
    {
        filterAll.onClick.AddListener(() => ApplyFilter(0));
        filterLevel1.onClick.AddListener(() => ApplyFilter(1));
        filterLevel2.onClick.AddListener(() => ApplyFilter(2));
        filterLevel3.onClick.AddListener(() => ApplyFilter(3));
        filterLevel4.onClick.AddListener(() => ApplyFilter(4));
        filterLevel5.onClick.AddListener(() => ApplyFilter(5));

        UpdateFilterVisuals();
    }

    private void ApplyFilter(int filterIndex)
    {
        activeFilter = filterIndex;
        UpdateFilterVisuals();
        UpdateCardVisibility();
    }

    private void UpdateFilterVisuals()
    {
        Button[] filters = { filterAll, filterLevel1, filterLevel2, filterLevel3, filterLevel4, filterLevel5 };
        for (int i = 0; i < filters.Length; i++)
        {
            Image bg = filters[i].GetComponent<Image>();
            TextMeshProUGUI label = filters[i].GetComponentInChildren<TextMeshProUGUI>();
            bool isActive = activeFilter == i;
            if (bg != null) bg.color = isActive ? FilterActiveColor : FilterInactiveColor;
            if (label != null) label.color = isActive ? Color.white : new Color(0.48f, 0.55f, 0.68f);
        }
    }

    private void UpdateCardVisibility()
    {
        foreach (var kvp in cardItemMap)
        {
            GameObject item = kvp.Key;
            Card card = kvp.Value;
            bool visible = activeFilter == 0 || card.RequiredLevel == activeFilter;
            item.SetActive(visible);
        }
    }

    public void PopulateCollection()
    {
        cardItemMap.Clear();
        int playerLevel = PlayerProgressionSystem.Instance.CurrentLevel;

        foreach (var card in allCards)
        {
            GameObject item = Instantiate(cardItemPrefab, cardsGrid);
            cardItemMap[item] = card;

            bool isUnlocked = card.RequiredLevel <= playerLevel;
            Image cardImage = item.GetComponent<Image>();
            Image strip = item.transform.Find("Strip").GetComponent<Image>();
            TextMeshProUGUI nameText = item.transform.Find("CardName").GetComponent<TextMeshProUGUI>();
            Image badgeImage = item.transform.Find("LevelBadge").GetComponent<Image>();
            TextMeshProUGUI badgeText = item.transform.Find("LevelBadge/LevelBadgeText").GetComponent<TextMeshProUGUI>();
            GameObject lockedOverlay = item.transform.Find("LockedOverlay").gameObject;

            Color riskColor = card.RiskLevel switch
            {
                RiskLevel.Low => new Color(0.18f, 0.80f, 0.44f),    // #2ECC70
                RiskLevel.Medium => new Color(0.90f, 0.49f, 0.13f), // #E67E21
                RiskLevel.High => new Color(0.91f, 0.30f, 0.24f),   // #E84C3D
                _ => new Color(0.55f, 0.36f, 0.96f)                  // #8B5CF6
            };

            if (isUnlocked)
            {
                strip.color = riskColor;
                nameText.text = card.DisplayName;
                badgeText.text = $"NIV.{card.RequiredLevel}";
                badgeImage.color = new Color(riskColor.r, riskColor.g, riskColor.b, 0.15f);
                badgeText.color = riskColor;
                lockedOverlay.SetActive(false);

                Outline outline = item.GetComponent<Outline>();
                if (outline != null)
                    outline.effectColor = new Color(riskColor.r, riskColor.g, riskColor.b, 0.35f);

                item.GetComponent<Button>().onClick.AddListener(() => cardDetailsOverlay.Open(card));
            }
            else
            {
                lockedOverlay.SetActive(true);
                nameText.text = "";
                badgeText.text = $"NIV.{card.RequiredLevel}";
                strip.color = new Color(1, 1, 1, 0.05f);
            }
        }
    }
}