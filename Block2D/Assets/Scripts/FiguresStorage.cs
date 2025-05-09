using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FiguresStorage", menuName = "Scriptable Objects/FiguresStorage")]
public class FiguresStorage : ScriptableObject
{
    [field: SerializeField]
    public List<FigureDragAndDrop> figures;

    public List<FigureDragAndDrop> Get3RandomFigures()
    {
        var rnd = new System.Random();
        var randomFigures = figures.OrderBy(x => rnd.Next()).Take(3).ToList();
        return randomFigures;
    }
}
