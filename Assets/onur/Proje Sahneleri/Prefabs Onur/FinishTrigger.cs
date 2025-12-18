using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine (IEnumerator) için gerekli

public class FinishTrigger : MonoBehaviour
{
    // Bütün dünya dursun diye bunu yine tutuyoruz
    public static bool GameStopped = false;

    // Ekrana gelecek "TEBRİKLER" yazısı
    public GameObject tebriklerUI;

    void Awake()
    {
        // Oyun başlayınca durma komutunu kaldır
        GameStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan şey PLAYER mı? (CarpismaKontrol'ün tam tersi mantık)
        if (other.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null)
        {
            Debug.Log("FİNİSH! Tebrikler.");

            // 1. Oyunu (hareket eden her şeyi) durdur
            GameStopped = true;

            // 2. Yazıyı aç
            if (tebriklerUI != null)
            {
                tebriklerUI.SetActive(true);
            }

            // 3. Bekle ve Resetle (Senin kodundaki sistemin aynısı)
            StartCoroutine(SahneYenile());
        }
    }

    IEnumerator SahneYenile()
    {
        // Tebrikler yazısı okunsun diye 5 saniye bekletiyoruz
        yield return new WaitForSeconds(3);

        // Sahneyi baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}