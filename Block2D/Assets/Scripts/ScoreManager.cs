using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {  get; private set; }
    public static bool SsInstanceExist => Instance != null;

    private int _score = 0;
    private float _multiplayer;

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

    private void Start()
    {
        IncreaseScore(0);
    }

    public void IncreaseScore(int scoreAmount)
    {
        _score += scoreAmount;
        OnScoreChanged?.Invoke(_score);
    }
}

