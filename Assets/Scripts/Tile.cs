using System.Collections.Generic;
using Models;
using UnityEngine;
using Utils;

public class Tile : MonoBehaviour
{
	public Constants.TileType TileType => _tileData.TileType;
	public List<Tile> Flood => _flood;
	public Tile Parent => _parent;
	public Vector2Int Pos => _pos;
	public bool hasGroup;


	private SpriteRenderer _tile;
	private Vector2Int _pos;
	private TileSo _tileData;
	private Constants.TileState _tileState = Constants.TileState.Default;
	private Tile _parent;
	private List<Tile> _flood;


	public void SetData(TileSo tileData, Vector2Int position)
	{
		_tileData = tileData;
		_tile.sprite = _tileData.Default;
		_pos = position;
		transform.localPosition = (Vector2) Pos;
		_flood = new List<Tile>();
	}


	public void UpdateState(Constants.TileState state)
	{
		_tileState = state;
		Sprite sprite;
		switch (_tileState)
		{
			default:
			case Constants.TileState.Off:
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
		_parent = null;
		_flood.Clear();
		UpdateState(Constants.TileState.Default);
	}

	public void AddParent(Tile tile)
	{
		if (GetInstanceID() == tile.GetInstanceID()) return;

		_parent = tile;
	}

	public void TurnOff()
	{
		_tileState = Constants.TileState.Off;
		gameObject.SetActive(false);
	}

	public void UpdatePos(int x,int y)
	{
		_pos = new Vector2Int(x,y);
	}

	private void Awake()
	{
		_tile = GetComponent<SpriteRenderer>();
	}


}