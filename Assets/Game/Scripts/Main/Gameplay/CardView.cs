using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public enum CardType
	{
		Archer,
		Warrior,
		Mage,
		Tank,
		Gun,
		Wizard,
		DoubleDamage,
		MaxHealth,
		FireCard,
		FreezingCard,
	}
	
	public record CardProperty(CardType CardType);

	public class CardView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _cardTitle;
		[SerializeField]
		private Image _cardImage;
		[SerializeField]
		private Button _button;

		private CardProperty _prop;
		
		public void Show(Action onClick)
		{
			_panel.SetActive(true);
			_button.onClick.RemoveAllListeners();
			_button.onClick.AddListener(() => onClick?.Invoke());
		}
		
		public void Hide()
		{
			_panel.SetActive(false);
		}

		public void Render(CardProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(CardProperty prop)
		{
			_cardTitle.text = prop.CardType.ToString();
		}
	}
}
