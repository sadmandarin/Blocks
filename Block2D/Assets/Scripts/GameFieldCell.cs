using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldCell : MonoBehaviour
{
    [SerializeField] private int _rowIndex;
    [SerializeField] private int _columnIndex;
    [SerializeField] private bool _isFilled;
    [SerializeField] private Image _previewImage;

    private Image _image;
    private Vector3 _position;
    private RectTransform _rectTransform;
    
    public int RowIndex => _rowIndex;
    public int ColumnIndex => _columnIndex;
    public bool IsFilled => _isFilled;
    public RectTransform RectTransform => _rectTransform;

    public void Init(int rowIndex, int heightIndex)
    {
        _rowIndex = rowIndex;
        _columnIndex = heightIndex;
        _isFilled = false;
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        if (_previewImage.isActiveAndEnabled)
            _previewImage.enabled = false;
        
        StartCoroutine(nameof(FadeIn));
    }

    public void PreviewField(Color color)
    {
        _previewImage.color = color;
        _previewImage.enabled = true;
    } 

    public void FillField(Color color)
    {
        _isFilled = true;
        _previewImage.enabled = false;
        _image.color = color;
    }

    public void ClearField()
    {
        if (!_isFilled)
        {
            _previewImage.enabled = false;
            _previewImage.color = Color.white;
        }
    }

    public void CompleteField()
    {
        if (_isFilled)
        {
            _isFilled = false;
            _image.color = Color.white;
        }
    }


    private IEnumerator FadeIn()
    {
        Color color = _image.color;

        float fromAlpha = color.a;
        float toAlpha = 1;

        float elapsedTime = 0;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime/0.5f);

            color.a = alpha;
            _image.color = color;

            yield return null;
        }
    }
}
