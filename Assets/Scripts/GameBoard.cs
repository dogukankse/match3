using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using View;

public class GameBoard
{
	public UnityAction OnButtonClicked;
	public UnityAction AllowUserInput;
	public UnityAction BlockUserInput;

	private readonly int _rows;
	private readonly int _columns;
	private readonly TilesSo _tilesSo;

	private readonly GameBoardView _view;

	private Tile[,] _tiles;
	private bool _hasMove; //has move on current board settings
	private bool _hasMatch; //has match on board 

	private Dictionary<Constants.TileType, int> _matchCountsByTypes;


	public GameBoard(int rows, int columns)
	{
		_rows = rows;
		_columns = columns;
		_view = new GameBoardView(_rows, _columns);
		_tiles = new Tile[_columns, _rows];
		_tilesSo = Resources.Load<TilesSo>("Tiles");

		_view.OnTileTurnOff = OnTileTurnOff;
		_view.OnBlastCompleted = OnBlastCompleted;
		_view.OnShuffleTilesCompleted = OnShuffleTilesCompleted;
		_view.OnTileFallCompleted = OnTileFallCompleted;
		_view.OnCreateTileCompleted = OnCreateTileCompleted;
	}


	public void CreateBoard()
	{
		_view.CalculateBoardSize();
		_view.CreateBoard();

		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				_view.CreateBackgroundTile(x, y);
				Tile tile = _view.CreateTile();
				int index = Random.Range(0, Rules.colorCount.MAX);
				tile.SetData(_tilesSo.Tiles[index], new Vector2Int(x, y));
				_tiles[x, y] = tile;
			}
		}

		_view.AdjustView();
		_view.CreateTileAnim();

		MatchTiles();
	}
	
	//if tile has more than "minBlastCount" tile neighbor blast them
	public void Blast(Tile tile)
	{
		if (tile.Flood.Count >= Rules.minBlastCount)
		{
			_view.BlastAnim(tile, true);
		}
		else if (tile.Parent && tile.Parent.Flood.Count >= Rules.minBlastCount)
		{
			_view.BlastAnim(tile);
		}
		else
		{
			AllowUserInput();
		}
	}

	//perform flood fill for all suitable tiles
	private void MatchTiles()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				FloodFill(x, y, _tiles[x, y]);
			}
		}


		UpdateTileState();
	}

	
	private void UpdateTileState()
	{
		_hasMove = false;
		_matchCountsByTypes = new Dictionary<Constants.TileType, int>();
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				Tile parent = _tiles[x, y];
				int count = parent.Flood.Count;
				//current board have move(blast)
				if (count >= Rules.minBlastCount)
				{
					_hasMove = true;
				}

				
				if (_matchCountsByTypes.ContainsKey(parent.TileType)) _matchCountsByTypes[parent.TileType] += 1;
				else _matchCountsByTypes[parent.TileType] = 1;

				if (count > Rules.defaultMinMax.MAX)
				{
					Constants.TileState state = Constants.TileState.Default;
					if (count >= Rules.aMinMax.MIN && count <= Rules.aMinMax.MAX)
						state = Constants.TileState.A;
					if (count >= Rules.bMinMax.MIN && count <= Rules.bMinMax.MAX)
						state = Constants.TileState.B;
					if (count >= Rules.cMinMax.MIN && count <= Rules.cMinMax.MAX)
						state = Constants.TileState.C;

					foreach (var tile in parent.Flood)
					{
						tile.UpdateState(state);
					}
				}
			}
		}

		_hasMatch = false;

		//Check if the board has a "minBlastCount" pair if not no more move, end game
		foreach (var count in _matchCountsByTypes.Values)
		{
			if (count >= Rules.minBlastCount)
			{
				_hasMatch = true;
			}
		}

		//current board has "minBlastCount" but not in this sequence
		if (!_hasMove)
			Shuffle();
	}

	private void FloodFill(int x, int y, Tile tile)
	{
		if (!IsValid(x, y) || _tiles[x, y].TileType != tile.TileType || _tiles[x, y].hasGroup)
			return;

		_tiles[x, y].hasGroup = true;
		tile.Flood.Add(_tiles[x, y]);
		_tiles[x, y].AddParent(tile);

		FloodFill(x + 1, y, tile);
		FloodFill(x - 1, y, tile);
		FloodFill(x, y + 1, tile);
		FloodFill(x, y - 1, tile);
	}

	private bool IsValid(int x, int y)
	{
		return x >= 0 && y >= 0 && x < _columns && y < _rows;
	}

	//count each tiles fall count
	private void FallTiles()
	{
		int fallCount = 0;
		for (int x = 0; x < _columns; x++)
		{
			for (int y = 0; y < _rows; y++)
			{
				Tile tile = _tiles[x, y];
				if (tile == null)
				{
					fallCount++;
				}
				else if (fallCount > 0)
				{
					_view.AddTileToFall(tile);
					_tiles[x, y] = null;
					tile.UpdatePos(tile.Pos.x, tile.Pos.y - fallCount);
					_tiles[tile.Pos.x, tile.Pos.y] = tile;
				}
			}

			fallCount = 0;
		}

		_view.FallAnim();
	}

	//clear tile state
	private void ClearTiles()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				_tiles[x, y].Clear();
			}
		}
	}

	//create tiles for empty spaces
	private void Refill()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _columns; x++)
			{
				if (_tiles[x, y] == null)
				{
					Tile tile = _view.CreateTile();
					int index = Random.Range(0, Rules.colorCount.MAX);
					tile.SetData(_tilesSo.Tiles[index], new Vector2Int(x, y));
					_tiles[x, y] = tile;
				}
			}
		}

		_view.CreateTileAnim();
	}

	private void Shuffle()
	{
		_tiles = _tiles.Shuffle();
		for (int x = 0; x < _columns; x++)
		{
			for (int y = 0; y < _rows; y++)
			{
				Tile tile = _tiles[x, y];
				tile.Clear();
				tile.UpdatePos(x, y);
			}
		}

		_view.ShuffleAnim(_tiles);
	}

	#region Events

	private void OnShuffleTilesCompleted()
	{
		MatchTiles();
	}

	private void OnTileFallCompleted()
	{
		Refill();
		ClearTiles();
		MatchTiles();
	}

	private void OnTileTurnOff(Vector2Int pos)
	{
		_tiles[pos.x, pos.y] = null;
	}

	private void OnBlastCompleted()
	{
		FallTiles();
	}

	private void OnCreateTileCompleted()
	{
		if (_hasMove)
		{
			AllowUserInput();
		}

		if (!_hasMatch) //no more match. end game
		{
			CanvasView.Instance.ShowAlert("No more match");
			Time.timeScale = 0;
		}
	}

	#endregion
}