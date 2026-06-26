using System.Collections;
using UnityEngine;

public class AirStrikeManager : MonoBehaviour
{
    public static AirStrikeManager Instance;

    [Header("Prefaby")]
    public GameObject bombWarningPrefab;  // Okrąg pokazujący gdzie spadnie bomba
    public GameObject bombExplosionPrefab; // Efekt wybuchu

    [Header("Ustawienia")]
    public int bombCount = 3;
    public float bombRadius = 1.5f;       // Promień trafienia
    public float stunDuration = 1.5f;     // Czas ogłuszenia przy bliskiej eksplozji
    public float warningDuration = 2f;    // Ile sekund widać ostrzeżenie przed uderzeniem
    public float spreadRadius = 4f;       // Jak daleko od przeciwnika mogą spaść bomby

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LaunchAirStrike(int attackerNumber, int targetNumber)
    {
        StartCoroutine(AirStrikeCoroutine(attackerNumber, targetNumber));
    }

    private IEnumerator AirStrikeCoroutine(int attackerNumber, int targetNumber)
    {
        // Znajdź pozycję przeciwnika
        GameObject targetPlayer = FindPlayer(targetNumber);
        if (targetPlayer == null) yield break;

        Vector2 targetPos = targetPlayer.transform.position;

        // Wygeneruj pozycje bomb
        Vector2[] bombPositions = new Vector2[bombCount];
        for (int i = 0; i < bombCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spreadRadius;
            bombPositions[i] = targetPos + offset;
        }

        // Pokaż ostrzeżenia (okręgi gdzie spadną bomby)
        GameObject[] warnings = new GameObject[bombCount];
        for (int i = 0; i < bombCount; i++)
        {
            if (bombWarningPrefab != null)
                warnings[i] = Instantiate(bombWarningPrefab, bombPositions[i], Quaternion.identity);
        }

        // Czekaj warningDuration sekund
        yield return new WaitForSeconds(warningDuration);

        // Bomby spadają
        for (int i = 0; i < bombCount; i++)
        {
            // Usuń ostrzeżenie
            if (warnings[i] != null)
                Destroy(warnings[i]);

            // Eksplozja
            if (bombExplosionPrefab != null)
                Instantiate(bombExplosionPrefab, bombPositions[i], Quaternion.identity);

            // Sprawdź czy ktoś jest w zasięgu
            Collider2D[] hits = Physics2D.OverlapCircleAll(bombPositions[i], bombRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;
                PlayerScore ps = hit.GetComponent<PlayerScore>();
                if (ps == null) continue;

                if (ps.playerNumber == targetNumber)
                {
                    // Trafiony przeciwnik — ogłuszenie
                    BonusEffect be = hit.GetComponent<BonusEffect>();
                    if (be != null) be.ApplyStun(stunDuration);
                }
                else if (ps.playerNumber == attackerNumber)
                {
                    // Trafił siebie — krótsze ogłuszenie
                    BonusEffect be = hit.GetComponent<BonusEffect>();
                    if (be != null) be.ApplyStun(stunDuration * 0.5f);
                }
            }

            yield return new WaitForSeconds(0.3f); // Lekkie opóźnienie między bombami
        }
    }

    private GameObject FindPlayer(int playerNumber)
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerScore ps = p.GetComponent<PlayerScore>();
            if (ps != null && ps.playerNumber == playerNumber)
                return p;
        }
        return null;
    }
}