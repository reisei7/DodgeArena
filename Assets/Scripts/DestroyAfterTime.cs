using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 0.25f;
    private SpriteRenderer sr;
    private float timer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = lifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Lekkie powiększanie + zanikanie
        float t = 1f - (timer / lifetime);
        transform.localScale = Vector3.one * (1f + t * 0.5f);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = timer / lifetime;
            sr.color = c;
        }

        if (timer <= 0f)
            Destroy(gameObject);
    }
}