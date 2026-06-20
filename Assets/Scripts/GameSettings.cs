using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    [Header("Wybrane typy pocisków")]
    public BulletType player1BulletType = BulletType.Explosive;
    public BulletType player2BulletType = BulletType.Explosive;

    [Header("Tryb gry")]
    public bool isVsAI = false;
    
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
}