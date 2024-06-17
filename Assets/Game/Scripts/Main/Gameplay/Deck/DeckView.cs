using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Deck
{
	public interface IDeckView
	{
		void RegisterCallback(Action onConfirm);
		void Render(GameplayProperty prop);
	}
	
	public class DeckView : MonoBehaviour, IDeckView
	{
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private GameplayProperty _prop;

		void IDeckView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void IDeckView.Render(GameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(GameplayProperty prop)
		{
			
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
