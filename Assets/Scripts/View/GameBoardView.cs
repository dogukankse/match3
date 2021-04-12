using System.Collections.Generic;
using Base;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Models;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace View
{
	public class GameBoardView
	{
		public UnityAction<Vector2Int> OnTileTurnOff;
		public UnityAction OnBlastCompleted;
		public UnityAction OnShuffleTilesCompleted;
		public UnityAction OnTileFallCompleted;
		public UnityAction OnCreateTileCompleted;

		private readonly int _rows;
		private readonly int _columns;
		private readonly GameObject _bgPrefab;
		private readonly Tile _tilePrefab;
		private readonly GameObject[,] _backgroundTiles;

		private GameObject _boardGo;
		private GameObject _backgroundGo;
		private GameObject _tilesGo;


		private ObjectPool _tilePool;

		private readonly List<Tile> _tilesToFall;
		private readonly List<TweenerCore<Vector3, Vector3, VectorOptions>> _tilesToCreate;


		public GameBoardView(int rows, int columns)
		{
			_rows = rows;
			_columns = columns;

			_backgroundTiles = new GameObject[_columns, _rows];
			_tilesToFall = new List<Tile>();
			_tilesToCreate = new List<TweenerCore<Vector3, Vector3, VectorOptions>>();

			_bgPrefab = Resources.Load<GameObject>("Prefabs/BackgroundTile");
			_tilePrefab = Resources.Load<Tile>("Prefabs/Tile");

			CreateGameObjects();
		}

		private void CreateGameObjects()
		{
			_boardGo = new GameObject("Board");
			_backgroundGo = new GameObject("Background");
			_tilesGo = new GameObject("Tiles");
			_backgroundGo.transform.parent = _boardGo.transform;
			_tilesGo.transform.parent = _boardGo.transform;
		}

		public void CalculateBoardSize()
		{
			if (_columns >= 5)
			{
				_boardGo.transform.localScale =
					Vector2.Lerp(Constants.BoardWidth.MAX, Constants.BoardWidth.MIN, _columns / (float) Rules.sizeMax.x);
			}

			if (_rows >= 8)
			{
				Vector2 scale =
					Vector2.Lerp(Constants.BoardHeight.MAX, Constants.BoardHeight.MIN, _columns / (float) Rules.sizeMax.y);
				if (_boardGo.transform.localScale.x >= scale.x)
					_boardGo.transform.localScale = scale;
			}

			if (Screen.height / (float)Screen.width > 16f / 9f)
			{
				_boardGo.transform.localScale -= new Vector3(.1f, .1f, 0);
			}
		}

		public void CreateBoard()
		{
			_tilePool = ObjectPool.CreatePool("tile", _rows * _columns, _tilePrefab.gameObject, _tilesGo.transform);
		}

		public void CreateBackgroundTile(int x, int y)
		{
			GameObject go = Object.Instantiate(_bgPrefab, _backgroundGo.transform);
			go.transform.localPosition = new Vector3(x, y, 0);
			go.name = $"({x}, {y})";
			_backgroundTiles[x, y] = go;
		}


		public void AdjustView()
		{
			//move to centre
			//Vector2 lastPos = new Vector2(_columns - 1, _rows - 1);
			Vector2 lastPos = _backgroundTiles[_columns - 1, _rows - 1].transform.position;
			_boardGo.transform.position = new Vector2(lastPos.x / -2f, lastPos.y / -2f);
		}

		public void CreateTileAnim(float delay = 0)
		{
			Sequence seq = DOTween.Sequence();
			seq.PrependInterval(delay);
			foreach (var tween in _tilesToCreate)
			{
				seq.Insert(0, tween);
			}

			_tilesToCreate.Clear();
			seq.OnComplete(() => OnCreateTileCompleted());
		}


		public Tile CreateTile()
		{
			Tile tile = _tilePool.Pop().GetComponent<Tile>();
			tile.transform.localScale = Vector3.zero;
			_tilesToCreate.Add(tile.transform.DOScale(Vector3.one, .2f));
			return tile;
		}

		public void Blast(Tile tile, bool isParent = false)
		{
			Sequence seq = DOTween.Sequence();

			if (isParent)
			{
				foreach (var child in tile.Flood)
				{
					seq.Insert(0,
						child.transform.DOMove(tile.transform.position, .5f)
							.OnComplete(() =>
							{
								//Object.Destroy(child.gameObject);
								child.TurnOff();
								ObjectPool.Pools["tile"].Push(child.gameObject);
								OnTileTurnOff?.Invoke(child.Pos);
							}));
				}
			}
			else
			{
				foreach (var child in tile.Parent.Flood)
				{
					seq.Insert(0,
						child.transform.DOMove(tile.transform.position, .5f)
							.OnComplete(() =>
							{
								//Object.Destroy(child.gameObject);
								child.TurnOff();
								ObjectPool.Pools["tile"].Push(child.gameObject);
								OnTileTurnOff?.Invoke(child.Pos);
							}));
				}
			}

			seq.OnComplete(() => OnBlastCompleted?.Invoke());
		}

		public void FallTiles()
		{
			Sequence seq = DOTween.Sequence();
			foreach (var fallTile in _tilesToFall)
			{
				seq.Insert(0,
					fallTile.gameObject.transform.DOLocalMoveY(fallTile.Pos.y, .2f));
			}

			_tilesToFall.Clear();

			seq.OnComplete(() => OnTileFallCompleted?.Invoke());
		}

		public void ShuffleTiles(Tile[,] tiles)
		{
			Sequence seq = DOTween.Sequence();
			for (int y = 0; y < _rows; y++)
			{
				for (int x = 0; x < _columns; x++)
				{
					seq.Insert(0,
						tiles[x, y].transform.DOLocalMove(new Vector3(tiles[x, y].Pos.x, tiles[x, y].Pos.y, 0), .2f));
				}
			}

			seq.AppendInterval(.2f);
			seq.OnComplete(() => OnShuffleTilesCompleted?.Invoke());
		}


		public void AddTileToFall(Tile tile)
		{
			_tilesToFall.Add(tile);
		}
	}
}