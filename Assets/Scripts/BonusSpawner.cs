using System.Collections;
using System.Collections.Generic; // Wymagane do działania List<>
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    [Header("Prefaby")]
    public GameObject ufoPrefab;
    public GameObject bonusPickupPrefab;

    [Header("Ustawienia spawnu")]
    public float minSpawnInterval = 8f;
    public float maxSpawnInterval = 15f;
    public int maxBonusesOnMap = 3;

    [Header("Granice mapy (dopasuj do swojej areny)")]
    public Vector2 mapMin = new Vector2(-20f, -10f);
    public Vector2 mapMax = new Vector2(20f, 10f);

    private int currentBonusCount = 0;

    // Które bonusy trafiają do przeciwnika (nie do zbierającego)
    private BonusType[] enemyTargetBonuses = new BonusType[]
    {
        BonusType.SlowEnemy,
        BonusType.NegativeRandom,
        BonusType.AirStrike
    };

    void Start()
    {
        // Pobranie wartości ze statycznej instancji opcji gry na początku rundy
        if (GameOptions.Instance != null)
        {
            minSpawnInterval = GameOptions.Instance.minSpawnInterval;
            maxSpawnInterval = GameOptions.Instance.maxSpawnInterval;
            maxBonusesOnMap  = GameOptions.Instance.maxBonusesOnMap;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Dynamicznie korzystamy z przedziałów czasowych przypisanych w Start
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Dynamicznie sprawdzamy limit maksymalnej ilości bonusów na mapie
            if (currentBonusCount < maxBonusesOnMap)
                StartCoroutine(SpawnWithUFO());
        }
    }

    private IEnumerator SpawnWithUFO()
    {
        // 1. FILTROWANIE DOZWOLONYCH BONUSÓW
        System.Array allBonusTypes = System.Enum.GetValues(typeof(BonusType));
        List<BonusType> allowedBonuses = new List<BonusType>();

        foreach (BonusType type in allBonusTypes)
        {
            // Pomijamy specyficzny techniczny stan techniczny "None" lub "AirStrike" (jeśli ma osobną logikę)
            if (type == BonusType.AirStrike) // Jeśli AirStrike ma się normalnie spawnować, zakomentuj tę linię
                continue;

            // Sprawdzamy czy bonus nie jest na liście zablokowanych w GameOptions
            if (GameOptions.Instance != null)
            {
                if (GameOptions.Instance.IsBonusEnabled(type))
                {
                    allowedBonuses.Add(type);
                }
            }
            else
            {
                allowedBonuses.Add(type); // Fallback jeśli nie odpalamy z poziomu menu
            }
        }

        // Jeśli gracz wyłączył ABSOLUTNIE WSZYSTKIE bonusy w menu, UFO nie przyleci
        if (allowedBonuses.Count == 0) yield break;

        // Losowe miejsce na mapie
        Vector2 spawnPos = new Vector2(
            Random.Range(mapMin.x, mapMax.x),
            Random.Range(mapMin.y, mapMax.y)
        );

        // UFO przylatuje z góry ekranu
        Vector3 ufoStartPos = new Vector3(spawnPos.x, mapMax.y + 3f, 0f);
        Vector3 ufoTargetPos = new Vector3(spawnPos.x, spawnPos.y + 2f, 0f);

        GameObject ufo = null;
        Animator ufoAnimator = null;

        if (ufoPrefab != null)
        {
            ufo = Instantiate(ufoPrefab, ufoStartPos, Quaternion.identity);
            ufoAnimator = ufo.GetComponent<Animator>();

            // Animacja lotu UFO w dół
            float elapsed = 0f;
            float flyDuration = 1.5f;
            while (elapsed < flyDuration)
            {
                elapsed += Time.deltaTime;
                ufo.transform.position = Vector3.Lerp(ufoStartPos, ufoTargetPos, elapsed / flyDuration);
                yield return null;
            }
        }

        // ================================================================
        // TUTAJ UFO DOLECIAŁO NA DÓŁ - ODPALAMY LASER
        // ================================================================
        if (ufoAnimator != null)
        {
            ufoAnimator.SetTrigger("Shoot");
        }

        yield return new WaitForSeconds(0.6f);

        // 2. LOSOWANIE TYLKO Z DOZWOLONYCH TYPÓW
        BonusType randomType = allowedBonuses[Random.Range(0, allowedBonuses.Count)];

        // Stwórz pickup
        if (bonusPickupPrefab != null)
        {
            GameObject pickup = Instantiate(bonusPickupPrefab, spawnPos, Quaternion.identity);
            PickupBonus pb = pickup.GetComponent<PickupBonus>();
            if (pb != null)
            {
                pb.bonusType = randomType;
                pb.appliedToEnemy = System.Array.Exists(enemyTargetBonuses, t => t == randomType);
            }
            currentBonusCount++;

            // Zniszcz pickup po 20s jeśli nikt nie zbierze
            Destroy(pickup, 20f);
            StartCoroutine(DecrementCountAfter(20f));
        }

        yield return new WaitForSeconds(0.2f);

        // UFO odlatuje w górę
        if (ufo != null)
        {
            float elapsed = 0f;
            float flyDuration = 1f;
            Vector3 exitPos = new Vector3(spawnPos.x, mapMax.y + 5f, 0f);
            while (elapsed < flyDuration)
            {
                elapsed += Time.deltaTime;
                ufo.transform.position = Vector3.Lerp(ufoTargetPos, exitPos, elapsed / flyDuration);
                yield return null;
            }
            Destroy(ufo);
        }
    }

    private IEnumerator DecrementCountAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentBonusCount = Mathf.Max(0, currentBonusCount - 1);
    }

    public void OnBonusPickedUp()
    {
        currentBonusCount = Mathf.Max(0, currentBonusCount - 1);
    }
}