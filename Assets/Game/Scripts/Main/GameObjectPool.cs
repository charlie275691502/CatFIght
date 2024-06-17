using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public class GameObjectPool : MonoBehaviour
	{
		[SerializeField]
		private GameObject _prefab;
		
		private Queue<GameObject> _pool = new Queue<GameObject>();
		
		public GameObject GetGameObject()
		{
			var gameObject = 
				(_pool.Count > 0)
					? _pool.Dequeue()
					: GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, transform);
			gameObject.SetActive(true);
			return gameObject;
		}
		
		public void ReturnGameObject(GameObject gameObject)
		{
			gameObject.SetActive(false);
			_pool.Enqueue(gameObject);
		}
	}
}