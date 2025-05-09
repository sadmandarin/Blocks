using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameLostWindow _gameLostPanel;

    private RectTransform _rectTransform;
    private GameLostWindow _activeGameLostPanel;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameManager.OnGameLost += ActivateLostWindow;
        GameManager.OnGameRestarted += ResetAllWindows;
    }

    private void OnDisable()
    {
        GameManager.OnGameLost -= ActivateLostWindow;
        GameManager.OnGameRestarted -= ResetAllWindows;
    }

    private void ActivateLostWindow()
    {
        _activeGameLostPanel = Instantiate(_gameLostPanel, _rectTransform);
    }

    private void ResetAllWindows()
    {
        Destroy(_activeGameLostPanel.gameObject);
    }
}
