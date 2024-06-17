using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public record CatProperty(int Id, string CharacterId, bool IsEnemy, int Position, int HP, int ATK, int Speed, int Range);

	public class CatView : MonoBehaviour
	{
		[SerializeField]
		private Transform _character;
		[SerializeField]
		private Animator _animator;
		[SerializeField]
		private Text _health;

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
			
			_character.localScale = new Vector2(prop.IsEnemy ? 100 : -100, 100);
			transform.localPosition = new Vector2((prop.Position - GameplayUtility.Length / 2) * 1500f / GameplayUtility.Length, 0);
			_health.text = prop.HP.ToString();
		}
	}
}
