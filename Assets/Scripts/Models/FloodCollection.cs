using System.Collections.Generic;
using UnityEngine;

namespace Models
{
	public class FloodCollection<T>
	{
		private Dictionary<Vector2, List<T>> _floods;

		public FloodCollection()
		{
			_floods = new Dictionary<Vector2, List<T>>();
		}

		public void Add(Vector2 index)
		{
			
		}
	}
}