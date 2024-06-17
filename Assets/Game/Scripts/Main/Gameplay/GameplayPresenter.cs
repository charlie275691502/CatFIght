using System;
using System.Collections.Generic;
using System.Diagnostics;
using Battle;
using Codice.CM.Client.Differences.Merge;
using Cysharp.Threading.Tasks;
using Deck;
using Summary;

namespace Gameplay
{
	public record GameplaySubTabReturnType()
	{
		public record Close() : GameplaySubTabReturnType;
	}

	public abstract record GameplayState
	{
		public record Open() : GameplayState;
		public record Idle() : GameplayState;
		public record Confirm() : GameplayState;
		public record Summary() : GameplayState;
		public record Close() : GameplayState;
	}

	public record GameplayProperty(GameplayState State, List<CatProperty> Cats);
	public record GameplaySubTabReturn(GameplaySubTabReturnType Type);
	
	public interface IGameplayPresenter
	{
		UniTask Run();
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private IGameplayView _view;

		private GameplayProperty _prop;
		private ISummaryPresenter _summaryPresenter;

		public GameplayPresenter(IGameplayView view, IBattleView battleView, IDeckView deckView, ISummaryPresenter summaryPresenter)
		{
			_view = view;
			_summaryPresenter = summaryPresenter;

			_view.RegisterCallback(
				battleView,
				deckView,
				() =>
					_ChangeStateIfIdle(new GameplayState.Confirm()));
		}

		async UniTask IGameplayPresenter.Run()
		{
			_prop = new GameplayProperty(new GameplayState.Open(), new List<CatProperty>(){ new CatProperty(1, "Cat", false, 13, 39), new CatProperty(2, "Cat", true, 17, 39) });
			
			while (_prop.State is not GameplayState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case GameplayState.Open:
						_prop = _prop with { State = new GameplayState.Idle() };
						break;

					case GameplayState.Idle:
						break;

					case GameplayState.Confirm info:
						_prop = _prop with { State = new GameplayState.Close() };
						break;
						
					case GameplayState.Summary:
						await _summaryPresenter.Run();
						break;

					case GameplayState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
		}

		private void _ChangeStateIfIdle(GameplayState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not GameplayState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}