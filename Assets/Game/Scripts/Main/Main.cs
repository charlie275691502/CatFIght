using UnityEngine;
using Cysharp.Threading.Tasks;
using Gameplay;

namespace Main
{
	public record MainState
	{
		public record Gameplay() : MainState;
		public record Close() : MainState;
	}

	public record MainProperty(MainState State);

	public class Main : MonoBehaviour
	{
		private IGameplayPresenter _gameplayPresenter;

		[Zenject.Inject]
		public void Zenject(IGameplayPresenter gameplayPresenter)
		{
			_gameplayPresenter = gameplayPresenter;
		}

		void Start()
		{
			_ = _Main();
		}

		private async UniTask _Main()
		{
			var prop = new MainProperty(new MainState.Gameplay());
			while (prop.State is not MainState.Close)
			{
				await _gameplayPresenter.Run();
			}
		}
	}
}