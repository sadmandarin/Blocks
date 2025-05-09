using UnityEngine;
using UnityEngine.UI;

public class FigureCell : MonoBehaviour
{
    [SerializeField] private int _rowIndex;
    [SerializeField] private int _columnIndex;
    [SerializeField] private bool _isFirstCell;

    private RectTransform _rectTransform;
    private Color _color;
    public RectTransform RectTransform => _rectTransform;
    public int RowIndex => _rowIndex;
    public int ColumnIndex => _columnIndex;
    public bool IsFirstCell => _isFirstCell;
    public Color Color => _color;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _color = GetComponent<Image>().color;
    }
}
