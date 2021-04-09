using System.Collections.Generic;
using Models;
using UnityEngine;
using Utils;

public class Tile : MonoBehaviour
{
    public Constants.TileType TileType => _tileData.TileType;
    public bool hasGroup;

    [SerializeField] private SpriteRenderer _tile;

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

    public void UpdatePosition(int? x = null, int? y = null)
    {
        _pos.x = x ?? _pos.x;
        _pos.y = y ?? _pos.y;
    }
    
}