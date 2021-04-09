using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Tiles", menuName = "M3/Tiles", order = 0)]
    public class TilesSo : ScriptableObject
    {
        public IList<TileSo> Tiles => _tiles;
        
        [SerializeField] public List<TileSo> _tiles;
        
    }
}