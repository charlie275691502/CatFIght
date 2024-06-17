using System;
using UnityEngine;
using UnityEngine.UI;

namespace Deck
{
	public interface IDeckView
	{
		void RegisterCallback(Action onConfirm);
		void Render(DeckProperty prop);
	}
	
	public record DeckProperty();

	public class DeckView : MonoBehaviour, IDeckView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private DeckProperty _prop;

		void IDeckView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void IDeckView.Render(DeckProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(DeckProperty prop)
		{
			
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
