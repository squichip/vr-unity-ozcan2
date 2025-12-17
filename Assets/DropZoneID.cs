using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DropZoneID : MonoBehaviour
{
    public int targetID;

    private void OnTriggerEnter(Collider other)
    {
        CarryObjectID obj = other.GetComponent<CarryObjectID>();
        if (obj == null) return;

        if (obj.objectID != targetID) return;
        if (obj.placedCorrectly) return;

        obj.placedCorrectly = true;

        // Snap
        Vector3 snapPos = transform.position;
        snapPos.y += transform.localScale.y / 2f;
        snapPos.y += other.transform.localScale.y / 2f;
        other.transform.position = snapPos;

        // Fizik kilitle
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Grab kapat
        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        // Skor + animasyon
        ScoreManager.Instance?.AddScoreWithEffect();

        // Sayaç
        TimerManager.Instance?.OnObjectPlaced();
    }
}
