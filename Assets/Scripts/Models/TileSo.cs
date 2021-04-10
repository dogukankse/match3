using UnityEngine;
using Utils;

namespace Models
{
    [CreateAssetMenu(fileName = "Tile", menuName = "M3/Tile", order = 0)]
    public class TileSo : ScriptableObject
    {
        public Sprite Default => _default;

        public Sprite A => _a;

        public Sprite B => _b;

        public Sprite C => _c;
        public Constants.TileType TileType => _tileType;

        [SerializeField] private Constants.TileType _tileType;
        [SerializeField] private Sprite _a;
        [SerializeField] private Sprite _b;
        [SerializeField] private Sprite _c;
        [SerializeField] private Sprite _default;
    }
}