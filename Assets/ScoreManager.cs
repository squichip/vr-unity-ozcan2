using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public int score = 0;
    public int scorePerObject = 100;

    [Header("UI")]
    public TextMeshProUGUI scoreText;         // ScoreText
    public TextMeshProUGUI recordText;        // RecordText
    public TextMeshProUGUI floatingScoreText; // FloatingScoreText

    [Header("Floating +Score")]
    public float floatDuration = 0.6f;

    private Vector3 floatingStartPos;
    private Coroutine floatCo;

    private const string RECORD_KEY = "CARRY_RECORD";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (floatingScoreText != null)
        {
            floatingStartPos = floatingScoreText.rectTransform.position;
            floatingScoreText.gameObject.SetActive(false);
        }

        UpdateScoreTextImmediate();
        UpdateRecordText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreTextImmediate();
        UpdateRecordText();

        if (floatingScoreText != null)
        {
            if (floatCo != null) StopCoroutine(floatCo);
            floatingScoreText.rectTransform.position = floatingStartPos;
            floatingScoreText.gameObject.SetActive(false);
        }
    }

    // DropZone çağırıyor
    public void AddScoreWithEffect()
    {
        // ✅ skoru HEMEN ekle
        score += scorePerObject;
        UpdateScoreTextImmediate();

        // animasyon
        if (floatingScoreText == null || scoreText == null) return;

        if (floatCo != null) StopCoroutine(floatCo);
        floatCo = StartCoroutine(FloatingScoreRoutine(scorePerObject));
    }

    private IEnumerator FloatingScoreRoutine(int amount)
    {
        var floatRT = floatingScoreText.rectTransform;
        var scoreRT = scoreText.rectTransform;

        Vector3 startPos = floatingStartPos;
        Vector3 endPos = scoreRT.position;

        floatingScoreText.gameObject.SetActive(true);
        floatingScoreText.text = "+" + amount;
        floatRT.position = startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, floatDuration);
            floatRT.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        floatRT.position = startPos;
        floatingScoreText.gameObject.SetActive(false);
    }

    public int GetRecord() => PlayerPrefs.GetInt(RECORD_KEY, 0);

    public bool TrySetNewRecord()
    {
        int record = GetRecord();
        if (score > record)
        {
            PlayerPrefs.SetInt(RECORD_KEY, score);
            PlayerPrefs.Save();
            UpdateRecordText();
            return true;
        }

        UpdateRecordText();
        return false;
    }

    public void UpdateScoreTextImmediate()
    {
        if (scoreText != null)
            scoreText.text = "SKOR: " + score;
    }

    public void UpdateRecordText()
    {
        if (recordText != null)
            recordText.text = "REKOR: " + GetRecord();
    }
}
