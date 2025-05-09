using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLostWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _youScoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        ShowGameLostWindow();
        _restartButton.onClick.AddListener(RestartButtonAction);
    }

    private void OnDestroy()
    {
        _restartButton?.onClick.RemoveListener(RestartButtonAction);
    }

    private void ShowGameLostWindow()
    {
        _youScoreText.text = $"You Score: {ScoreManager.Instance.Score}";
        _bestScoreText.text = $"Best Score: {GameManager.Instance.PlayerData.highScore}";

    }

    private void RestartButtonAction()
    {
        GameManager.Instance.SetGameState(GameManager.States.Restart);
    }
}
