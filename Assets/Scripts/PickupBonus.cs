using UnityEngine;

public class PickupBonus : MonoBehaviour
{
    public BonusType bonusType;
    public bool appliedToEnemy = false; // true = efekt idzie do przeciwnika

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerScore ps = other.GetComponent<PlayerScore>();
        if (ps == null) return;

        int collectorNumber = ps.playerNumber;
        int enemyNumber = collectorNumber == 1 ? 2 : 1;

        // Znajdź cel efektu
        int targetNumber = appliedToEnemy ? enemyNumber : collectorNumber;
        GameObject targetPlayer = FindPlayer(targetNumber);

        if (targetPlayer != null)
        {
            BonusEffect be = targetPlayer.GetComponent<BonusEffect>();
            if (be != null)
                be.ApplyBonus(bonusType, collectorNumber);
        }

        // Nalot bombowy — osobna obsługa
        if (bonusType == BonusType.AirStrike)
        {
            AirStrikeManager.Instance?.LaunchAirStrike(collectorNumber, enemyNumber);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUiClick(); // tymczasowo, docelowo osobny dźwięk

        Destroy(gameObject);
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