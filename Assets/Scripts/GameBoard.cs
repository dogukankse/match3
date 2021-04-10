using Base;
using DG.Tweening;
using Models;
using UnityEngine;
using Utils;

public class GameBoard
{
	private readonly int _rows;
	private readonly int _columns;
	private readonly GameObject[,] _backgroundTiles;
	private readonly Tile[,] _tiles;
	private readonly TilesSo _tilesSo;


	private GameObject _boardGo;
	private GameObject _backgrounfGo;
	private GameObject _tilesGo;

	private ObjectPool _tilePool;


	public GameBoard(int rows, int columns)
	{
		_rows = rows;
		_columns = columns;
		_backgroundTiles = new GameObject[_columns, _rows];
		_tiles = new Tile[_columns, _rows];
		_tilesSo = Resources.Load<TilesSo>("Tiles");

		CreateGameObjects();
	}


	public void CreateBoard(SpriteRenderer bgPrefab, Tile tilePrefab)
	{
		GameObject bgGo = bgPrefab.gameObject;
		GameObject tileGo = tilePrefab.gameObject;

		CalculateBoardSize();

		_tilePool = ObjectPool.CreatePool("tile", _rows * _columns, tileGo, _tilesGo.transform);

		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				Vector2Int pos = new Vector2Int(x, y);
				GameObject go = Object.Instantiate(bgGo, _backgrounfGo.transform);
				go.transform.localPosition = (Vector2) pos;
				go.name = $"({x}, {y})";
				_backgroundTiles[x, y] = go;


				//Tile tile = Object.Instantiate(tileGo, _tilesGo.transform).GetComponent<Tile>();
				Tile tile = _tilePool.GetPoolObject().GetComponent<Tile>();
				int index = Random.Range(0, Rules.colorCount.MAX);
				tile.SetData(_tilesSo.Tiles[index], pos);
				tile.name = $"({x}, {y})";
				_tiles[x, y] = tile;
			}
		}

		//move to centre
		Vector2 lastPos = _backgroundTiles[_columns - 1, _rows - 1].transform.position;
		_boardGo.transform.position = new Vector2(lastPos.x / -2f, lastPos.y / -2f);


		MatchTiles();
	}

	private void MatchTiles()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				FloodFill(x, y, _tiles[x, y]);
			}
		}

		Debug.Log("Tiles matched");


		UpdateTileState();
	}

	private void CreateGameObjects()
	{
		_boardGo = new GameObject("Board");
		_backgrounfGo = new GameObject("Background");
		_tilesGo = new GameObject("Tiles");
		_backgrounfGo.transform.parent = _boardGo.transform;
		_tilesGo.transform.parent = _boardGo.transform;
	}

	private void CalculateBoardSize()
	{
		if (_rows >= 5)
		{
			_boardGo.transform.localScale =
				Vector2.Lerp(Constants.BoardWidth.MAX, Constants.BoardWidth.MIN, _rows / 10f);
		}
		else if (_columns >= 8)
		{
			Vector2 scale =
				Vector2.Lerp(Constants.BoardHeight.MAX, Constants.BoardHeight.MIN, _columns / 10f);
			if (_boardGo.transform.localScale.x >= scale.x && _boardGo.transform.localScale.x >= scale.y)
				_boardGo.transform.localScale = scale;
		}
	}

	public void Blast(Tile tile)
	{
		if (tile.flood.Count >= Rules.minBlastCount)
		{
			CollectOnTile(tile, true);
		}
		else if (tile.parent && tile.parent.flood.Count >= Rules.minBlastCount)
		{
			CollectOnTile(tile);
		}
	}

	private void FloodFill(int x, int y, Tile tile)
	{
		//base state
		if (!IsValid(x, y) || _tiles[x, y] == null || _tiles[x, y].TileType != tile.TileType || _tiles[x, y].hasGroup)
			return;

		_tiles[x, y].hasGroup = true;
		tile.flood.Add(_tiles[x, y]);
		_tiles[x, y].AddParent(tile);

		FloodFill(x + 1, y, tile);
		FloodFill(x - 1, y, tile);
		FloodFill(x, y + 1, tile);
		FloodFill(x, y - 1, tile);
	}

	private bool IsValid(int x, int y)
	{
		return (x >= 0 && y >= 0 && x < _columns && y < _rows);
	}

	private void UpdateTileState()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				if (_tiles[x, y] == null) continue;

				Tile parent = _tiles[x, y];
				int count = parent.flood.Count;

				if (count > Rules.defaultMinMax.MAX)
				{
					Constants.TileState state = Constants.TileState.Default;
					if (count >= Rules.aMinMax.MIN && count <= Rules.aMinMax.MAX)
						state = Constants.TileState.A;
					if (count >= Rules.bMinMax.MIN && count <= Rules.bMinMax.MAX)
						state = Constants.TileState.B;
					if (count >= Rules.cMinMax.MIN && count <= Rules.cMinMax.MAX)
						state = Constants.TileState.C;

					foreach (var tile in parent.flood)
					{
						tile.UpdateState(state);
					}
				}
			}
		}

		Debug.Log("States updated");
	}

	private void CollectOnTile(Tile tile, bool isParent = false)
	{
		Sequence seq = DOTween.Sequence();

		if (isParent)
		{
			foreach (var child in tile.flood)
			{
				seq.Insert(0,
					child.transform.DOMove(tile.transform.position, .5f)
						.OnComplete(() =>
						{
							Object.Destroy(child.gameObject);
							_tiles[child.Pos.x, child.Pos.y] = null;
						}));
				seq.OnComplete(Collapse);
			}
		}
		else
		{
			foreach (var child in tile.parent.flood)
			{
				seq.Insert(0,
					child.transform.DOMove(tile.transform.position, .5f)
						.OnComplete(() =>
						{
							Object.Destroy(child.gameObject);
							_tiles[child.Pos.x, child.Pos.y] = null;
						}));
				seq.OnComplete(Collapse);
			}
		}
	}

	private void Collapse()
	{
		int emptyCount = 0;
		for (int x = 0; x < _columns; x++)
		{
			for (int y = 0; y < _rows; y++)
			{
				Tile tile = _tiles[x, y];
				if (tile == null)
				{
					emptyCount++;
				}
				else if (emptyCount > 0)
				{
					tile.Fall(emptyCount);
					_tiles[x, y] = null;
					_tiles[tile.Pos.x, tile.Pos.y] = tile;
				}
			}

			emptyCount = 0;
		}

		ClearTiles();
		MatchTiles();
	}

	private void ClearTiles()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				if (_tiles[x, y] != null)
					_tiles[x, y].Clear();
			}
		}
	}
}