using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay;

namespace HoldingCards
{
	public record HoldingCardsSubTabReturnType()
	{
		public record Close() : HoldingCardsSubTabReturnType;
	}

	public abstract record HoldingCardsState
	{
		public record Open() : HoldingCardsState;
		public record Idle() : HoldingCardsState;
		public record OnClickCard() : HoldingCardsState;
		public record Close() : HoldingCardsState;
	}

	public record HoldingCardsProperty(HoldingCardsState State, List<CardProperty> Cards);
	public record HoldingCardsSubTabReturn(HoldingCardsSubTabReturnType Type);

	public interface IHoldingCardsPresenter
	{
		UniTask<CardProperty> Run(List<CardProperty> cards);
	}

	public class HoldingCardsPresenter : IHoldingCardsPresenter
	{
		private IHoldingCardsView _view;

		private HoldingCardsProperty _prop;

		public HoldingCardsPresenter(IHoldingCardsView view)
		{
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new HoldingCardsState.OnClickCard()));
		}

		async UniTask<CardProperty> IHoldingCardsPresenter.Run(List<CardProperty> cards)
		{
			_prop = new HoldingCardsProperty(new HoldingCardsState.Open(), cards);
			var ret = new CardProperty(CardType.Archer);

			while (_prop.State is not HoldingCardsState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case HoldingCardsState.Open:
						_prop = _prop with { State = new HoldingCardsState.Idle() };
						break;

					case HoldingCardsState.Idle:
						break;

					case HoldingCardsState.OnClickCard:
						_prop = _prop with { State = new HoldingCardsState.Close() };
						break;

					case HoldingCardsState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(HoldingCardsState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not HoldingCardsState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}