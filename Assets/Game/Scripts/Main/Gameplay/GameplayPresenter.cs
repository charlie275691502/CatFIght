using System;
using Cysharp.Threading.Tasks;
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

	public record GameplayProperty(GameplayState State);
	public record GameplaySubTabReturn(GameplaySubTabReturnType Type);

	public interface IGameplayPresenter
	{
		UniTask<GameplaySubTabReturn> Run();
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private IGameplayView _view;

		private GameplayProperty _prop;
		private ISummaryPresenter _summaryPresenter;

		public GameplayPresenter(IGameplayView view, ISummaryPresenter summaryPresenter)
		{
			_view = view;
			_summaryPresenter = summaryPresenter;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new GameplayState.Confirm()));
		}

		async UniTask<GameplaySubTabReturn> IGameplayPresenter.Run()
		{
			_prop = new GameplayProperty(new GameplayState.Open());
			var ret = new GameplaySubTabReturn(new GameplaySubTabReturnType.Close());

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
			return ret;
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