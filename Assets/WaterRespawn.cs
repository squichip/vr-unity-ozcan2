using UnityEngine;

public class WaterRespawn : MonoBehaviour
{
    public Vector3 respawnPosition = new Vector3(9.87f, 0.179f, -27.02f);
    private AudioSource splashSound;
    private bool triggered = false;

    void Start()
    {
        splashSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            if (splashSound != null)
                splashSound.Play();

            // CAN DÜŞÜR
            LifeManager.Instance.LoseLife();

            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = respawnPosition;
                cc.enabled = true;
            }
            else
            {
                other.transform.position = respawnPosition;
            }

            Invoke(nameof(ResetTrigger), 1f);
        }
    }

    void ResetTrigger()
    {
        triggered = false;
    }
}
