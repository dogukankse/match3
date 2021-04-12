using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base
{
	public class ObjectPool : IDisposable
	{
		public static readonly Dictionary<string, ObjectPool> Pools = new Dictionary<string, ObjectPool>();

		public int Count => _passivePool.Count;

		private readonly List<GameObject> _passivePool;
		private readonly List<GameObject> _activePool;

		private ObjectPool()
		{
			_passivePool = new List<GameObject>();
			_activePool = new List<GameObject>();
		}

		public static ObjectPool CreatePool(string name, int size, GameObject baseObject, Transform parent = null)
		{
			Pools[name] = new ObjectPool();

			for (int i = 0; i < size; i++)
			{
				var go = parent != null ? Object.Instantiate(baseObject, parent) : Object.Instantiate(baseObject);

				go.name = name + i;
				Pools[name]._passivePool.Add(go);
			}

			return Pools[name];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Active game object</returns>
		public GameObject Pop()
		{
			int lastIndex = _passivePool.Count - 1;
			GameObject go = _passivePool[lastIndex];
			go.SetActive(true);

			_activePool.Add(go);
			_passivePool.RemoveAt(lastIndex);

			return go;
		}

		/// <summary>
		/// Add passive game object. If object not passive throws error
		/// </summary>
		public void Push(GameObject go)
		{
			if (go.activeSelf) throw new Exception("GameObject is active");
			for (int i = 0; i < _activePool.Count; i++)
			{
				if (_activePool[i].GetInstanceID() == go.GetInstanceID())
					_activePool.RemoveAt(i);
			}

			_passivePool.Add(go);
		}

		public void Dispose()
		{
			foreach (var objectPool in Pools)
			{
				foreach (var obj in objectPool.Value._passivePool)
				{
					Object.Destroy(obj);
				}

				foreach (var obj in objectPool.Value._activePool)
				{
					Object.Destroy(obj);
				}
			}
		}
	}
}