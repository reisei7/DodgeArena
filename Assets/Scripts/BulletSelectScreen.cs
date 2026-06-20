using UnityEngine;
using UnityEngine.UI;

public class BulletSelectScreen : MonoBehaviour
{
    public BulletCardSelector[] cards;
    public GameObject confirmButton;

    private BulletType? selectedType = null;

    void Awake()
    {
        foreach (var card in cards)
            card.parentScreen = this;
    }

    void OnEnable()
    {
        selectedType = null;
        foreach (var card in cards)
            card.SetSelected(false);

        UpdateConfirmButton();
    }

    public void OnCardClicked(BulletCardSelector clickedCard)
    {
        selectedType = clickedCard.bulletType;

        foreach (var card in cards)
            card.SetSelected(card == clickedCard);

        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        if (confirmButton != null)
            confirmButton.GetComponent<Button>().interactable = selectedType.HasValue;
    }

    public BulletType GetSelectedType()
    {
        return selectedType ?? BulletType.Explosive;
    }
}