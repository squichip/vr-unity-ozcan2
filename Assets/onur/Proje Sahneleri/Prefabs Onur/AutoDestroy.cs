using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float yasamSuresi = 8f;

    void Start()
    {
        Destroy(gameObject, yasamSuresi);
    }
}

