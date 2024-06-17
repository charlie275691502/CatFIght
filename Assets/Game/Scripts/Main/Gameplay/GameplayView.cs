using System;
using Battle;
using Deck;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public interface IGameplayView
	{
		void RegisterCallback(IBattleView battleView, IDeckView deckView, Action<int> onClickCard);
		void Render(GameplayProperty prop);
	}

	public class GameplayView : MonoBehaviour, IGameplayView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private GameplayProperty _prop;
		private IBattleView _battleView;
		private IDeckView _deckView;

		void IGameplayView.RegisterCallback(IBattleView battleView, IDeckView deckView, Action<int> onClickCard)
		{
			_battleView = battleView;
			_deckView = deckView;
			
			_battleView.RegisterCallback();
			_deckView.RegisterCallback(onClickCard);
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
				case GameplayState.OnClickCard:
				case GameplayState.Summary:
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
			_battleView.Render(prop);
			_deckView.Render(prop);
		}
	}
}
