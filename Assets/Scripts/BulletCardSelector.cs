using UnityEngine;
using UnityEngine.UI;

public class BulletCardSelector : MonoBehaviour
{
    public BulletType bulletType;
    public GameObject selectedBorder;

    [HideInInspector]
    public BulletSelectScreen parentScreen;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnCardClicked);
    }

    public void SetSelected(bool selected)
    {
        if (selectedBorder != null)
            selectedBorder.SetActive(selected);
    }

    private void OnCardClicked()
    {
        if (parentScreen != null)
            parentScreen.OnCardClicked(this);
    }
}