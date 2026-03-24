using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

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

        int level = PlayerProgressionManager.Instance.CurrentLevel;

        collectionCount.text = LocalizationSettings.StringDatabase.GetLocalizedString(
            "UI_Collection", "collection.count",
            new object[] { CardApiService.Instance.GetUnlockedCards(level).Length, allCards.Length }
        );

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
        int playerLevel = PlayerProgressionManager.Instance.CurrentLevel;

        string levelFormat = LocalizationSettings.StringDatabase
            .GetLocalizedString("UI_Cards", "card.label.level");

        foreach (var card in allCards)
        {
            GameObject item = Instantiate(cardItemPrefab, cardsGrid);
            cardItemMap[item] = card;

            bool isUnlocked = card.RequiredLevel <= playerLevel;
            Image strip = item.transform.Find("Strip").GetComponent<Image>();
            TextMeshProUGUI nameText = item.transform.Find("CardName").GetComponent<TextMeshProUGUI>();
            Image badgeImage = item.transform.Find("LevelBadge").GetComponent<Image>();
            TextMeshProUGUI badgeText = item.transform.Find("LevelBadge/LevelBadgeText").GetComponent<TextMeshProUGUI>();
            GameObject lockedOverlay = item.transform.Find("LockedOverlay").gameObject;

            Color riskColorText = ColorUtilities.GetRiskColorText(card.RiskLevel);
            Color riskColorBackground = ColorUtilities.GetRiskColorBackground(card.RiskLevel);

            if (isUnlocked)
            {
                strip.color = riskColorText;
                nameText.text = card.DisplayName;
                badgeText.text = $"{levelFormat}{card.RequiredLevel}";
                badgeImage.color = riskColorBackground;
                badgeText.color = riskColorText;
                lockedOverlay.SetActive(false);

                Outline outline = item.GetComponent<Outline>();
                if (outline != null)
                    outline.effectColor = riskColorText;

                item.GetComponent<Button>().onClick.AddListener(() => cardDetailsOverlay.Open(card));
            }
            else
            {
                lockedOverlay.SetActive(true);
                nameText.text = "";
                badgeText.text = $"{levelFormat}{card.RequiredLevel}";
                strip.color = new Color(1, 1, 1, 0.05f);
            }
        }
    }
}