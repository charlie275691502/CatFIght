using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay;

namespace Retry
{
	public record RetrySubTabReturnType()
	{
		public record Close() : RetrySubTabReturnType;
	}

	public abstract record RetryState
	{
		public record Open() : RetryState;
		public record Idle() : RetryState;
		public record OnClickClose() : RetryState;
		public record Close() : RetryState;
	}

	public record RetryProperty(RetryState State);
	public record RetrySubTabReturn(RetrySubTabReturnType Type);

	public interface IRetryPresenter
	{
		UniTask<CardProperty> Run();
	}

	public class RetryPresenter : IRetryPresenter
	{
		private IRetryView _view;

		private RetryProperty _prop;
		
		private Dictionary<CardType, float> _pool = new Dictionary<CardType, float>()
		{
			{CardType.Archer, 1},
			{CardType.Warrior, 3},
			{CardType.Mage, 2},
		};

		public RetryPresenter(IRetryView view)
		{
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new RetryState.OnClickClose()));
		}

		async UniTask<CardProperty> IRetryPresenter.Run()
		{
			_prop = new RetryProperty(new RetryState.Open());
			var ret = new CardProperty(CardType.Archer);

			while (_prop.State is not RetryState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case RetryState.Open:
						_prop = _prop with { State = new RetryState.Idle() };
						break;

					case RetryState.Idle:
						break;

					case RetryState.OnClickClose:
						_prop = _prop with { State = new RetryState.Close() };
						break;

					case RetryState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(RetryState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not RetryState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}