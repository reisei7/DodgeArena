using UnityEngine;

public enum BulletType { Explosive, Ricochet, Tracker }

public class Bullet : MonoBehaviour
{
    [Header("Typ pocisku")]
    public BulletType bulletType = BulletType.Explosive;
    public float speed = 10f;
    public float lifetime = 5f;

    [Header("Ricochet")]
    public int maxBounces = 2;

    [Header("Tracker")]
    public float homingStrength = 5f;
    public float homingRange = 8f;
    private float retargetTimer = 0f;
    private const float retargetInterval = 0.3f; // szuka celu co 0.3s, nie tylko raz

    [Header("Explosive")]
    public float explosionRadius = 2.5f;
    public GameObject explosionPrefab;
    public LayerMask playerLayerMask; // ustaw w Inspectorze na warstwę graczy

    [Header("Efekty")]
    public GameObject impactPrefab;
    public Animator animator;
    public GameObject bounceEffectPrefab;
    private Vector2 direction;
    private int ownerPlayerNumber;
    private int bounceCount = 0;
    private Transform target;
    private bool isDying = false;

    public void Initialize(Vector2 dir, int owner)
    {
        direction = dir.normalized;
        ownerPlayerNumber = owner;

        if (GameOptions.Instance != null)
        {
            switch (bulletType)
            {
                case BulletType.Explosive:
                    explosionRadius = GameOptions.Instance.explosiveRadius;
                    break;
                case BulletType.Ricochet:
                    maxBounces = GameOptions.Instance.ricochetBounces;
                    break;
                case BulletType.Tracker:
                    homingStrength = GameOptions.Instance.trackerHomingStrength;
                    break;
            }
        }

        Destroy(gameObject, lifetime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        if (isDying) return;

        if (bulletType == BulletType.Tracker)
        {
            retargetTimer -= Time.deltaTime;
            if (retargetTimer <= 0f)
            {
                FindTarget();
                retargetTimer = retargetInterval;
            }

            if (target != null)
                HomingUpdate();
        }

        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void HomingUpdate()
    {
        if (target == null) return;

        Vector2 toTarget = (Vector2)(target.position - transform.position);

        // Jeśli cel wyszedł poza zasięg, przestań go śledzić
        if (toTarget.magnitude > homingRange * 1.5f)
        {
            target = null;
            return;
        }

        direction = Vector2.Lerp(direction.normalized, toTarget.normalized, homingStrength * Time.deltaTime);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float closestDist = homingRange;

        foreach (var p in players)
        {
            PlayerScore ps = p.GetComponent<PlayerScore>();
            if (ps == null || ps.playerNumber == ownerPlayerNumber) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = p.transform;
            }
        }

        target = closest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;

        if (other.CompareTag("Wall"))
        {
            if (bulletType == BulletType.Ricochet && bounceCount < maxBounces)
            {
                Bounce(other);
                return;
            }
            HandleHit(transform.position);
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerScore ps = other.GetComponent<PlayerScore>();
            if (ps == null) return;

            // Nigdy nie trafiaj własnego strzelca bezpośrednim trafieniem
            if (ps.playerNumber == ownerPlayerNumber) return;

            if (bulletType == BulletType.Explosive)
            {
                // Explosive nie liczy punktu od razu - eksplozja zrobi to obszarowo
                HandleHit(transform.position);
            }
            else
            {
                GameManager.Instance.AddPoint(ownerPlayerNumber);
                HandleHit(transform.position);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayHit();
            }
        }
    }

    private void Bounce(Collider2D wall)
    {
        bounceCount++;

        // Prawdziwa normalna kolizji - znajdź najbliższy punkt na ścianie i policz wektor odbicia
        Vector2 closestPoint = wall.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)transform.position - closestPoint).normalized;

        // Zabezpieczenie - jeśli pocisk jest dokładnie na krawędzi, normal może wyjść zerowy
        if (normal == Vector2.zero)
            normal = -direction;

        direction = Vector2.Reflect(direction, normal).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Odsuń pocisk minimalnie od ściany żeby nie wykrył tej samej kolizji w następnej klatce
        transform.position = (Vector2)transform.position + normal * 0.1f;

        if (animator) animator.SetTrigger("Bounce");
    }

    private void HandleHit(Vector2 hitPosition)
    {
        if (isDying) return;
        isDying = true;

        if (bulletType == BulletType.Explosive)
        {
            Explode(hitPosition);
        }
        else if (impactPrefab != null)
        {
            Instantiate(impactPrefab, hitPosition, Quaternion.identity);
        }

        if (animator)
        {
            animator.SetTrigger("Hit");
            // Daj animacji chwilę zanim zniknie obiekt
            GetComponent<SpriteRenderer>().enabled = false;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
            Destroy(gameObject, 0.3f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

        private void Explode(Vector2 center)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, center, Quaternion.identity);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayExplosion();

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius);
        bool hitEnemy = false;
        bool hitSelf = false;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerScore ps = hit.GetComponent<PlayerScore>();
            if (ps == null) continue;

            if (ps.playerNumber == ownerPlayerNumber)
            {
                hitSelf = true;
            }
            else
            {
                hitEnemy = true;
            }
        }

        // Punkt tylko za trafienie przeciwnika, raz na eksplozję
        if (hitEnemy)
            GameManager.Instance.AddPoint(ownerPlayerNumber);

        if (hitSelf && GameManager.Instance != null)
        {
            int otherPlayer = ownerPlayerNumber == 1 ? 2 : 1;
            GameManager.Instance.AddPoint(otherPlayer);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (bulletType == BulletType.Explosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
        if (bulletType == BulletType.Tracker)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, homingRange);
        }
    }
}