using Battle;
using Deck;
using Gameplay;
using Summary;
using UnityEngine;
using Zenject;

namespace Main
{
	public class MainInstaller : MonoInstaller
	{
		[SerializeField]
		private GameplayView _gameplayView;
		[SerializeField]
		private BattleView _battleView;
		[SerializeField]
		private DeckView _deckView;
		[SerializeField]
		private SummaryView _summaryView;

		public override void InstallBindings()
		{
			#region Gameplay
			
			Container
				.Bind<IGameplayPresenter>()
				.To<GameplayPresenter>()
				.AsSingle();
			Container
				.Bind<IGameplayView>()
				.To<GameplayView>()
				.FromInstance(_gameplayView);
				
			Container
				.Bind<IBattleView>()
				.To<BattleView>()
				.FromInstance(_battleView);
			Container
				.Bind<IDeckView>()
				.To<DeckView>()
				.FromInstance(_deckView);
			
			Container
				.Bind<ISummaryPresenter>()
				.To<SummaryPresenter>()
				.AsSingle();
			Container
				.Bind<ISummaryView>()
				.To<SummaryView>()
				.FromInstance(_summaryView);
				
			#endregion
		}
	}
}
