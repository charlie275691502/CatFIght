using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public interface IGameplayView
	{
		void RegisterCallback(Action onConfirm);
		void Render(GameplayProperty prop);
	}

	public class GameplayView : MonoBehaviour, IGameplayView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private GameplayProperty _prop;

		void IGameplayView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void IGameplayView.Render(GameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case GameplayState.Open:
					_Open();
					break;

				case GameplayState.Idle:
				case GameplayState.Confirm:
					_Render(prop);
					break;

				case GameplayState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
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
