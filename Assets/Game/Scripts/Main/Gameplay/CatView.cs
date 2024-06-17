using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public record CatProperty(int Id, string CharacterId, bool IsEnemy, int Position, int HP);

	public class CatView : MonoBehaviour
	{
		public const int Length = 30;
		
		[SerializeField]
		private Transform _character;
		[SerializeField]
		private Text _health;

		private CatProperty _prop;

		public void Render(CatProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(CatProperty prop)
		{
			_character.localScale = new Vector2((prop.IsEnemy ? -100 : 100), 100);
			transform.localPosition = new Vector2((prop.Position - Length / 2) * 500f / 15f, 0);
			_health.text = prop.HP.ToString();
		}
	}
}
