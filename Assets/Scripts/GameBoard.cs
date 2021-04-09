using System.Collections.Generic;
using System.Linq;
using Models;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class GameBoard
{
    private int _rows;
    private int _columns;
    private GameObject[,] _backgroundTiles;
    private Tile[,] _tiles;

    private GameObject _boardGo;
    private GameObject _backgrounfGo;
    private GameObject _tilesGo;
    private Vector2 _origin;

    private TilesSo _tilesSo;

    private List<Tile> _flood;

    public GameBoard(int rows, int columns, Vector2 origin)
    {
        _rows = rows;
        _columns = columns;
        _backgroundTiles = new GameObject[_rows, _columns];
        _tiles = new Tile[_rows, _columns];

        _origin = origin;
        _tilesSo = Resources.Load<TilesSo>("Tiles");

        CreateGameObjects();
    }


    public void CreateBoard(SpriteRenderer bgPrefab, Tile tilePrefab)
    {
        GameObject bgGo = bgPrefab.gameObject;
        GameObject tileGo = tilePrefab.gameObject;

        CalculateBoardSize();

        for (int y = 0; y < _columns; y++)
        {
            for (int x = 0; x < _rows; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                //pos *= _margin;
                //pos += _padding;
                //pos += _origin;
                GameObject go = Object.Instantiate(bgGo, _backgrounfGo.transform);
                go.transform.localPosition = (Vector2) pos;
                go.name = $"({x}, {y})";
                _backgroundTiles[x, y] = go;

                Tile tile = Object.Instantiate(tileGo, _tilesGo.transform).GetComponent<Tile>();
                tile.SetData(_tilesSo.Tiles.GetRandom(), pos);
                tile.name = $"({x}, {y})";
                _tiles[x, y] = tile;
            }
        }

        //move to centre
        Vector2 lastPos = _backgroundTiles[_rows - 1, _columns - 1].transform.position;
        _boardGo.transform.position = new Vector2(lastPos.x / -2f, lastPos.y / -2f);

        _flood = new List<Tile>();
        for (int y = 0; y < _columns; y++)
        {
            for (int x = 0; x < _rows; x++)
            {
                //var a = FloodFill(0, 0, _tiles[0, 0].TileType, null);
                Debug.Log(_flood.Count);
            }
        }
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


    private Flood FloodFill(int x, int y, Constants.TileType type, Flood flood)
    {
        if (!IsValid(x, y)) return flood;

        Tile tile = _tiles[x, y];
        if (tile.TileType != type || tile.hasGroup) return flood;

        tile.hasGroup = true;
        _flood.Add(tile);
        FloodFill(y + 1, x, tile.TileType, flood); //  up
        FloodFill(y, x - 1, tile.TileType, flood); //  left 
        FloodFill(y, x + 1, tile.TileType, flood); //  right
        FloodFill(y - 1, x, tile.TileType, flood); //  down 

        return flood;
    }


    private bool IsValid(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < _rows && y < _columns);
    }
}