using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        if (scoreValueText)    scoreValueText.text    = scoreToWin.ToString();
        if (timeValueText)     timeValueText.text     = roundDuration.ToString();
        if (tankSpeedValueText) tankSpeedValueText.text = tankSpeed.ToString("F1");
        if (rotationSpeedValueText) rotationSpeedValueText.text = rotationSpeed.ToString("F0");

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