using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldBuilderAndFigurePlacer : MonoBehaviour
{
    public static FieldBuilderAndFigurePlacer Instance;
    public bool IsInstanceExist => Instance != null;

    [SerializeField] private FiguresStorage _storage;
    [SerializeField] private RectTransform _layout;
    [SerializeField] private GameFieldCell _cell;
    [SerializeField] private int _fieldWidth;
    [SerializeField] private int _fieldHeight;

    private GameFieldCell[,] _gameField;

    private List<GameFieldCell> _placementCells = new();
    private List<SpawnPoint> _spawnPoints = new();

    public List<GameFieldCell> PlacementCells => _placementCells;

    public static event Action OnNeedToAddExp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameRestarted += ResetGameField;
    }

    private void OnDisable()
    {
        GameManager.OnGameRestarted -= ResetGameField;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(SetGameOnStart());
    }

    public void ClearPlacementPlacesList() => _placementCells?.Clear();

    private IEnumerator SetGameOnStart()
    {
        _gameField = new GameFieldCell[_fieldWidth, _fieldHeight];
        for (int i = 0; i < _fieldWidth; i++)
        {
            for (int j = 0; j < _fieldHeight; j++)
            {
                var cell = Instantiate(_cell, _layout);
                cell.Init(i, j);
                _gameField[i, j] = cell;
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        GameManager.Instance.SetGameState(GameManager.States.Playing);
        _spawnPoints = FindObjectsByType<SpawnPoint>(sortMode: FindObjectsSortMode.None).ToList();

        SpawnNewFiguresSet();
    }

    public void GetClosestCell(FigureCell figureCell, List<FigureCell> figureCells)
    {
        float maxDistance = 50f;
        float closestSqrDistance = float.MaxValue;
        _placementCells = new();

        List<GameFieldCell> bestPlacement = null;

        // Получаем экранную позицию якорной фигуры
        Vector2 figureScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, figureCell.RectTransform.position);

        foreach (var cell in _gameField)
        {
            Vector2 cellScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cell.RectTransform.position); // <=== Добавь RectTransform в GameFieldCell
            float sqrDistance = (figureScreenPos - cellScreenPos).sqrMagnitude;

            if (sqrDistance < maxDistance * maxDistance && !cell.IsFilled)
            {
                if (CheckIfCanPlaceAllFigure(cell, figureCells, out _placementCells))
                {
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        bestPlacement = _placementCells;
                    }
                }
            }
        }

        ClearAllPreviews();

        if (bestPlacement != null)
        {
            foreach (var cell in bestPlacement)
            {
                cell.PreviewField(figureCell.Color);
            }
        }
    }

    public void ClearAllPreviews()
    {
        foreach (var cell in _gameField)
        {
            cell.ClearField();
        }
    }

    private bool CheckIfCanPlaceAllFigure(GameFieldCell fieldCell, List<FigureCell> figureCells, out List<GameFieldCell> placementFieldCell)
    {
        var tempPlacement = new List<GameFieldCell>();
        int fieldWidth = _fieldWidth;
        int fieldHeight = _fieldHeight;

        // Добавляем якорную клетку
        if (fieldCell.IsFilled)
        {
            placementFieldCell = null;
            return false;
        }

        tempPlacement.Add(fieldCell);

        foreach (var cell in figureCells)
        {
            int targetX = fieldCell.RowIndex + cell.RowIndex;     // cell.RowIndex - уже смещение (например -1, 0)
            int targetY = fieldCell.ColumnIndex + cell.ColumnIndex;

            // Проверка на выход за пределы
            if (targetX < 0 || targetX >= fieldWidth || targetY < 0 || targetY >= fieldHeight)
            {
                placementFieldCell = null;
                return false;
            }

            GameFieldCell targetCell = _gameField[targetX, targetY];

            // Проверка занятости
            if (targetCell.IsFilled)
            {
                placementFieldCell = null;
                return false;
            }

            tempPlacement.Add(targetCell);
        }
        placementFieldCell = tempPlacement;
        return true;
    }

    private void SpawnNewFiguresSet()
    {
        var figures = _storage.Get3RandomFigures();
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            _spawnPoints[i].SpawnFigure(figures[i]);
        }
    }

    public void CheckIfCompleteRowOrColumn()
    {
        for (int y = 0; y < _fieldHeight; y++)
        {
            if (Enumerable.Range(0, _fieldWidth).All(x => _gameField[x, y].IsFilled))
                ClearColumn(y);
        }

        for (int x = 0; x < _fieldWidth; x++)
        {
            if (Enumerable.Range(0, _fieldHeight).All(y => _gameField[x, y].IsFilled))
                ClearRow(x);
        }

        if (CheckIfAllFiguresArePlaced())
        {
            SpawnNewFiguresSet();
        }

        if (CheckIfLose())
        {
            GameManager.Instance.SetGameState(GameManager.States.Lost);
        }
    }
    private void ClearRow(int rowIndex)
    {
        for (int i = 0; i < _fieldHeight; i++)
            _gameField[rowIndex, i].CompleteField();

        OnNeedToAddExp?.Invoke();
    }

    private void ClearColumn(int columnIndex)
    {
        for (int i = 0; i < _fieldWidth; i++)
            _gameField[i, columnIndex].CompleteField();

        OnNeedToAddExp?.Invoke();
    }

    public bool CheckIfAllFiguresArePlaced()
    {
        return _spawnPoints.All(x => x.SpawnedFigure == null);
    }

    private bool CheckIfLose()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            if (spawnPoint.SpawnedFigure == null) continue;

            var figureCells = spawnPoint.SpawnedFigure.GetComponentsInChildren<FigureCell>().ToList();

            foreach (var cell in _gameField)
            {
                if (!cell.IsFilled && TryToCheckIfCanPlaceFigure(cell, figureCells))
                    return false;
            }
        }
        return true;
    }

    private bool TryToCheckIfCanPlaceFigure(GameFieldCell fieldCell, List<FigureCell> figureCells)
    {
        int fieldWidth = _fieldWidth;
        int fieldHeight = _fieldHeight;

        foreach (var cell in figureCells)
        {
            int targetX = fieldCell.RowIndex + cell.RowIndex;     // cell.RowIndex - уже смещение (например -1, 0)
            int targetY = fieldCell.ColumnIndex + cell.ColumnIndex;

            // Проверка на выход за пределы
            if (targetX < 0 || targetX >= fieldWidth || targetY < 0 || targetY >= fieldHeight)
            {
                return false;
            }

            GameFieldCell targetCell = _gameField[targetX, targetY];

            // Проверка занятости
            if (targetCell.IsFilled)
            {
                return false;
            }

        }

        return true;
    }

    private void ResetGameField()
    {
        foreach(var cell in _gameField)
        {
            Destroy(cell.gameObject);
        }

        StartGame();
    }
}
