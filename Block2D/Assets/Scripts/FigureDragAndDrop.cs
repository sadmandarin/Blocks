using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FigureDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float _dragSizeMultiplayer = 2;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalPosition;
    private Canvas _canvas;
    private bool _allPartsCanAttached;
    private FigureCell _referenceCell;
    private List<FigureCell> _figureCells = new();

    public List<FigureCell> FiguresCells => _figureCells;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _figureCells = GetComponentsInChildren<FigureCell>().ToList();
        _referenceCell = _figureCells.Where(x => x.IsFirstCell).First();
        _figureCells.Remove(_referenceCell);
        _originalPosition = _rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.IsInstanceExist && GameManager.Instance.ReadyToGame)
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _rectTransform.localScale = Vector3.one * _dragSizeMultiplayer;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        GameManager.Instance.GetClosestCell(_referenceCell, _figureCells);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.PlacementCells != null && GameManager.Instance.PlacementCells.Count != 0)
        {
            foreach (var cell in GameManager.Instance.PlacementCells)
            {
                cell.FillField(_referenceCell.Color);
            }

            GameManager.Instance.CheckIfCompleteRowOrColumn();

            Destroy(gameObject);
        }
        else
        {
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.anchoredPosition = _originalPosition;
        }
        GameManager.Instance.ClearPlacementPlacesList();
        _rectTransform.localScale = Vector3.one;
    }
}
