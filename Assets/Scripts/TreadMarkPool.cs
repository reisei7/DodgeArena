using UnityEngine;
using System.Collections.Generic;

public class TreadMarkPool : MonoBehaviour
{
    public static TreadMarkPool Instance;

    [Header("Pool Settings")]
    public GameObject treadMarkPrefab;
    public int poolSize = 150;
    public float markLifetime = 6f;
    public float fadeDuration = 2.5f;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(treadMarkPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

public GameObject GetFromPool(Vector2 position, Quaternion rotation)
{
    GameObject obj;

    if (pool.Count > 0)
    {
        obj = pool.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
    }
    else
    {
        // Pool pusty - NIE twórz nowego obiektu w nieskończoność,
        // tylko pomiń ten ślad (lepsze niż lag)
        return null;
    }

    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
    if (sr != null) sr.color = Color.white;

    StartCoroutine(ReturnToPoolAfterTime(obj));
    return obj;
}

        private System.Collections.IEnumerator ReturnToPoolAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(markLifetime);

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float timer = fadeDuration;
            Color original = sr.color;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                Color c = original;
                c.a = timer / fadeDuration;
                sr.color = c;
                yield return null;
            }
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}