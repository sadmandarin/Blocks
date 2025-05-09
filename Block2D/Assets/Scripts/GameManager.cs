using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsInstanceExist => Instance != null;

    private PlayerData _playerData;
    private bool _readyToGame = false;
    private States _currentGameState;

    public enum States
    {
        Preparing,
        Playing,
        Lost,
        Restart
    }

    public States CurrentGameState => _currentGameState;
    public bool ReadyToGame => _readyToGame;
    public PlayerData PlayerData => _playerData;


    public static event Action OnGameLost;
    public static event Action OnGameRestarted;
    private void Awake()
    {
        SetGameState(States.Preparing);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _playerData = new PlayerData();

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void RestartGame()
    {
        OnGameRestarted?.Invoke();
    }

    private void SetHighScore()
    {
        if (_playerData.highScore < ScoreManager.Instance.Score)
        {
            _playerData.highScore = ScoreManager.Instance.Score;
        }
    }

    private void GameLost()
    {
        SetHighScore();
        OnGameLost?.Invoke();
    }


    public void SetGameState(States state)
    {
        switch (state)
        {
            case States.Preparing:
                break;
            case States.Playing:
                _readyToGame = true;
                break;
            case States.Lost:
                GameLost();
                break;
            case States.Restart:
                RestartGame();
                break;
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int highScore = 0;
}
