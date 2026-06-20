using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [System.Serializable]
    public class BulletConfig
    {
        public BulletType type;
        public GameObject prefab;
        public float cooldown;
        public float speed;
    }

    [Header("Konfiguracja pocisków (3 typy)")]
    public BulletConfig[] bulletConfigs = new BulletConfig[3];

    [Header("Wybrany typ (ustawiany przed grą)")]
    public BulletType selectedType = BulletType.Explosive;

    public int playerNumber = 1;
    public Transform firePoint;

    [Header("Animacja wystrzału")]
    public Animator muzzleFlashAnimator;

    private float cooldownTimer = 0f;

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            if (playerNumber == 1)
                selectedType = GameSettings.Instance.player1BulletType;
            else if (playerNumber == 2)
                selectedType = GameSettings.Instance.player2BulletType;
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (playerNumber == 1 && keyboard.spaceKey.isPressed)
            TryShoot();

        if (playerNumber == 2 && keyboard.pKey.isPressed)
            TryShoot();
    }

    void TryShoot()
    {
        if (cooldownTimer > 0f) return;

        BulletConfig config = GetConfig(selectedType);
        if (config == null || config.prefab == null) return;

        cooldownTimer = config.cooldown;

        Vector2 shootDir = -transform.up;

        GameObject bulletObj = Instantiate(config.prefab, firePoint.position, Quaternion.identity);
        Bullet b = bulletObj.GetComponent<Bullet>();
        b.speed = config.speed;
        b.Initialize(shootDir, playerNumber);

        PlayMuzzleFlash(selectedType);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }

    private void PlayMuzzleFlash(BulletType type)
    {
        if (muzzleFlashAnimator == null) return;

        switch (type)
        {
            case BulletType.Explosive: muzzleFlashAnimator.SetTrigger("PlayExplosive"); break;
            case BulletType.Ricochet:  muzzleFlashAnimator.SetTrigger("PlayRicochet"); break;
            case BulletType.Tracker:   muzzleFlashAnimator.SetTrigger("PlayTracker"); break;
        }
    }

    private BulletConfig GetConfig(BulletType type)
    {
        foreach (var c in bulletConfigs)
        {
            if (c.type == type) return c;
        }
        return null;
    }
}