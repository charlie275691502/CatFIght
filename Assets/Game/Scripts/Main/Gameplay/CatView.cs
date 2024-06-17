using System;
using Cysharp.Threading.Tasks.Triggers;
using Optional.Collections;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public enum CatType
	{
		Tower,
		Archer,
		Warrior,
		Mage,
		Tank,
		Gun,
		Wizard,
	}
	
	[Serializable]
	public class CatKeyPair
	{
		public CatType CatType;
		public bool IsAttacking;
		public GameObject GameObject;
	}
		
	public record CatProperty(int Id, CatType CatType, bool IsEnemy, int Position, int HP, int ATK, int Speed, int Range, bool IsAttacking);

	public class CatView : MonoBehaviour
	{
		[SerializeField]
		private Transform _character;
		[SerializeField]
		private Text _health;
		[SerializeField]
		private Text _name;
		[SerializeField]
		private CatKeyPair[] _keys;

		private CatProperty _prop;
	
		public void Init()
		{
			// _animator.Play();
		}

		public void Render(CatProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(CatProperty prop)
		{
			gameObject.SetActive(true);
			
			_character.localScale = new Vector2(prop.IsEnemy ? -1 : 1, 1);
			transform.localPosition = new Vector2((prop.Position - GameplayUtility.Length / 2) * 1500f / GameplayUtility.Length, 0);
			_health.text = prop.HP.ToString();
			_name.text = prop.CatType.ToString();
			
			foreach(var key in _keys)
			{
				key.GameObject.SetActive(key.CatType == prop.CatType && key.IsAttacking == prop.IsAttacking);
			}
		}
	}
}
