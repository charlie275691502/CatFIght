using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay;

namespace Summary
{
	public record SummarySubTabReturnType()
	{
		public record Close() : SummarySubTabReturnType;
	}

	public abstract record SummaryState
	{
		public record Open() : SummaryState;
		public record Idle() : SummaryState;
		public record OnClickCard(int Index) : SummaryState;
		public record Close() : SummaryState;
	}

	public record SummaryProperty(SummaryState State, List<CardProperty> Cards);
	public record SummarySubTabReturn(SummarySubTabReturnType Type);

	public interface ISummaryPresenter
	{
		UniTask<CardProperty> Run();
	}

	public class SummaryPresenter : ISummaryPresenter
	{
		private ISummaryView _view;

		private SummaryProperty _prop;
		
		private Dictionary<CardType, float> _pool = new Dictionary<CardType, float>()
		{
			{CardType.Archer, 2},
			{CardType.Warrior, 2},
			{CardType.Mage, 2},
			{CardType.DoubleDamage, 1},
			{CardType.FireCard, 1},
			{CardType.FreezingCard, 1},
			{CardType.MaxHealth, 1},
			
		};

		public SummaryPresenter(ISummaryView view)
		{
			_view = view;

			_view.RegisterCallback(
				(index) =>
					_ChangeStateIfIdle(new SummaryState.OnClickCard(index)));
		}

		async UniTask<CardProperty> ISummaryPresenter.Run()
		{
			_prop = new SummaryProperty(new SummaryState.Open(), Enumerable.Range(0, 3).Select(_ => _SelectRandomCard()).ToList());
			var ret = new CardProperty(CardType.Archer);

			while (_prop.State is not SummaryState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case SummaryState.Open:
						_prop = _prop with { State = new SummaryState.Idle() };
						break;

					case SummaryState.Idle:
						break;

					case SummaryState.OnClickCard Info:
						ret = _prop.Cards[Info.Index];
						_prop = _prop with { State = new SummaryState.Close() };
						break;

					case SummaryState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(SummaryState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not SummaryState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private CardProperty _SelectRandomCard()
		{
			var totalWeight = _pool.Values.Sum();
			double randomValue = new Random().NextDouble() * totalWeight;

			foreach (var item in _pool)
			{
				if (randomValue < item.Value)
				{
					return new CardProperty(item.Key);
				}
				randomValue -= item.Value;
			}

			return new CardProperty(CardType.Archer);
		}
	}
}