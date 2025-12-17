using UnityEngine;
using System.Collections.Generic;

public class ScoreTriggeryeni : MonoBehaviour
{
    [Header("Filter")]
    public string ballTag = "Ball";
    public bool requireDownward = true;
    public float perBallCooldown = 0.8f;

    [Header("UI")]
    public string message = "Baþarýlý atýþ!";
    public float showSeconds = 1.5f; // SÜRE AYNI
    public int fontSize = 56;

    float showUntil = -1f; // oyuna girince yazmasýn
    readonly Dictionary<int, float> lastHit = new Dictionary<int, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag(ballTag)) return;

        int id = other.GetInstanceID();
        if (lastHit.TryGetValue(id, out float t) && Time.time - t < perBallCooldown) return;

        var rb = other.attachedRigidbody;
        if (requireDownward && rb != null && rb.velocity.y > 0f) return;

        lastHit[id] = Time.time;
        showUntil = Time.time + showSeconds;
    }

    private void OnGUI()
    {
        if (Time.time >= showUntil) return; // baþlangýçta 1 kere görünme bug'ý gider

        float w = Screen.width;
        float h = Screen.height;

        Rect textRect = new Rect(0, h * 0.08f, w, 90);
        Rect bgRect = new Rect(w * 0.25f, h * 0.075f, w * 0.5f, 95);

        // Arka plan (yarý saydam)
        GUI.color = new Color(0f, 0f, 0f, 0.55f);
        GUI.Box(bgRect, GUIContent.none);

        // Yazý stili
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        // Siyah outline (4 yönden)
        GUI.color = Color.black;
        GUI.Label(new Rect(textRect.x - 2, textRect.y, textRect.width, textRect.height), message, style);
        GUI.Label(new Rect(textRect.x + 2, textRect.y, textRect.width, textRect.height), message, style);
        GUI.Label(new Rect(textRect.x, textRect.y - 2, textRect.width, textRect.height), message, style);
        GUI.Label(new Rect(textRect.x, textRect.y + 2, textRect.width, textRect.height), message, style);

        // Asýl yazý
        GUI.color = Color.white;
        GUI.Label(textRect, message, style);

        GUI.color = Color.white;
    }
}
        