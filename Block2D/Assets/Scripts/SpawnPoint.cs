using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private RectTransform _rectTransform;
    private FigureDragAndDrop _spawnedFigure;

    public FigureDragAndDrop SpawnedFigure => _spawnedFigure;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameManager.OnGameRestarted += ClearObject;
    }

    private void OnDisable()
    {
        GameManager.OnGameRestarted -= ClearObject;
    }

    public void SpawnFigure(FigureDragAndDrop figure)
    {
        _spawnedFigure = Instantiate(figure, _rectTransform);
        _spawnedFigure.Init(this);
    }

    public void ClearObject()
    {
        if (_spawnedFigure != null)
        {
            Destroy(_spawnedFigure.gameObject);
            _spawnedFigure = null;
        }
    }
}
