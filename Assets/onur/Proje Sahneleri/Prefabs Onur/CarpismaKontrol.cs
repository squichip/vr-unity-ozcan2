using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CarpismaKontrol : MonoBehaviour
{
    public GameObject basarisizYazisi; // Ekrana gelecek "BAÞARISIZ" yazýsý

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan þeyin etiketi "Engel" mi?
        if (other.CompareTag("Engel"))
        {
            Debug.Log("GÜM! Çarptýn.");

            // 1. Yazýyý aç (Eðer kutuya sürüklediysen)
            if (basarisizYazisi != null)
            {
                basarisizYazisi.SetActive(true);
            }

            // 2. Bekle ve Resetle
            StartCoroutine(SahneYenile());
        }
    }

    IEnumerator SahneYenile()
    {
        yield return new WaitForSeconds(1); // 1 saniye bekle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}