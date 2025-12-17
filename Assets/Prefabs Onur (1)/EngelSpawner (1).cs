using UnityEngine;

public class EngelSpawner : MonoBehaviour
{
    public GameObject engelPrefab;
    public float cikmaSikligi = 2.0f;

    [Header("Auto Stop")]
    public float kacSaniyeSpawnEtsin = 20f; // kaç saniye sonra dursun?

    void Start()
    {
        InvokeRepeating(nameof(EngelYarat), 1.0f, cikmaSikligi);

        // X saniye sonra spawner'ý durdur
        Invoke(nameof(SpawneriDurdur), kacSaniyeSpawnEtsin);
    }

    void EngelYarat()
    {
        Instantiate(engelPrefab, transform.position, transform.rotation);
    }

    void SpawneriDurdur()
    {
        CancelInvoke(nameof(EngelYarat)); // sadece spawn'u durdurur
        // enabled = false; // istersen scripti de kapatabilirsin
    }
}
