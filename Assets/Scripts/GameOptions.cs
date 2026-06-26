using UnityEngine;
using System.Collections.Generic;

public class GameOptions : MonoBehaviour
{
    public static GameOptions Instance;

    [Header("Rozgrywka")]
    public int scoreToWin = 5;
    public int roundDuration = 180;

    [Header("Czołg & Dash")]
    public float tankSpeed = 1f;
    public float tankRotateSpeed = 120f;
    public float dashCooldown = 1.5f;     // NOWE
    public float dashForce = 15f;          // NOWE
    public float dashDuration = 0.15f;     // NOWE

    [Header("Bonus Spawner")]
    public float minSpawnInterval = 5f;    // NOWE
    public float maxSpawnInterval = 15f;   // NOWE
    public int maxBonusesOnMap = 3;        // NOWE

    [Header("Lista Wyłączonych Bonusów")]
    // Lista przechowuje bonusy, które gracz WYŁĄCZYŁ w menu opcji
    public List<BonusType> disabledBonuses = new List<BonusType>(); // NOWE

    [Header("Pocisk — Explosive")]
    public float explosiveSpeed = 3f;
    public float explosiveCooldown = 1.5f;
    public float explosiveRadius = 3.5f;

    [Header("Pocisk — Ricochet")]
    public float ricochetSpeed = 14f;
    public float ricochetCooldown = 0.8f;
    public int ricochetBounces = 3;

    [Header("Pocisk — Tracker")]
    public float trackerSpeed = 8f;
    public float trackerCooldown = 0.4f;
    public float trackerHomingStrength = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Pomocnicze metody do sprawdzania stanu bonusa (użyje ich Spawner i Dropdown w UI)
    public bool IsBonusEnabled(BonusType type)
    {
        return !disabledBonuses.Contains(type);
    }

    public void SetBonusState(BonusType type, bool isEnabled)
    {
        if (isEnabled)
        {
            if (disabledBonuses.Contains(type)) 
                disabledBonuses.Remove(type);
        }
        else
        {
            if (!disabledBonuses.Contains(type)) 
                disabledBonuses.Add(type);
        }
    }

    public void ResetToDefaults()
    {
        scoreToWin = 10;
        roundDuration = 180;
        
        tankSpeed = 1f;
        tankRotateSpeed = 120f;
        
        // Reset NOWYCH wartości dasha
        dashCooldown = 3f;
        dashForce = 18f;
        dashDuration = 0.30f;

        // Reset NOWYCH wartości spawnera
        minSpawnInterval = 5f;
        maxSpawnInterval = 15f;
        maxBonusesOnMap = 4;

        // Domyslnie wszystkie bonusy są włączone (czysta lista zablokowanych)
        disabledBonuses.Clear();

        explosiveSpeed = 3f;
        explosiveCooldown = 1.5f;
        explosiveRadius = 3.5f;
        
        ricochetSpeed = 14f;
        ricochetCooldown = 0.8f;
        ricochetBounces = 3;
        
        trackerSpeed = 8f;
        trackerCooldown = 0.4f;
        trackerHomingStrength = 5f;
    }
}