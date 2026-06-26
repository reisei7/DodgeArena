using UnityEngine;
using TMPro; // Zamień na 'using UnityEngine.UI;' jeśli używasz zwykłego Text

public class BonusUI : MonoBehaviour
{
    [Header("Referencje")]
    public BonusEffect targetTank; // Przeciągnij czołg tego gracza w Inspectorze
    public TextMeshProUGUI uiText; // Przeciągnij komponent tekstowy pod punktami

    void Update()
    {
        if (targetTank == null || uiText == null) return;

        // Pobieramy informacje ze skryptu przymocowanego do czołgu
        if (targetTank.GetActiveBonusInfo(out string name, out float time))
        {
            // F1 oznacza zaokrąglenie do 1 miejsca po przecinku (np. 3.4s)
            uiText.text = $"{name}\nPozostało: {time.ToString("F1")}s";
            uiText.color = Color.yellow; // Wyróżnienie aktywnego efektu
        }
        else
        {
            uiText.text = "Brak bonusu";
            uiText.color = Color.gray;
        }
    }
}