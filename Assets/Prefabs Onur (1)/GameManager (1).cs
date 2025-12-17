using UnityEngine;

public class GameManager : MonoBehaviour
{
    // BÜTÜN OYUNUN TRAFÝK IÞIÐI BU:
    // static olduðu için her yerden "GameManager.oyunDevamEdiyor" diye ulaþýlabilir.
    public static bool oyunDevamEdiyor = false;

    public GameObject spawnerObjesi;
    public GameObject baslangicMenusuUI;

    void Start()
    {
        // Oyun açýldýðýnda IÞIK KIRMIZI (Hareket yok)
        oyunDevamEdiyor = false;

        // Spawner KAPALI
        if (spawnerObjesi != null) spawnerObjesi.SetActive(false);
        // Menü AÇIK
        if (baslangicMenusuUI != null) baslangicMenusuUI.SetActive(true);
    }

    public void OyunuBaslat()
    {
        // Butona basýnca IÞIK YEÞÝL (Herkes hareket etsin)
        oyunDevamEdiyor = true;

        // Spawner AÇIK
        if (spawnerObjesi != null) spawnerObjesi.SetActive(true);
        // Menü KAPALI
        if (baslangicMenusuUI != null) baslangicMenusuUI.SetActive(false);
    }
}