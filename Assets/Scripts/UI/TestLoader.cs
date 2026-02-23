using UnityEngine;

public class TestLoader : MonoBehaviour
{
    public CardData testCard;
    public CardUI cardUI;

    void Start()
    {
        cardUI.DisplayCard(testCard);
    }
}