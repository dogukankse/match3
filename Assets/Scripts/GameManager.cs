using Models;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private TouchController _touchController;

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

	private void Start()
	{
		int rows = Random.Range(_rowsBounds.MIN, _rowsBounds.MAX + 1);
		int columns = Random.Range(_columnsBounds.MIN, _columnsBounds.MAX + 1);
		SetRules();

		_gameBoard = new GameBoard(rows, columns);

		_touchController.OnClick = _gameBoard.Blast;
		_gameBoard.AllowUserInput = _touchController.AllowUserInput;
		_gameBoard.BlockUserInput = _touchController.BlockUserInput;

		_gameBoard.CreateBoard();
	}

	private void SetRules()
	{
		Rules.colorCount = _colorVariantCount;
		Rules.minBlastCount = _minBlastGroupCount;
		Rules.defaultMinMax = _groupDefault;
		Rules.aMinMax = _groupA;
		Rules.bMinMax = _groupB;
		Rules.cMinMax = _groupC;
		Rules.sizeMax = new Vector2Int(_columnsBounds.MAX, _rowsBounds.MAX);
	}
}