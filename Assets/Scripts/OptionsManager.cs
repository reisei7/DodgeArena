using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class OptionsManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject optionsPanel;
    public GameObject mainPanel;

    [Header("Rozgrywka")]
    public TextMeshProUGUI scoreValueText;
    public TextMeshProUGUI timeValueText;
    public TextMeshProUGUI tankSpeedValueText;
    public TextMeshProUGUI rotationSpeedValueText;

    [Header("Dash Settings")]
    public TextMeshProUGUI dashCooldownText;
    public TextMeshProUGUI dashForceText;
    public TextMeshProUGUI dashDurationText;

    [Header("Bonus Spawner")]
    public TextMeshProUGUI minSpawnIntervalText;
    public TextMeshProUGUI maxSpawnIntervalText;
    public TextMeshProUGUI maxBonusesText;

    [Header("Explosive")]
    public TextMeshProUGUI expSpeedText;
    public TextMeshProUGUI expCooldownText;
    public TextMeshProUGUI expRadiusText;

    [Header("Ricochet")]
    public TextMeshProUGUI ricSpeedText;
    public TextMeshProUGUI ricCooldownText;
    public TextMeshProUGUI ricBouncesText;

    [Header("Tracker")]
    public TextMeshProUGUI traSpeedText;
    public TextMeshProUGUI traCooldownText;
    public TextMeshProUGUI traHomingText;

    // Lokalne kopie wartości (zmieniane przez +/−, zapisywane przy Save)
    private int scoreToWin;
    private int roundDuration;
    private float tankSpeed;
    private float rotationSpeed;

    // NOWE: Dash
    private float dashCooldown;
    private float dashForce;
    private float dashDuration;

    // NOWE: Bonus Spawner
    private float minSpawnInterval;
    private float maxSpawnInterval;
    private int maxBonusesOnMap;

    private float expSpeed;
    private float expCooldown;
    private float expRadius;

    private float ricSpeed;
    private float ricCooldown;
    private int ricBounces;

    private float traSpeed;
    private float traCooldown;
    private float traHoming;

    // Kroki zmiany dla każdej wartości
    private const int scoreStep = 1;
    private const int timeStep = 10;
    private const float speedStep = 0.5f;
    private const float rotStep = 10f;
    
    // NOWE: Kroki regulacji
    private const float dashCooldownStep = 0.2f;
    private const float dashForceStep = 1.0f;
    private const float dashDurationStep = 0.05f;
    private const float spawnIntervalStep = 0.5f;
    private const int maxBonusesStep = 1;

    private const float bulletSpeedStep = 0.5f;
    private const float cooldownStep = 0.1f;
    private const float radiusStep = 0.5f;
    private const int bouncesStep = 1;
    private const float homingStep = 0.5f;

    void OnEnable()
    {
        LoadFromGameOptions();
        RefreshUI();
    }

    private void LoadFromGameOptions()
    {
        if (GameOptions.Instance == null) return;

        scoreToWin    = GameOptions.Instance.scoreToWin;
        roundDuration = GameOptions.Instance.roundDuration;
        tankSpeed     = GameOptions.Instance.tankSpeed;
        rotationSpeed = GameOptions.Instance.tankRotateSpeed;

        // NOWE: Pobieranie z GameOptions (Upewnij się, że te zmienne istnieją w GameOptions)
        dashCooldown     = GameOptions.Instance.dashCooldown;
        dashForce        = GameOptions.Instance.dashForce;
        dashDuration     = GameOptions.Instance.dashDuration;
        minSpawnInterval = GameOptions.Instance.minSpawnInterval;
        maxSpawnInterval = GameOptions.Instance.maxSpawnInterval;
        maxBonusesOnMap  = GameOptions.Instance.maxBonusesOnMap;

        expSpeed    = GameOptions.Instance.explosiveSpeed;
        expCooldown = GameOptions.Instance.explosiveCooldown;
        expRadius   = GameOptions.Instance.explosiveRadius;

        ricSpeed    = GameOptions.Instance.ricochetSpeed;
        ricCooldown = GameOptions.Instance.ricochetCooldown;
        ricBounces  = GameOptions.Instance.ricochetBounces;

        traSpeed    = GameOptions.Instance.trackerSpeed;
        traCooldown = GameOptions.Instance.trackerCooldown;
        traHoming   = GameOptions.Instance.trackerHomingStrength;
    }

    private void RefreshUI()
    {
        if (scoreValueText)         scoreValueText.text         = scoreToWin.ToString();
        if (timeValueText)          timeValueText.text          = roundDuration.ToString();
        if (tankSpeedValueText)     tankSpeedValueText.text     = tankSpeed.ToString("F1");
        if (rotationSpeedValueText) rotationSpeedValueText.text = rotationSpeed.ToString("F0");

        // NOWE: Aktualizacja tekstu w UI
        if (dashCooldownText)       dashCooldownText.text       = dashCooldown.ToString("F1") + "s";
        if (dashForceText)          dashForceText.text          = dashForce.ToString("F0");
        if (dashDurationText)       dashDurationText.text       = dashDuration.ToString("F2") + "s";
        if (minSpawnIntervalText)   minSpawnIntervalText.text   = minSpawnInterval.ToString("F1") + "s";
        if (maxSpawnIntervalText)   maxSpawnIntervalText.text   = maxSpawnInterval.ToString("F1") + "s";
        if (maxBonusesText)         maxBonusesText.text         = maxBonusesOnMap.ToString();

        if (expSpeedText)    expSpeedText.text    = expSpeed.ToString("F1");
        if (expCooldownText) expCooldownText.text = expCooldown.ToString("F1");
        if (expRadiusText)   expRadiusText.text   = expRadius.ToString("F1");

        if (ricSpeedText)    ricSpeedText.text    = ricSpeed.ToString("F1");
        if (ricCooldownText) ricCooldownText.text = ricCooldown.ToString("F1");
        if (ricBouncesText)  ricBouncesText.text  = ricBounces.ToString();

        if (traSpeedText)    traSpeedText.text    = traSpeed.ToString("F1");
        if (traCooldownText) traCooldownText.text = traCooldown.ToString("F1");
        if (traHomingText)   traHomingText.text   = traHoming.ToString("F1");
    }

    // ─── Rozgrywka ───────────────────────────────────────────────
    public void ScoreMinus()  { scoreToWin    = Mathf.Max(1, scoreToWin - scoreStep); RefreshUI(); }
    public void ScorePlus()   { scoreToWin    = Mathf.Min(20, scoreToWin + scoreStep); RefreshUI(); }
    public void TimeMinus()   { roundDuration = Mathf.Max(10, roundDuration - timeStep); RefreshUI(); }
    public void TimePlus()    { roundDuration = Mathf.Min(600, roundDuration + timeStep); RefreshUI(); }
    public void TankSpeedMinus()  { tankSpeed = Mathf.Max(1f, tankSpeed - speedStep); RefreshUI(); }
    public void TankSpeedPlus()   { tankSpeed = Mathf.Min(20f, tankSpeed + speedStep); RefreshUI(); }
    public void RotSpeedMinus()   { rotationSpeed = Mathf.Max(10f, rotationSpeed - rotStep); RefreshUI(); }
    public void RotSpeedPlus()    { rotationSpeed = Mathf.Min(360f, rotationSpeed + rotStep); RefreshUI(); }

    // ─── NOWE: Dash Settings ──────────────────────────────────────
    public void DashCooldownMinus() { dashCooldown = Mathf.Max(0.2f, dashCooldown - dashCooldownStep); RefreshUI(); }
    public void DashCooldownPlus()  { dashCooldown = Mathf.Min(10f, dashCooldown + dashCooldownStep); RefreshUI(); }
    public void DashForceMinus()    { dashForce = Mathf.Max(5f, dashForce - dashForceStep); RefreshUI(); }
    public void DashForcePlus()     { dashForce = Mathf.Min(50f, dashForce + dashForceStep); RefreshUI(); }
    public void DashDurationMinus() { dashDuration = Mathf.Max(0.05f, dashDuration - dashDurationStep); RefreshUI(); }
    public void DashDurationPlus()  { dashDuration = Mathf.Min(1f, dashDuration + dashDurationStep); RefreshUI(); }

    // ─── NOWE: Bonus Spawner ──────────────────────────────────────
    public void MinSpawnIntervalMinus() 
    { 
        minSpawnInterval = Mathf.Max(1f, minSpawnInterval - spawnIntervalStep); 
        // Zapobieganie sytuacji, gdzie min jest większe niż max
        if (minSpawnInterval > maxSpawnInterval) maxSpawnInterval = minSpawnInterval;
        RefreshUI(); 
    }
    public void MinSpawnIntervalPlus()  
    { 
        minSpawnInterval = Mathf.Min(30f, minSpawnInterval + spawnIntervalStep); 
        if (minSpawnInterval > maxSpawnInterval) maxSpawnInterval = minSpawnInterval;
        RefreshUI(); 
    }
    public void MaxSpawnIntervalMinus() 
    { 
        maxSpawnInterval = Mathf.Max(1f, maxSpawnInterval - spawnIntervalStep); 
        if (maxSpawnInterval < minSpawnInterval) minSpawnInterval = maxSpawnInterval;
        RefreshUI(); 
    }
    public void MaxSpawnIntervalPlus()  
    { 
        maxSpawnInterval = Mathf.Min(60f, maxSpawnInterval + spawnIntervalStep); 
        if (maxSpawnInterval < minSpawnInterval) minSpawnInterval = maxSpawnInterval;
        RefreshUI(); 
    }
    public void MaxBonusesMinus() { maxBonusesOnMap = Mathf.Max(0, maxBonusesOnMap - maxBonusesStep); RefreshUI(); }
    public void MaxBonusesPlus()  { maxBonusesOnMap = Mathf.Min(10, maxBonusesOnMap + maxBonusesStep); RefreshUI(); }

    // ─── Explosive ───────────────────────────────────────────────
    public void ExpSpeedMinus()    { expSpeed    = Mathf.Max(1f, expSpeed - bulletSpeedStep); RefreshUI(); }
    public void ExpSpeedPlus()     { expSpeed    = Mathf.Min(20f, expSpeed + bulletSpeedStep); RefreshUI(); }
    public void ExpCooldownMinus() { expCooldown = Mathf.Max(0.1f, expCooldown - cooldownStep); RefreshUI(); }
    public void ExpCooldownPlus()  { expCooldown = Mathf.Min(5f, expCooldown + cooldownStep); RefreshUI(); }
    public void ExpRadiusMinus()   { expRadius   = Mathf.Max(0.5f, expRadius - radiusStep); RefreshUI(); }
    public void ExpRadiusPlus()    { expRadius   = Mathf.Min(10f, expRadius + radiusStep); RefreshUI(); }

    // ─── Ricochet ────────────────────────────────────────────────
    public void RicSpeedMinus()    { ricSpeed    = Mathf.Max(1f, ricSpeed - bulletSpeedStep); RefreshUI(); }
    public void RicSpeedPlus()     { ricSpeed    = Mathf.Min(30f, ricSpeed + bulletSpeedStep); RefreshUI(); }
    public void RicCooldownMinus() { ricCooldown = Mathf.Max(0.1f, ricCooldown - cooldownStep); RefreshUI(); }
    public void RicCooldownPlus()  { ricCooldown = Mathf.Min(5f, ricCooldown + cooldownStep); RefreshUI(); }
    public void RicBouncesMinus()  { ricBounces  = Mathf.Max(1, ricBounces - bouncesStep); RefreshUI(); }
    public void RicBouncesPlus()   { ricBounces  = Mathf.Min(10, ricBounces + bouncesStep); RefreshUI(); }

    // ─── Tracker ─────────────────────────────────────────────────
    public void TraSpeedMinus()    { traSpeed    = Mathf.Max(1f, traSpeed - bulletSpeedStep); RefreshUI(); }
    public void TraSpeedPlus()     { traSpeed    = Mathf.Min(20f, traSpeed + bulletSpeedStep); RefreshUI(); }
    public void TraCooldownMinus() { traCooldown = Mathf.Max(0.1f, traCooldown - cooldownStep); RefreshUI(); }
    public void TraCooldownPlus()  { traCooldown = Mathf.Min(5f, traCooldown + cooldownStep); RefreshUI(); }
    public void TraHomingMinus()   { traHoming   = Mathf.Max(0.5f, traHoming - homingStep); RefreshUI(); }
    public void TraHomingPlus()    { traHoming   = Mathf.Min(20f, traHoming + homingStep); RefreshUI(); }

    // ─── Zapisz / Reset ──────────────────────────────────────────
    public void SaveAndBack()
    {
        if (GameOptions.Instance == null) return;

        GameOptions.Instance.scoreToWin       = scoreToWin;
        GameOptions.Instance.roundDuration    = roundDuration;
        GameOptions.Instance.tankSpeed        = tankSpeed;
        GameOptions.Instance.tankRotateSpeed  = rotationSpeed;

        // NOWE: Zapisywanie do GameOptions
        GameOptions.Instance.dashCooldown     = dashCooldown;
        GameOptions.Instance.dashForce        = dashForce;
        GameOptions.Instance.dashDuration     = dashDuration;
        GameOptions.Instance.minSpawnInterval = minSpawnInterval;
        GameOptions.Instance.maxSpawnInterval = maxSpawnInterval;
        GameOptions.Instance.maxBonusesOnMap  = maxBonusesOnMap;

        GameOptions.Instance.explosiveSpeed    = expSpeed;
        GameOptions.Instance.explosiveCooldown = expCooldown;
        GameOptions.Instance.explosiveRadius   = expRadius;

        GameOptions.Instance.ricochetSpeed    = ricSpeed;
        GameOptions.Instance.ricochetCooldown = ricCooldown;
        GameOptions.Instance.ricochetBounces  = ricBounces;

        GameOptions.Instance.trackerSpeed          = traSpeed;
        GameOptions.Instance.trackerCooldown       = traCooldown;
        GameOptions.Instance.trackerHomingStrength = traHoming;

        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ResetToDefaults()
    {
        if (GameOptions.Instance == null) return;
        GameOptions.Instance.ResetToDefaults();
        LoadFromGameOptions();
        RefreshUI();
    }
}