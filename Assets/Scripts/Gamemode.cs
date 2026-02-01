using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Gamemode : MonoBehaviour
{
    public static Gamemode Instance { get; private set; }
    
    public float gameDuration = 600f; // 10 minutes
    public TextMeshProUGUI timerText;
    public string starterNodeArchetype = "Space Generator";
    
    public CanvasGroup gameOverPanel;
    public float gameOverFadeDuration = 1f;
    
    public UnityEvent onGameStart;
    public UnityEvent onTimerStart;
    public UnityEvent onGameEnd;
    
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isTimerRunning = false;
    
    public Transform FloatersCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public float GetDifficulty()
    {
        return 1.0f + (gameDuration - timeRemaining) / 60.0f;
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.alpha = 0f;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
            gameOverPanel.gameObject.SetActive(false);
        }
        
        StartGame();
    }

    public void StartGame()
    {
        timeRemaining = gameDuration;
        isGameActive = true;
        isTimerRunning = false;
        onGameStart?.Invoke();
        UpdateTimerDisplay();
        
        SpawnStarterNode();
    }

    private void SpawnStarterNode()
    {
        LootSelectionUI lootUI = LootSelectionUI.Instance;
        if (lootUI == null || lootUI.lootSpawner == null) return;
        
        Vector3 spawnPos = lootUI.spawnPoint != null ? lootUI.spawnPoint.position : Vector3.zero;
        PowerNode starterNode = lootUI.lootSpawner.Spawn(starterNodeArchetype, spawnPos);
        
        if (starterNode != null)
        {
            starterNode.isStarterNode = true;
            starterNode.dragForce = 0f;
        }
    }

    public void OnStarterNodeDocked()
    {
        isTimerRunning = true;
        onTimerStart?.Invoke();
        
        SpawnStarterWeapon();
    }

    private void SpawnStarterWeapon()
    {
        LootSelectionUI lootUI = LootSelectionUI.Instance;
        if (lootUI == null || lootUI.lootSpawner == null) return;
        
        Vector3 spawnPos = lootUI.spawnPoint != null ? lootUI.spawnPoint.position : Vector3.zero;
        lootUI.lootSpawner.Spawn("AutoGun", spawnPos);
    }

    private void Update()
    {
        if (!isGameActive || !isTimerRunning || Time.timeScale == 0f) return;
        
        timeRemaining -= Time.deltaTime;
        
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }
        
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        
        XPBarSystem xpBar = XPBarSystem.GetInstance();
        int level = xpBar != null ? Mathf.FloorToInt(xpBar.Level) : 1;
        
        timerText.text = $"Lv.{level}  {minutes:00}:{seconds:00}";
    }

    private void EndGame()
    {
        isGameActive = false;
        onGameEnd?.Invoke();
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public float GetTimeElapsed()
    {
        return gameDuration - timeRemaining;
    }

    public float GetProgress()
    {
        return 1f - (timeRemaining / gameDuration);
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public bool IsTimerRunning()
    {
        return isTimerRunning;
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        
        isGameActive = false;
        isTimerRunning = false;
        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen()
    {
        if (gameOverPanel == null) yield break;
        
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = true;
        
        float elapsed = 0f;
        while (elapsed < gameOverFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            gameOverPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / gameOverFadeDuration);
            yield return null;
        }
        
        gameOverPanel.alpha = 1f;
        gameOverPanel.interactable = true;
        Time.timeScale = 0f;
        canRestart = true;
    }

    private bool canRestart = false;

    private void LateUpdate()
    {
        if (canRestart && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        canRestart = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
