using System;
using Cysharp.Threading.Tasks;

namespace Template
{
	public record TemplateSubTabReturnType()
	{
		public record Close() : TemplateSubTabReturnType;
	}

	public abstract record TemplateState
	{
		public record Open() : TemplateState;
		public record Idle() : TemplateState;
		public record Confirm() : TemplateState;
		public record Close() : TemplateState;
	}

	public record TemplateProperty(TemplateState State);
	public record TemplateSubTabReturn(TemplateSubTabReturnType Type);

	public interface ITemplatePresenter
	{
		UniTask<TemplateSubTabReturn> Run();
	}

	public class TemplatePresenter : ITemplatePresenter
	{
		private ITemplateView _view;

		private TemplateProperty _prop;

		public TemplatePresenter(ITemplateView view)
		{
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new TemplateState.Confirm()));
		}

		async UniTask<TemplateSubTabReturn> ITemplatePresenter.Run()
		{
			_prop = new TemplateProperty(new TemplateState.Open());
			var ret = new TemplateSubTabReturn(new TemplateSubTabReturnType.Close());

			while (_prop.State is not TemplateState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case TemplateState.Open:
						_prop = _prop with { State = new TemplateState.Idle() };
						break;

					case TemplateState.Idle:
						break;

					case TemplateState.Confirm info:
						_prop = _prop with { State = new TemplateState.Close() };
						break;

					case TemplateState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(TemplateState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not TemplateState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}