using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Main
{
	public record MainState
	{
		public record Authorization() : MainState;
		public record Metagame() : MainState;
		public record Close() : MainState;
	}

	public record MainProperty(MainState State);

	public class Main : MonoBehaviour
	{
		// private IAuthorizationPresenter _authorizationPresenter;
		// private IMetagamePresenter _metagamePresenter;
		// private IGameplayPresenter _gameplayPresenter;

		[Zenject.Inject]
		public void Zenject()
		{
			
		}

		void Start()
		{
			_ = _Main();
		}

		private async UniTask _Main()
		{
			var prop = new MainProperty(new MainState.Authorization());
			while (prop.State is not MainState.Close)
			{
				var subTabReturn = await _GetCurrentSubTabUniTask(prop.State);

				switch (subTabReturn.Type)
				{
					case MainSubTabReturnType.Close:
						prop = prop with { State = new MainState.Close() };
						break;
					case MainSubTabReturnType.Switch info:
						prop = prop with { State = info.State };
						break;
				}
			}
		}

		private UniTask<MainSubTabReturn> _GetCurrentSubTabUniTask(MainState type)
			=> type switch
			{
				// MainState.Authorization => _authorizationPresenter.Run(),
				// MainState.Metagame => _metagamePresenter.Run(),
				// MainState.Game info => _gameplayPresenter.Run(new GameplayParameter(info.GameData)),
				_ => throw new System.NotImplementedException(),
			};
	}
}