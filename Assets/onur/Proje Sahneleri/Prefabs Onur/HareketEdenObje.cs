using UnityEngine;

public class HareketEdenObje : MonoBehaviour
{
    public float hiz = 5f;
    public float yokEtZ = -10f; // Arkada yok olma mesafesi
    public bool local = false;

    void Update()
    {
        // 1. KURAL: Oyun baþlamadýysa KIPIRDAMA! (Eklediðimiz yer burasý)
        if (GameManager.oyunDevamEdiyor == false) return;

        // 2. KURAL: Bitiþ çizgisine deðildiyse DUR! (Senin eski kodun)
        if (FinishTrigger.GameStopped == true) return;

        // --- HAREKET KODLARI ---
        Vector3 dir = Vector3.back; // Geriye (sana doðru) gelmesi için

        if (local)
            transform.Translate(dir * hiz * Time.deltaTime, Space.Self);
        else
            transform.Translate(dir * hiz * Time.deltaTime, Space.World);

        // Arkayý geçince yok ol
        if (transform.position.z < yokEtZ)
            Destroy(gameObject);
    }
}