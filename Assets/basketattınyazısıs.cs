using UnityEngine;
using System.Collections.Generic;

public class ShotFeedback : MonoBehaviour
{
    [Header("UI")]
    public string message = "Başarılı atış!";
    public float showSeconds = 1.5f;
    public int fontSize = 42;

    [Header("Filter")]
    public string ballTag = "Ball";
    public bool requireDownward = true;   // alttan yukarı girerse saymasın
    public float perBallCooldown = 0.8f;  // aynı top için spam engeli

    float showUntil;
    readonly Dictionary<int, float> lastHit = new();

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ballTag)) return;

        int id = other.GetInstanceID();
        if (lastHit.TryGetValue(id, out float t) && Time.time - t < perBallCooldown) return;

        var rb = other.attachedRigidbody;
        if (requireDownward && rb != null && rb.velocity.y > 0f) return;

        lastHit[id] = Time.time;
        showUntil = Time.time + showSeconds;
    }

    void OnGUI()
    {
        if (Time.time > showUntil) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            alignment = TextAnchor.UpperCenter
        };

        GUI.Label(new Rect(0, 40, Screen.width, 80), message, style);
    }
}
