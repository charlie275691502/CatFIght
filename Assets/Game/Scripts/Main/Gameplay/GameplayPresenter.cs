using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Battle;
using Codice.Client.Common;
using Codice.CM.Client.Differences.Merge;
using Common;
using Cysharp.Threading.Tasks;
using Deck;
using Optional.Collections;
using PlasticGui.Configuration.OAuth;
using Summary;
using UnityEngine;

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
		public record OnClickCard(int Index) : GameplayState;
		public record Summary() : GameplayState;
		public record Close() : GameplayState;
	}

	public record GameplayProperty(GameplayState State, List<CatProperty> Cats, List<CardProperty> HandCards, List<CardProperty> DrawCards, List<CardProperty> GraveCards, float DrawCardsRemainingTime);
	public record GameplaySubTabReturn(GameplaySubTabReturnType Type);
	
	public interface IGameplayPresenter
	{
		UniTask Run();
	}
	
	public static class GameplayUtility
	{
		public const int Length = 50;
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private IGameplayView _view;

		private GameplayProperty _prop;
		private ISummaryPresenter _summaryPresenter;
		
		public GameplayPresenter(IGameplayView view, IBattleView battleView, IDeckView deckView, ISummaryPresenter summaryPresenter)
		{
			_view = view;
			_summaryPresenter = summaryPresenter;

			_view.RegisterCallback(
				battleView,
				deckView,
				(index) =>
					_ChangeStateIfIdle(new GameplayState.OnClickCard(index)));
		}

		async UniTask IGameplayPresenter.Run()
		{
			_prop = new GameplayProperty(
				new GameplayState.Open(), 
				new List<CatProperty>(){ },
				new List<CardProperty>(){ },
				new List<CardProperty>(){ 
					new CardProperty(CardType.Warrior),
					new CardProperty(CardType.Warrior),
					new CardProperty(CardType.Warrior),
					new CardProperty(CardType.Archer),
					new CardProperty(CardType.Archer),
					new CardProperty(CardType.Archer),
					new CardProperty(CardType.Mage),
					new CardProperty(CardType.Mage),
					new CardProperty(CardType.DoubleDamage),
					new CardProperty(CardType.FireCard),
				},
				new List<CardProperty>(){ },
				card_time_threshold);
			
			while (_prop.State is not GameplayState.Close)
			{
				_TryUpdateCard();
				_TryUpdateBattle();
				_view.Render(_prop);
				switch (_prop.State)
				{
					case GameplayState.Open:
						_prop = _prop with { State = new GameplayState.Idle() };
						break;

					case GameplayState.Idle:
						break;

					case GameplayState.OnClickCard Info:
						if (!_prop.Cats.Any(cat => cat.Position == GameplayUtility.Length))
						{
							_UseCard(_prop.HandCards[Info.Index]);
							_prop.GraveCards.Add(_prop.HandCards[Info.Index]);
							_prop.HandCards.RemoveAt(Info.Index);
						}
						_prop = _prop with { State = new GameplayState.Idle() };
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
		}

		private void _ChangeStateIfIdle(GameplayState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not GameplayState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
		
		float battle_time = 0f;
		float battle_time_threshold = 1f;
		private void _TryUpdateBattle()
		{
			battle_time += UnityEngine.Time.deltaTime;
			if (battle_time >= battle_time_threshold)
			{
				_UpdateBattle();
				battle_time = 0f;
			}
		}
		
		float card_time_threshold = 5f;
		private void _TryUpdateCard()
		{
			_prop = _prop with {
				DrawCardsRemainingTime = _prop.DrawCardsRemainingTime - UnityEngine.Time.deltaTime
			};
			
			if (_prop.DrawCardsRemainingTime < 0)
			{
				_UpdateCard();
				_prop = _prop with {
					DrawCardsRemainingTime = 5f
				};
			}
		}
		
		private void _UpdateCard()
		{
			_prop.HandCards.Add(_prop.DrawCards[UnityEngine.Random.Range(0, _prop.DrawCards.Count())]);
		}
		
		private void _UpdateBattle()
		{
			var cats = _prop.Cats.ToList();
			for(int i=0; i<cats.Count(); i++)
			{
				var cat = cats[i];
				if (cat.IsEnemy)
				{
					var newPosition = 0;
					for (int j=0; j<=cat.Speed && cat.Position + j <= GameplayUtility.Length; j++)
					{
						newPosition = cat.Position + j;
						if(cats.Any(otherCat => otherCat.Position >= newPosition + 1 && otherCat.Position <= newPosition + cat.Range))
						{
							break;
						}
					}
					cats[i] = cat with {Position = newPosition};
				} else 
				{
					var newPosition = 0;
					for (int j=0; j<=cat.Speed && cat.Position - j >= 0; j++)
					{
						newPosition = cat.Position - j;
						if(cats.Any(otherCat => otherCat.Position <= newPosition - 1 && otherCat.Position >= newPosition - cat.Range))
						{
							break;
						}
					}
					cats[i] = cat with {Position = newPosition};
				}
			}
			
			for(int i=0; i<cats.Count(); i++)
			{
				var cat = cats[i];
				if (cat.IsEnemy)
				{
					for (int j=1; j<=cat.Range; j++)
					{
						for (int k=0; k<cats.Count(); k++)
						{
							if(cats[k].IsEnemy != cat.IsEnemy && cats[k].Position == cat.Position + j)
							{
								cats[k] = cats[k] with {HP = cats[k].HP - cats[i].ATK};
							}
						}
					}
				} else 
				{
					for (int j=1; j<=cat.Range; j++)
					{
						for (int k=0; k<cats.Count(); k++)
						{
							if(cats[k].IsEnemy != cat.IsEnemy && cats[k].Position == cat.Position - j)
							{
								cats[k] = cats[k] with {HP = cats[k].HP - cats[i].ATK};
							}
						}
					}
				}
			}
			
			_prop = _prop with 
			{
				Cats = cats
			};
		}
		
		int catId = 3;
		private void _UseCard(CardProperty card)
		{
			switch(card.CardType)
			{
				case CardType.Archer:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.Warrior:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.Mage:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.Tank:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.Gun:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.Wizard:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.DoubleDamage:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.MaxHealth:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.FireCard:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
				case CardType.FreezingCard:
					_prop.Cats.Add(new CatProperty(catId++, "Cat", false, GameplayUtility.Length, 39, 5, 2, 1));
					break;
			}
		}
	}
}