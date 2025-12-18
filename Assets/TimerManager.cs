using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [Header("Game Settings")]
    public float totalTimeSeconds = 180f;
    public int totalObjects = 5;

    [Header("Exit Settings")]
    public Transform basketSpawnPoint;

    // XR
    [Header("XR")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("UI Roots (Canvas Objects)")]
    public GameObject startCanvasRoot;   // GameCanvas (World Space)
    public GameObject hudCanvasRoot;     // HUDCanvas (Screen Space Overlay)
    public GameObject endCanvasRoot;     // EndCanvas (Screen Space - Camera)

    [Header("UI Panels")]
    public GameObject startPanel;        // StartPanel (World)
    public GameObject gameUIRoot;        // GameUIRoot (HUD)
    public GameObject endPanel;          // EndPanel (EndCanvas altında)

    [Header("UI Texts")]
    public TextMeshProUGUI timerText;    // HUD timer
    public TextMeshProUGUI endTitleText; // EndPanel title
    public TextMeshProUGUI endScoreText; // EndPanel score

    [Header("Buttons")]
    public Button startButton;
    public Button replayButton;
    public Button exitButton;

    // State
    private float timeLeft;
    private bool playing = false;
    private bool inGameArea = false;
    private int placedCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (xrOrigin == null) xrOrigin = FindObjectOfType<XROrigin>(true);

        if (basketSpawnPoint == null)
        {
            var go = GameObject.Find("BasketSpawnPoint");
            if (go != null) basketSpawnPoint = go.transform;
        }

        // Fallback find (Inspector boşsa)
        if (startCanvasRoot == null) startCanvasRoot = GameObject.Find("GameCanvas");
        if (hudCanvasRoot == null) hudCanvasRoot = GameObject.Find("HUDCanvas");
        if (endCanvasRoot == null) endCanvasRoot = GameObject.Find("EndCanvas");

        if (startPanel == null) startPanel = GameObject.Find("StartPanel");
        if (gameUIRoot == null) gameUIRoot = GameObject.Find("GameUIRoot");
        if (endPanel == null) endPanel = GameObject.Find("EndPanel");

        if (timerText == null) timerText = FindTMP("TimerText", gameUIRoot);
        if (endTitleText == null) endTitleText = FindTMP("EndTitleText", endPanel);
        if (endScoreText == null) endScoreText = FindTMP("EndScoreText", endPanel);

        if (startButton == null) startButton = FindButton("StartButton", startPanel);
        if (replayButton == null) replayButton = FindButton("ReplayButton", endPanel);
        if (exitButton == null) exitButton = FindButton("ExitButton", endPanel);
    }

    private void Start()
    {
        // Canvas kökleri açık kalsın (panel aç/kapa yapacağız)
        SetActiveSafe(startCanvasRoot, true);
        SetActiveSafe(hudCanvasRoot, true);
        SetActiveSafe(endCanvasRoot, true);

        // Listener bağla
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGameFromButton);
        }

        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(ReplayGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitGame);
        }

        // başlangıç UI
        SetActiveSafe(startPanel, false);
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);

        UpdateTimerUI(totalTimeSeconds);
        ScoreManager.Instance?.UpdateRecordText();
        ScoreManager.Instance?.UpdateScoreTextImmediate();
    }

    private void Update()
    {
        if (!playing) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        UpdateTimerUI(timeLeft);

        if (timeLeft <= 0f)
            EndGame(false);
    }

    // GameStartTrigger burayı çağırıyor
    public void OnEnteredGameArea()
    {
        inGameArea = true;

        // Start ekranı (world)
        SetActiveSafe(startPanel, true);

        // HUD kapalı, End kapalı
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);

        UpdateTimerUI(totalTimeSeconds);
        ScoreManager.Instance?.UpdateRecordText();
        ScoreManager.Instance?.UpdateScoreTextImmediate();
    }

    public void StartGameFromButton()
    {
        if (!inGameArea) return;

        placedCount = 0;
        timeLeft = totalTimeSeconds;
        playing = true;

        ScoreManager.Instance?.ResetScore();
        ResetAllCarryObjects();

        // HUD aç, Start/End kapa
        SetActiveSafe(startPanel, false);
        SetActiveSafe(endPanel, false);
        SetActiveSafe(gameUIRoot, true);

        UpdateTimerUI(timeLeft);
        ScoreManager.Instance?.UpdateRecordText();
        ScoreManager.Instance?.UpdateScoreTextImmediate();
    }

    public void OnObjectPlaced()
    {
        if (!playing) return;

        placedCount++;
        if (placedCount >= totalObjects)
            EndGame(true);
    }

    // ✅ BONUSLU ENDGAME (KALAN SANİYEYİ EKLER)
    private void EndGame(bool win)
    {
        playing = false;

        // HUD açık kalsın, EndPanel aç
        SetActiveSafe(gameUIRoot, true);
        SetActiveSafe(endCanvasRoot, true);
        SetActiveSafe(endPanel, true);
        SetActiveSafe(startPanel, false);

        int baseScore = ScoreManager.Instance != null ? ScoreManager.Instance.score : 0;
        int bonus = win ? Mathf.CeilToInt(timeLeft) : 0;
        int finalScore = baseScore + bonus;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.score = finalScore;

        if (endTitleText != null)
            endTitleText.text = win ? "KAZANDIN!" : "KAYBETTİN!";

        if (endScoreText != null)
            endScoreText.text = win
                ? $"TOPLAM SKOR: {finalScore}  (BONUS: {bonus})"
                : $"TOPLAM SKOR: {finalScore}";

        ScoreManager.Instance?.TrySetNewRecord();
        ScoreManager.Instance?.UpdateRecordText();
        ScoreManager.Instance?.UpdateScoreTextImmediate();
    }

    public void ReplayGame()
    {
        StartGameFromButton();
    }

    // ✅ ÇIKIŞ: her şeyi sıfırla + basket sahasına ışınla
    public void ExitGame()
    {
        playing = false;
        inGameArea = false;

        placedCount = 0;
        timeLeft = totalTimeSeconds;

        ScoreManager.Instance?.ResetScore();
        ResetAllCarryObjects();

        // UI kapat
        SetActiveSafe(startPanel, false);
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);

        // İstersen canvas köklerini de kapat:
        // SetActiveSafe(startCanvasRoot, false);
        // SetActiveSafe(hudCanvasRoot, false);
        // SetActiveSafe(endCanvasRoot, false);

        TeleportTo(basketSpawnPoint);
        FindObjectOfType<GameStartTrigger>(true)?.ResetTrigger();
    }

    // ---------------- helpers ----------------

    private void UpdateTimerUI(float seconds)
    {
        if (timerText == null) return;

        int s = Mathf.CeilToInt(seconds);
        int m = s / 60;
        int r = s % 60;
        timerText.text = $"{m:00}:{r:00}";
    }

    private void ResetAllCarryObjects()
    {
        var objs = FindObjectsOfType<CarryObjectID>(true);
        foreach (var o in objs) o.ResetObject();
    }

    private void TeleportTo(Transform target)
    {
        if (xrOrigin == null || target == null) return;

        xrOrigin.MoveCameraToWorldLocation(target.position);

        Vector3 flatForward = target.forward;
        flatForward.y = 0f;
        if (flatForward.sqrMagnitude < 0.001f) flatForward = Vector3.forward;

        xrOrigin.MatchOriginUpCameraForward(Vector3.up, flatForward);
    }

    private static void SetActiveSafe(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }

    private static Button FindButton(string name, GameObject root)
    {
        if (root == null) return null;

        foreach (var b in root.GetComponentsInChildren<Button>(true))
            if (b.name == name || b.name.Contains(name))
                return b;

        return root.GetComponentInChildren<Button>(true);
    }

    private static TextMeshProUGUI FindTMP(string name, GameObject root)
    {
        if (root == null) return null;

        foreach (var t in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            if (t.name == name || t.name.Contains(name))
                return t;

        return root.GetComponentInChildren<TextMeshProUGUI>(true);
    }
}
