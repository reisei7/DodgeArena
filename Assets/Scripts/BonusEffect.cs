using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusEffect : MonoBehaviour
{
    // Aktywne efekty z czasem wygaśnięcia
    private Dictionary<BonusType, Coroutine> activeEffects = new Dictionary<BonusType, Coroutine>();

    // Stan efektów
    public bool hasShield = false;
    public bool isSlowed = false;
    public bool isSpeedBoosted = false;
    public bool bulletsThroughWalls = false;
    public bool controlsReversed = false;
    public bool noRotation = false;
    public bool forcedForward = false;
    public bool isStunned = false;

    private PlayerMovement movement;
    private PlayerShoot shoot;
    private float originalSpeed;
    private float originalRotateSpeed;
    
    // Słownik przechowujący informację ile sekund zostało dla danego bonusu
    private Dictionary<BonusType, float> timers = new Dictionary<BonusType, float>();

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        shoot = GetComponent<PlayerShoot>();
    }

    void Update()
    {
        // Lista pomocnicza do bezpiecznej modyfikacji słownika podczas pętli
        List<BonusType> keys = new List<BonusType>(timers.Keys);
        foreach (BonusType key in keys)
        {
            if (timers[key] > 0f)
            {
                timers[key] -= Time.deltaTime;
            }
        }
    }

    // POPRAWNA KORUTYNA Z ODPOWIEDZIALNOŚCIĄ ZA LICZNIKI
    private IEnumerator TimedEffect(IEnumerator applyEffect, float duration, BonusType type)
    {
        yield return StartCoroutine(applyEffect);
        timers[type] = duration; // <-- USTAWIANIE CZASU STARTOWEGO

        yield return new WaitForSeconds(duration);
        
        timers.Remove(type); // <-- CZYSZCZENIE PO ZAKOŃCZENIU
        RemoveEffect(type);
    }

    public void ApplyBonus(BonusType type, int ownerPlayerNumber)
    {
        switch (type)
        {
            case BonusType.Shield:
                StartTimedEffect(BonusType.Shield, ApplyShield(), 5f);
                break;
            case BonusType.SlowEnemy:
                StartTimedEffect(BonusType.SlowEnemy, ApplySlow(), 5f);
                break;
            case BonusType.SpeedBoost:
                StartTimedEffect(BonusType.SpeedBoost, ApplySpeedBoost(), 5f);
                break;
            case BonusType.BulletsThroughWalls:
                StartTimedEffect(BonusType.BulletsThroughWalls, ApplyBulletsThroughWalls(), 5f);
                break;
            case BonusType.NegativeRandom:
                ApplyRandomNegative();
                break;
            case BonusType.AirStrike:
                // Obsługiwane przez AirStrikeManager
                break;
        }
    }

    private void StartTimedEffect(BonusType type, IEnumerator effect, float duration)
    {
        if (activeEffects.ContainsKey(type) && activeEffects[type] != null)
            StopCoroutine(activeEffects[type]);
        activeEffects[type] = StartCoroutine(TimedEffect(effect, duration, type));
    }

    // ── SHIELD ──────────────────────────────────────────
    private IEnumerator ApplyShield()
    {
        hasShield = true;
        // Wizualnie — np. zmień kolor sprite'a na niebieski
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(0.3f, 0.6f, 1f);
        yield break;
    }

    private void RemoveShield()
    {
        hasShield = false;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;
    }

    // ── SLOW ────────────────────────────────────────────
    private IEnumerator ApplySlow()
    {
        isSlowed = true;
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            originalRotateSpeed = movement.rotateSpeed;
            movement.moveSpeed *= 0.4f;
            movement.rotateSpeed *= 0.4f;
        }
        // Wizualnie — fioletowy kolor
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(0.7f, 0.3f, 1f);
        yield break;
    }

    private void RemoveSlow()
    {
        isSlowed = false;
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
            movement.rotateSpeed = originalRotateSpeed;
        }
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;
    }

    // ── SPEED BOOST ─────────────────────────────────────
    private IEnumerator ApplySpeedBoost()
    {
        isSpeedBoosted = true;
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            movement.moveSpeed *= 2f;
        }
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 0.8f, 0f);
        yield break;
    }

    private void RemoveSpeedBoost()
    {
        isSpeedBoosted = false;
        if (movement != null)
            movement.moveSpeed = originalSpeed;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;
    }

    // ── BULLETS THROUGH WALLS ────────────────────────────
    private IEnumerator ApplyBulletsThroughWalls()
    {
        bulletsThroughWalls = true;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(0f, 1f, 0.8f);
        yield break;
    }

    private void RemoveBulletsThroughWalls()
    {
        bulletsThroughWalls = false;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;
    }

    // ── NEGATIVE RANDOM ──────────────────────────────────
    private void ApplyRandomNegative()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                StartTimedEffect(BonusType.NegativeRandom, ApplyControlsReversed(), 5f);
                break;
            case 1:
                StartTimedEffect(BonusType.NegativeRandom, ApplyNoRotation(), 5f);
                break;
            case 2:
                StartTimedEffect(BonusType.NegativeRandom, ApplyForcedForward(), 5f);
                break;
        }
    }

    private IEnumerator ApplyControlsReversed()
    {
        controlsReversed = true;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 0.3f, 0.3f);
        yield break;
    }

    private IEnumerator ApplyNoRotation()
    {
        noRotation = true;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 0.5f, 0f);
        yield break;
    }

    private IEnumerator ApplyForcedForward()
    {
        forcedForward = true;
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 0.2f, 0.6f);
        yield break;
    }

    // ── STUN (z nalotu) ──────────────────────────────────
    public void ApplyStun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            originalRotateSpeed = movement.rotateSpeed;
            movement.moveSpeed = 0f;
            movement.rotateSpeed = 0f;
        }
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 1f, 0f); // żółty = ogłuszenie
        yield return new WaitForSeconds(duration);
        isStunned = false;
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
            movement.rotateSpeed = originalRotateSpeed;
        }
        if (sr) sr.color = Color.white;
    }

    // ── USUWANIE EFEKTÓW ─────────────────────────────────
    private void RemoveEffect(BonusType type)
    {
        switch (type)
        {
            case BonusType.Shield: RemoveShield(); break;
            case BonusType.SlowEnemy: RemoveSlow(); break;
            case BonusType.SpeedBoost: RemoveSpeedBoost(); break;
            case BonusType.BulletsThroughWalls: RemoveBulletsThroughWalls(); break;
            case BonusType.NegativeRandom:
                controlsReversed = false;
                noRotation = false;
                forcedForward = false;
                var sr = GetComponent<SpriteRenderer>();
                if (sr) sr.color = Color.white;
                break;
        }
        if (activeEffects.ContainsKey(type))
            activeEffects.Remove(type);
    }

    private string GetReadableName(BonusType type)
    {
        switch (type)
        {
            case BonusType.Shield: return "Tarcza ochronna";
            case BonusType.SlowEnemy: return "Spowolnienie";
            case BonusType.SpeedBoost: return "Super prędkość";
            case BonusType.BulletsThroughWalls: return "Przenikanie pocisków";
            case BonusType.NegativeRandom:
                if (controlsReversed) return "Odwrócone sterowanie";
                if (noRotation) return "Blokada obrotu";
                if (forcedForward) return "Przymusowy ruch";
                return "Pechowy los";
            default: return "Aktywny bonus";
        }
    }

    public bool GetActiveBonusInfo(out string bonusName, out float timeLeft)
    {
        bonusName = "";
        timeLeft = 0f;

        foreach (var kvp in timers)
        {
            if (kvp.Key == BonusType.AirStrike) continue;

            timeLeft = kvp.Value;
            bonusName = GetReadableName(kvp.Key);
            return true; // Znaleziono aktywny bonus
        }

        return false; // Brak bonusów
    }
}