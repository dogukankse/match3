using Models;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bgPrefab;
    [SerializeField] private Tile _tilePrefab;

    [Header("Game Rules")] [SerializeField]
    private int _minBlastGroupCount = 2;

    [SerializeField] private MinMax<int> _colorVariantCount;
    [SerializeField] private MinMax<int> _rowsBounds;
    [SerializeField] private MinMax<int> _columnsBounds;

    private GameBoard _gameBoard;
    private Bounds _cameraBounds;

    private void Start()
    {
        _cameraBounds = Camera.main.OrthographicBounds();


        int rows = Random.Range(_rowsBounds.MIN, _rowsBounds.MAX + 1);
        int columns = Random.Range(_columnsBounds.MIN, _columnsBounds.MAX + 1);
        _gameBoard = new GameBoard(rows, columns, _cameraBounds.min);
        _gameBoard.CreateBoard(_bgPrefab,_tilePrefab);
    }
}