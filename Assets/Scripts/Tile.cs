using System.Collections.Generic;
using DG.Tweening;
using Models;
using UnityEngine;
using Utils;

public class Tile : MonoBehaviour
{
	public Constants.TileType TileType => _tileData.TileType;

	public Constants.TileState TileState => _tileState;

	public Vector2Int Pos => _pos;

	public Tile parent;
	public bool hasGroup;
	public List<Tile> flood;

	private SpriteRenderer _tile;

	private Vector2Int _pos;

	private TileSo _tileData;
	private Constants.TileState _tileState = Constants.TileState.Default;


	public void SetData(TileSo tileData, Vector2Int position)
	{
		_tileData = tileData;
		_tile.sprite = _tileData.Default;
		_pos = position;
		transform.localPosition = (Vector2) _pos;
		//_icon.sprite = _tileData.Icon;
	}

	public void UpdateState(Constants.TileState state)
	{
		_tileState = state;
		Sprite sprite;
		switch (_tileState)
		{
			default:
			case Constants.TileState.Default:
				sprite = _tileData.Default;
				break;
			case Constants.TileState.A:
				sprite = _tileData.A;
				break;
			case Constants.TileState.B:
				sprite = _tileData.B;
				break;
			case Constants.TileState.C:
				sprite = _tileData.C;
				break;
		}

		_tile.sprite = sprite;
	}

	public void Clear()
	{
		hasGroup = false;
		parent = null;
		flood.Clear();
		UpdateState(Constants.TileState.Default);
	}

	public void AddParent(Tile tile)
	{
		if (GetInstanceID() == tile.GetInstanceID()) return;

		parent = tile;
	}

	public void Fall(int fallCount)
	{
		_pos.y -= fallCount;
		transform.DOLocalMoveY(_pos.y, .2f);
	}

	private void Awake()
	{
		_tile = GetComponent<SpriteRenderer>();
	}

	public void TurnOff()
	{
		_tileState = Constants.TileState.Off;
		gameObject.SetActive(false);
	}
}