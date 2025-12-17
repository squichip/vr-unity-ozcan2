using UnityEngine;

public class EngelHareket : MonoBehaviour
{
    public float hiz = 6f; // Engelin hýzý
    public float yokOlmaSuresi = 10f; // 10 saniye sonra sahne dolsun diye silinsin

    void Start()
    {
        // Hafýza dolmasýn diye 10 saniye sonra bu objeyi yok et
        Destroy(gameObject, yokOlmaSuresi);
    }

    void Update()
    {
        // Objeyi geriye (sana doðru) hareket ettir
        // Eðer engeller ters gidiyorsa Vector3.back yerine Vector3.forward yap
        transform.Translate(Vector3.back * hiz * Time.deltaTime);
    }
}