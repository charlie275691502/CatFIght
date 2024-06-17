using System;
using Cysharp.Threading.Tasks;

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
		public record Confirm() : SummaryState;
		public record Close() : SummaryState;
	}

	public record SummaryProperty(SummaryState State);
	public record SummarySubTabReturn(SummarySubTabReturnType Type);

	public interface ISummaryPresenter
	{
		UniTask<SummarySubTabReturn> Run();
	}

	public class SummaryPresenter : ISummaryPresenter
	{
		private ISummaryView _view;

		private SummaryProperty _prop;

		public SummaryPresenter(ISummaryView view)
		{
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new SummaryState.Confirm()));
		}

		async UniTask<SummarySubTabReturn> ISummaryPresenter.Run()
		{
			_prop = new SummaryProperty(new SummaryState.Open());
			var ret = new SummarySubTabReturn(new SummarySubTabReturnType.Close());

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

					case SummaryState.Confirm Info:
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
	}
}