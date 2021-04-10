using Models;
using UnityEngine;

namespace Utils
{
	public static class Constants
	{
		public static readonly MinMax<Vector2> BoardWidth = new MinMax<Vector2>(new Vector2(.4f, .4f), Vector2.one);
		public static readonly MinMax<Vector2> BoardHeight = new MinMax<Vector2>(new Vector2(.6f, .6f), Vector2.one);

		public static readonly Vector2 TileMargin = new Vector2(1.1f, 1.1f);
		public static readonly Vector2 TilePadding = new Vector2(1, 1);

		public enum TileType
		{
			Red,
			Green,
			Blue,
			Pink,
			Purple,
			Yellow
		}

		public enum TileState
		{
			Default,
			Off,
			A,
			B,
			C
		}
	}
}