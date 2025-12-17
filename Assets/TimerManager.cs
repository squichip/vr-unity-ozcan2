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
    public Transform basketSpawnPoint; // Sahnedeki BasketSpawnPoint

    // XR
    private XROrigin xrOrigin;

    // UI Roots
    private GameObject gameCanvas;
    private GameObject startPanel;
    private GameObject gameUIRoot;
    private GameObject endPanel;

    // UI Texts
    private TextMeshProUGUI timerText;
    private TextMeshProUGUI endTitleText;
    private TextMeshProUGUI endScoreText;

    // Buttons
    private Button startButton;
    private Button replayButton;
    private Button exitButton;

    // State
    private float timeLeft;
    private bool playing = false;
    private bool inGameArea = false;
    private int placedCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // XR
        xrOrigin = FindObjectOfType<XROrigin>(true);

        // Spawn otomatik bul
        if (basketSpawnPoint == null)
        {
            var go = GameObject.Find("BasketSpawnPoint");
            if (go != null) basketSpawnPoint = go.transform;
        }

        // UI roots (isimle)
        gameCanvas = GameObject.Find("GameCanvas");
        startPanel = GameObject.Find("StartPanel");
        gameUIRoot = GameObject.Find("GameUIRoot");
        endPanel = GameObject.Find("EndPanel");

        // Texts (isimle, yoksa fallback)
        timerText = FindTMP("TimerText", gameUIRoot);
        endTitleText = FindTMP("EndTitleText", endPanel);
        endScoreText = FindTMP("EndScoreText", endPanel);

        // Buttons (isimle, yoksa fallback)
        startButton = FindButton("StartButton", startPanel);
        replayButton = FindButton("ReplayButton", endPanel);
        exitButton = FindButton("ExitButton", endPanel);
    }

    private void Start()
    {
        // Listenerları garanti bağla
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
        SetActiveSafe(gameCanvas, false);
        SetActiveSafe(startPanel, false);
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);
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

        SetActiveSafe(gameCanvas, true);
        ForceVisibleScale(gameCanvas);

        // Canvas world-space ise kameraya yapıştır → HUD kesin görünür
        AttachCanvasToCameraIfWorldSpace();

        // Start ekranı
        SetActiveSafe(startPanel, true);
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);

        ForceVisibleScale(startPanel);

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

        // reset
        ScoreManager.Instance?.ResetScore();
        ResetAllCarryObjects();

        // HUD aç
        SetActiveSafe(startPanel, false);
        SetActiveSafe(endPanel, false);
        SetActiveSafe(gameUIRoot, true);

        ForceVisibleScale(gameUIRoot);

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

        // Oyun HUD açık kalsın (skor/rekor görünür)
        SetActiveSafe(gameUIRoot, true);
        SetActiveSafe(endPanel, true);
        SetActiveSafe(startPanel, false);

        ForceVisibleScale(endPanel);

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

        // oyun state reset
        placedCount = 0;
        timeLeft = totalTimeSeconds;

        // skor + objeler reset
        ScoreManager.Instance?.ResetScore();
        ResetAllCarryObjects();

        // UI kapat
        SetActiveSafe(startPanel, false);
        SetActiveSafe(gameUIRoot, false);
        SetActiveSafe(endPanel, false);
        SetActiveSafe(gameCanvas, false);

        // Basket sahasına gönder
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

    private void AttachCanvasToCameraIfWorldSpace()
    {
        if (gameCanvas == null || xrOrigin == null) return;

        var canvas = gameCanvas.GetComponent<Canvas>();
        if (canvas == null) return;

        // Sadece WorldSpace ise kameraya yapıştır
        if (canvas.renderMode != RenderMode.WorldSpace) return;

        Camera cam = xrOrigin.Camera;
        if (cam == null) return;

        gameCanvas.transform.SetParent(cam.transform, false);
        gameCanvas.transform.localPosition = new Vector3(0f, 0f, 1.25f);
        gameCanvas.transform.localRotation = Quaternion.identity;
        gameCanvas.transform.localScale = Vector3.one * 0.001f;

        canvas.worldCamera = cam;
    }

    private static void SetActiveSafe(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }

    private static void ForceVisibleScale(GameObject go)
    {
        if (go == null) return;
        var rt = go.GetComponent<RectTransform>();
        if (rt != null && (rt.localScale.x == 0f || rt.localScale.y == 0f || rt.localScale.z == 0f))
            rt.localScale = Vector3.one;
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
