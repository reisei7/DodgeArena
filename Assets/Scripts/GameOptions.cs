using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public static GameOptions Instance;

    [Header("Rozgrywka")]
    public int scoreToWin = 5;
    public int roundDuration = 120;

    [Header("Czołg")]
    public float tankSpeed = 5f;
    public float tankRotateSpeed = 120f;

    [Header("Pocisk — Explosive")]
    public float explosiveSpeed = 4f;
    public float explosiveCooldown = 1.5f;
    public float explosiveRadius = 2.5f;

    [Header("Pocisk — Ricochet")]
    public float ricochetSpeed = 14f;
    public float ricochetCooldown = 0.8f;
    public int ricochetBounces = 2;

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

    public void ResetToDefaults()
    {
        scoreToWin = 5;
        roundDuration = 120;
        tankSpeed = 5f;
        tankRotateSpeed = 120f;
        explosiveSpeed = 4f;
        explosiveCooldown = 1.5f;
        explosiveRadius = 2.5f;
        ricochetSpeed = 14f;
        ricochetCooldown = 0.8f;
        ricochetBounces = 2;
        trackerSpeed = 8f;
        trackerCooldown = 0.4f;
        trackerHomingStrength = 5f;
    }
}