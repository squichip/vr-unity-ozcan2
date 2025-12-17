using UnityEngine;

public class MovingBallZ : MonoBehaviour
{
    public float minZ = -17.87f;
    public float maxZ = -12.373f;
    public float speed = 2f;

    void Update()
    {
        float z = Mathf.PingPong(Time.time * speed, maxZ - minZ) + minZ;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            z
        );
    }
}
