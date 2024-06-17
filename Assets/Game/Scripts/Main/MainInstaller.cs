using UnityEngine;
using Zenject;

namespace Main
{
	public class MainInstaller : MonoInstaller
	{
		// [SerializeField]
		// private WarningView _warningView;
		// [SerializeField]
		// private LoadingView _loadingView;

		// [SerializeField]
		// private LoginView _loginView;
		// [SerializeField]
		// private RegisterView _registerView;
		// [SerializeField]
		// private MainPageView _mainPageView;
		// [SerializeField]
		// private CreateRoomView _createRoomView;
		// [SerializeField]
		// private RoomView _roomView;
		// [SerializeField]
		// private TopMenuView _topMenuView;

		// [SerializeField]
		// private TicTacToeGameplayView _ticTacToeGameplayView;

		public override void InstallBindings()
		{
			// #region Web

			// Container
			// 	.Bind<IHTTPPresenter>()
			// 	.To<HTTPPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IRoomWebSocketPresenter>()
			// 	.To<RoomWebSocketPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<ITicTacToeGameplayWebSocketPresenter>()
			// 	.To<TicTacToeGameplayWebSocketPresenter>()
			// 	.AsSingle();

			// Container
			// 	.Bind<BackendPlayerData>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IBackendPlayerPresenter>()
			// 	.To<BackendPlayerPresenter>()
			// 	.AsSingle();

			// #endregion

			// #region Common

			// Container
			// 	.Bind<IAssetSession>()
			// 	.To<ResourceLoader>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IWarningPresenter>()
			// 	.To<WarningPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IWarningView>()
			// 	.To<WarningView>()
			// 	.FromInstance(_warningView);
			// Container
			// 	.Bind<ILoadingView>()
			// 	.To<LoadingView>()
			// 	.FromInstance(_loadingView);
			// Container
			// 	.Bind<IBackendPlayerView>()
			// 	.WithId("TopMenu")
			// 	.To<TopMenuView>()
			// 	.FromInstance(_topMenuView);

			// #endregion

			// #region Authorization

			// Container
			// 	.Bind<IAuthorizationPresenter>()
			// 	.To<AuthorizationPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<ILoginPresneter>()
			// 	.To<LoginPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<ILoginView>()
			// 	.To<LoginView>()
			// 	.FromInstance(_loginView);
			// Container
			// 	.Bind<IRegisterPresenter>()
			// 	.To<RegisterPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IRegisterView>()
			// 	.To<RegisterView>()
			// 	.FromInstance(_registerView);

			// #endregion

			// #region Metagame

			// Container
			// 	.Bind<IMetagamePresenter>()
			// 	.To<MetagamePresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IMainPagePresenter>()
			// 	.To<MainPagePresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IMainPageView>()
			// 	.To<MainPageView>()
			// 	.FromInstance(_mainPageView);
			// Container
			// 	.Bind<ICreateRoomPresenter>()
			// 	.To<CreateRoomPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<ICreateRoomView>()
			// 	.To<CreateRoomView>()
			// 	.FromInstance(_createRoomView);
			// Container
			// 	.Bind<IRoomPresenter>()
			// 	.To<RoomPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<IRoomView>()
			// 	.To<RoomView>()
			// 	.FromInstance(_roomView);
			// Container
			// 	.Bind<ITopMenuView>()
			// 	.To<TopMenuView>()
			// 	.FromInstance(_topMenuView);

			// #endregion

			// #region Gameplay
			
			// Container
			// 	.Bind<IGameplayPresenter>()
			// 	.To<GameplayPresenter>()
			// 	.AsSingle();
			
			// Container
			// 	.Bind<ITicTacToeGameplayPresenter>()
			// 	.To<TicTacToeGameplayPresenter>()
			// 	.AsSingle();
			// Container
			// 	.Bind<ITicTacToeGameplayView>()
			// 	.To<TicTacToeGameplayView>()
			// 	.FromInstance(_ticTacToeGameplayView);
				
			// #endregion
		}
	}
}
