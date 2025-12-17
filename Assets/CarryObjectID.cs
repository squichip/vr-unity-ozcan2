using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CarryObjectID : MonoBehaviour
{
    public int objectID;
    [HideInInspector] public bool placedCorrectly = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Rigidbody rb;
    private XRGrabInteractable grab;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    // DropZone doğru yerleştirince çağrılır
    public void LockObject()
    {
        placedCorrectly = true;

        if (rb != null)
        {
            // ✅ Önce hızları sıfırla (kinematic yapmadan önce!)
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;

            rb.Sleep();
        }

        if (grab != null)
            grab.enabled = false;
    }

    // Replay / Yeni oyun için reset
    public void ResetObject()
    {
        placedCorrectly = false;

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.WakeUp();
        }

        if (grab != null)
            grab.enabled = true;
    }
}
