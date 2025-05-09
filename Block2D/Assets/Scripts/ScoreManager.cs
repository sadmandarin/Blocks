using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {  get; private set; }
    public static bool IsInstanceExist => Instance != null;

    private int _score = 0;
    private float _multiplayer;
    private int _scoreAmount = 50;

    public int Score => _score;

    public static event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        GameManager.OnGameRestarted += ResetScore;
        FieldBuilderAndFigurePlacer.OnNeedToAddExp += IncreaseScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameRestarted -= ResetScore;
        FieldBuilderAndFigurePlacer.OnNeedToAddExp -= IncreaseScore;
    }

    private void Start()
    {
        ResetScore();
    }

    public void IncreaseScore()
    {
        _score += _scoreAmount;
        OnScoreChanged?.Invoke(_score);
    }

    private void ResetScore()
    {
        _score = 0;
        OnScoreChanged?.Invoke(_score);
    }
}

