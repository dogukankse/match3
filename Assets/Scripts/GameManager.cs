using System.Data;
using DG.Tweening;
using Models;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
	[SerializeField] private TouchController _touchController;

	[SerializeField] private SpriteRenderer _bgPrefab;
	[SerializeField] private Tile _tilePrefab;

	[Header("Game Rules")] [SerializeField]
	private int _minBlastGroupCount = 2;

	[SerializeField] private MinMax<int> _groupDefault;
	[SerializeField] private MinMax<int> _groupA;
	[SerializeField] private MinMax<int> _groupB;
	[SerializeField] private MinMax<int> _groupC;
	[SerializeField] private MinMax<int> _colorVariantCount;

	[Space] [Header("Bounds")] [SerializeField]
	private MinMax<int> _rowsBounds;

	[SerializeField] private MinMax<int> _columnsBounds;

	private GameBoard _gameBoard;
	private Bounds _cameraBounds;

	private void Start()
	{

		_cameraBounds = Camera.main.OrthographicBounds();
		
		SetRules();
		int rows = Random.Range(_rowsBounds.MIN, _rowsBounds.MAX + 1);
		int columns = Random.Range(_columnsBounds.MIN, _columnsBounds.MAX + 1);
		_gameBoard = new GameBoard(rows, columns);
		_gameBoard.CreateBoard(_bgPrefab, _tilePrefab);
		_touchController.OnClick = _gameBoard.Blast;
	}

	private void SetRules()
	{
		Rules.colorCount = _colorVariantCount;
		Rules.minBlastCount = _minBlastGroupCount;
		Rules.defaultMinMax = _groupDefault;
		Rules.aMinMax = _groupA;
		Rules.bMinMax = _groupB;
		Rules.cMinMax = _groupC;
	}
}