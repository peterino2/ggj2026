using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Gamemode : MonoBehaviour
{
    public static Gamemode Instance { get; private set; }
    
    public float gameDuration = 600f; // 10 minutes
    public TextMeshProUGUI timerText;
    public string starterNodeArchetype = "Basic Generator";
    
    public UnityEvent onGameStart;
    public UnityEvent onTimerStart;
    public UnityEvent onGameEnd;
    
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
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
        
        LootSelectionUI lootUI = LootSelectionUI.Instance;
        if (lootUI != null)
        {
            lootUI.ShowSelection(RollType.WeaponOnly);
        }
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
        timerText.text = $"{minutes:00}:{seconds:00}";
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
}
