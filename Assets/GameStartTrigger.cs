using UnityEngine;
using Unity.XR.CoreUtils;

public class GameStartTrigger : MonoBehaviour
{
    [Header("Spawn")]
    public Transform gameAreaSpawnPoint;   // Oyun alanındaki spawn noktası
    public XROrigin xrOrigin;              // XR Origin

    [Header("Game Logic")]
    public TimerManager timerManager;

    private bool triggered = false;

    private void Awake()
    {
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>(true);

        if (timerManager == null)
            timerManager = FindObjectOfType<TimerManager>(true);
    }

    private bool IsPlayer(Collider other)
    {
        // XR Origin içindeki collider’ları yakala
        if (other.GetComponentInParent<XROrigin>() != null) return true;
        if (other.CompareTag("Player")) return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        if (triggered) return;
        triggered = true;

        Debug.Log("[GameStartTrigger] PLAYER ENTERED → GAME AREA");

        // Oyuncuyu oyun alanına taşı
        if (xrOrigin != null && gameAreaSpawnPoint != null)
        {
            xrOrigin.MoveCameraToWorldLocation(gameAreaSpawnPoint.position);

            Vector3 flatForward = gameAreaSpawnPoint.forward;
            flatForward.y = 0f;
            if (flatForward.sqrMagnitude < 0.001f)
                flatForward = Vector3.forward;

            xrOrigin.MatchOriginUpCameraForward(Vector3.up, flatForward);
        }

        // UI + StartPanel aç
        if (timerManager != null)
            timerManager.OnEnteredGameArea();
        else
            Debug.LogError("[GameStartTrigger] TimerManager BULUNAMADI");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        // Normal çıkışta da resetle ki tekrar girince çalışsın
        triggered = false;
    }

    // ✅ TimerManager ExitGame çağıracak
    public void ResetTrigger()
    {
        triggered = false;
        Debug.Log("[GameStartTrigger] ResetTrigger()");
    }
}
