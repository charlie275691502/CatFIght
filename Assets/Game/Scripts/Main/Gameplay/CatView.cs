using System;
using System.Linq;
using Common.LinqExtension;
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
		private CatKeyPair[] _keys;
		[SerializeField]
		private Color _hitColor;
		[SerializeField]
		private Color _normalColor;
		[SerializeField]
		private SpriteRenderer[] _images;
		[SerializeField]
		private GameObject _catHealthBarGmo;
		[SerializeField]
		private SpriteRenderer _catHealthBar;
		[SerializeField]
		private Sprite[] _catHealthImages;
		[SerializeField]
		private GameObject _towerHealthBarGmo;
		[SerializeField]
		private SpriteRenderer _towerHealthBar;
		[SerializeField]
		private Sprite[] _towerHealthImages;

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
			transform.localPosition = new Vector2(GameplayUtility.GetWorldPosition(prop.Position), 0);
			
			_catHealthBarGmo.SetActive(prop.CatType != CatType.Tower);
			_towerHealthBarGmo.SetActive(prop.CatType == CatType.Tower);
			_catHealthBar.sprite = _catHealthImages[Mathf.Clamp(prop.HP, 0, _catHealthImages.Count() - 1)];
			_towerHealthBar.sprite = _towerHealthImages[Mathf.Clamp(prop.HP, 0, _towerHealthImages.Count() - 1)];
			
			foreach(var key in _keys)
			{
				key.GameObject.SetActive(key.CatType == prop.CatType && key.IsAttacking == prop.IsAttacking);
			}
		}

		public void HitColor()
		{
			foreach( var image in _images)
				image.color = _hitColor;
		}

		public void NormalColor()
		{
			foreach( var image in _images)
				image.color = _normalColor;
		}
	}
}
