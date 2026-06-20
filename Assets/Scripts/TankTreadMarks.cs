using UnityEngine;

public class TankTreadMarks : MonoBehaviour
{
    [Header("Ślady gąsienic")]
    public float spawnInterval = 0.14f;
    public float offsetBehind = 0.65f;

    private float timer = 0f;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (TreadMarkPool.Instance == null)
            Debug.LogWarning("TreadMarkPool nie znaleziony! Dodaj go do sceny.");
    }

    void Update()
    {
        if (playerMovement == null || TreadMarkPool.Instance == null) return;

        bool isMovingForward = playerMovement.GetMoveInput().y > 0.2f || playerMovement.IsDashing();

        if (isMovingForward)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnTreadMark();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    void SpawnTreadMark()
    {
        Vector2 backDir = -transform.up;
        Vector2 spawnPos = (Vector2)transform.position + backDir * offsetBehind;

        TreadMarkPool.Instance.GetFromPool(spawnPos, transform.rotation);
    }
}