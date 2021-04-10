using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base
{
	public class ObjectPool : IDisposable
	{
		public static readonly Dictionary<string, ObjectPool> Pools = new Dictionary<string, ObjectPool>();

		public int Count => _pool.Count;

		private readonly Queue<GameObject> _pool;

		private ObjectPool()
		{
			_pool = new Queue<GameObject>();
		}

		public static ObjectPool CreatePool(string name, int size, GameObject baseObject, Transform parent = null)
		{
			Pools[name] = new ObjectPool();

			for (int i = 0; i < size; i++)
			{
				var go = parent != null ? Object.Instantiate(baseObject, parent) : Object.Instantiate(baseObject);


				go.SetActive(false);
				go.name = name + i;
				Pools[name]._pool.Enqueue(go);
			}

			return Pools[name];
		}

		public GameObject GetPoolObject()
		{
			GameObject go = _pool.Dequeue();
			go.SetActive(true);

			_pool.Enqueue(go);

			return go;
		}

		public void Dispose()
		{
			foreach (var objectPool in Pools)
			{
				foreach (var obj in objectPool.Value._pool)
				{
					Object.Destroy(obj);
				}
			}
		}
	}
}