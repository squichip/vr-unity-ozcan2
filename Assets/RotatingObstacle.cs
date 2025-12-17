using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public float rotationSpeed = 90f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(Vector3.up * rotationSpeed * Time.fixedDeltaTime)
        );
    }
}
