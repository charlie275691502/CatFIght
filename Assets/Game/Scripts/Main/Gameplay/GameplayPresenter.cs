using System;
using System.Collections;
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
using Main;
using Optional.Collections;
using PlasticGui.Configuration.OAuth;
using Retry;
using Summary;
using UnityEngine;
using UnityEngine.UIElements;

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
		public record Retry() : GameplayState;
		public record Close() : GameplayState;
	}

	public record GameplayProperty(GameplayState State, List<CatProperty> Cats, List<CardProperty> HandCards, List<CardProperty> DrawCards, List<CardProperty> GraveCards, float DrawCardsRemainingTime);
	public record GameplaySubTabReturn(GameplaySubTabReturnType Type);
	
	public interface IGameplayPresenter
	{
		UniTask Run(Stage[] timelines);
	}
	
	public static class GameplayUtility
	{
		public const int Length = 50;
		
		public static float GetWorldPosition(int Position)
			=> (Position - Length / 2) * 1500f / Length;
	}
	
	public class GameplayPresenter : IGameplayPresenter
	{
		private IGameplayView _view;

		private GameplayProperty _prop;
		private ISummaryPresenter _summaryPresenter;
		private IRetryPresenter _retryPresenter;
		private Stage[] _timelines;
		
		private ActionQueue _actionQueue;
		
		public GameplayPresenter(IGameplayView view, IBattleView battleView, IDeckView deckView, ISummaryPresenter summaryPresenter, IRetryPresenter retryPresenter)
		{
			_view = view;
			_summaryPresenter = summaryPresenter;
			_retryPresenter = retryPresenter;

			_actionQueue = new ActionQueue();
			_view.RegisterCallback(
				battleView,
				deckView,
				(index) =>
					_ChangeStateIfIdle(new GameplayState.OnClickCard(index)));
		}

		int stage = 0;
		async UniTask IGameplayPresenter.Run(Stage[] timelines)
		{
			stage = 0;
			_timelines = timelines;
			_prop = new GameplayProperty(
				new GameplayState.Open(), 
				new List<CatProperty>(){ 
					new CatProperty(catId++, CatType.Tower, true, 1, 20, 0, 0, 0, false),
					new CatProperty(catId++, CatType.Tower, false, GameplayUtility.Length - 1, 20, 0, 0, 0, false)
				},
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
						var new_card = await _summaryPresenter.Run();
						_prop = _prop with 
						{
							State = new GameplayState.Idle(),
							Cats = new List<CatProperty>(){ 
								new CatProperty(catId++, CatType.Tower, true, 1, 20, 0, 0, 0, false),
								new CatProperty(catId++, CatType.Tower, false, GameplayUtility.Length - 1, 20, 0, 0, 0, false)
							},
							HandCards = new List<CardProperty>(){ },
							DrawCards = _prop.HandCards.Concat(_prop.DrawCards).Concat(_prop.GraveCards).Append(new_card).ToList(),
							GraveCards = new List<CardProperty>(){ },
							DrawCardsRemainingTime = card_time_threshold
						};
						second = 0;
						stage += 1;
						break;

					case GameplayState.Retry:
						await _retryPresenter.Run();
						_prop = _prop with 
						{
							State = new GameplayState.Idle(),
							Cats = new List<CatProperty>(){ 
								new CatProperty(catId++, CatType.Tower, true, 1, 20, 0, 0, 0, false),
								new CatProperty(catId++, CatType.Tower, false, GameplayUtility.Length - 1, 20, 0, 0, 0, false)
							},
							HandCards = new List<CardProperty>(){ },
							DrawCards = _prop.HandCards.Concat(_prop.DrawCards).Concat(_prop.GraveCards).ToList(),
							GraveCards = new List<CardProperty>(){ },
							DrawCardsRemainingTime = card_time_threshold
						};
						second = 0;
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
		int second = 0;
		private void _TryUpdateBattle()
		{
			battle_time += UnityEngine.Time.deltaTime;
			if (battle_time >= battle_time_threshold)
			{
				second ++;
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
			if(_prop.HandCards.Count() == 0)
			{
				_prop.DrawCards.AddRange(_prop.GraveCards);
				_prop.GraveCards.Clear();
			}
			if(_prop.HandCards.Count() < 5)
			{
				_prop.HandCards.Add(_prop.DrawCards[UnityEngine.Random.Range(0, _prop.DrawCards.Count())]);
			}
		}
		
		private void _UpdateBattle()
		{
			var cats = _prop.Cats.Where(cat => cat.HP > 0).ToList();
			for(int i=0; i<cats.Count(); i++)
			{
				var cat = cats[i];
				if (cat.IsEnemy)
				{
					var positions = new List<int>(){};
					for (int j=1; j<=cat.Speed; j++)
					{
						var position = cat.Position + j;
						if (position <= 0 || GameplayUtility.Length <= position)
						{
							break;
						}
						
						if(cats.Any(otherCat => otherCat.Position == position && otherCat.IsEnemy != cat.IsEnemy))
						{
							break;
						}
						
						if(cats.Any(otherCat => otherCat.Position == position && otherCat.IsEnemy == cat.IsEnemy))
						{
							continue;
						}
						
						positions.Add(position);
					}
					var nextEnemyPosition = cat.Position;
					while (nextEnemyPosition < GameplayUtility.Length && !cats.Any(otherCat => otherCat.Position == nextEnemyPosition && otherCat.IsEnemy != cat.IsEnemy))
					{
						nextEnemyPosition ++;
					}
					
					cats[i] = cat with {Position = positions.Where(position => nextEnemyPosition - position >= cat.Range).LastOrNone().ValueOr(cat.Position)};
				} else 
				{
					var positions = new List<int>(){};
					for (int j=1; j<=cat.Speed; j++)
					{
						var position = cat.Position - j;
						if (position <= 0 || GameplayUtility.Length <= position)
						{
							break;
						}
						
						if(cats.Any(otherCat => otherCat.Position == position && otherCat.IsEnemy != cat.IsEnemy))
						{
							break;
						}
						
						if(cats.Any(otherCat => otherCat.Position == position && otherCat.IsEnemy == cat.IsEnemy))
						{
							continue;
						}
						
						positions.Add(position);
					}
					var nextEnemyPosition = cat.Position;
					while (nextEnemyPosition > 0 && !cats.Any(otherCat => otherCat.Position == nextEnemyPosition && otherCat.IsEnemy != cat.IsEnemy))
					{
						nextEnemyPosition --;
					}
					
					cats[i] = cat with {Position = positions.Where(position => position - nextEnemyPosition >= cat.Range).LastOrNone().ValueOr(cat.Position)};
				}
			}
			
			for(int i=0; i<cats.Count(); i++)
			{
				var cat = cats[i];
				var isAttacking = false;
				if (cat.IsEnemy)
				{
					for (int j=1; j<=cat.Range && !isAttacking; j++)
					{
						for (int k=0; k<cats.Count(); k++)
						{
							if(cats[k].IsEnemy != cat.IsEnemy && cats[k].Position == cat.Position + j)
							{
								cats[k] = cats[k] with {HP = cats[k].HP - cats[i].ATK};
								isAttacking = true;
								UniTask.Create(() => _view.HitEffect(cats[k].Id));
								if (cats[i].CatType == CatType.Archer)
								{
									UniTask.Create(() => _view.FireProjectile(ProjectileType.Arrow, cat.IsEnemy, cats[i].Position, cats[k].Position));
								}
								if (cats[i].CatType == CatType.Mage)
								{
									UniTask.Create(() => _view.FireProjectile(ProjectileType.Fireball, cat.IsEnemy, cats[i].Position, cats[k].Position));
								}
								break;
							}
						}
					}
				} else 
				{
					for (int j=1; j<=cat.Range && !isAttacking; j++)
					{
						for (int k=0; k<cats.Count(); k++)
						{
							if(cats[k].IsEnemy != cat.IsEnemy && cats[k].Position == cat.Position - j)
							{
								cats[k] = cats[k] with {HP = cats[k].HP - cats[i].ATK};
								isAttacking = true;
								UniTask.Create(() => _view.HitEffect(cats[k].Id));
								if (cats[i].CatType == CatType.Archer)
								{
									UniTask.Create(() => _view.FireProjectile(ProjectileType.Arrow, cat.IsEnemy, cats[i].Position, cats[k].Position));
								}
								if (cats[i].CatType == CatType.Mage)
								{
									UniTask.Create(() => _view.FireProjectile(ProjectileType.Fireball, cat.IsEnemy, cats[i].Position, cats[k].Position));
								}
								break;
							}
						}
					}
				}
				cats[i] = cats[i] with {IsAttacking = isAttacking };
			}
			
			if (cats.All(otherCat => otherCat.Position != 0))
			{
				_timelines[stage].Timeline
					.FirstOrNone(timeline => timeline.Second == second)
					.MatchSome(timeline => 
					{
						switch(timeline.CatType)
						{
							case CatType.Archer:
								cats.Add(new CatProperty(catId++, CatType.Archer, true, 0, 1, 1, 3, 10, false));
								break;
							case CatType.Warrior:
								cats.Add(new CatProperty(catId++, CatType.Warrior, true, 0, 4, 1, 5, 1, false));
								break;
							case CatType.Mage:
								cats.Add(new CatProperty(catId++, CatType.Mage, true, 0, 2, 2, 3, 5, false));
								break;
							case CatType.Tank:
								cats.Add(new CatProperty(catId++, CatType.Tank, true, 0, 10, 1, 6, 1, false));
								break;
							case CatType.Gun:
								cats.Add(new CatProperty(catId++, CatType.Tank, true, 0, 3, 2, 1, 10, false));
								break;
							case CatType.Wizard:
								cats.Add(new CatProperty(catId++,CatType.Wizard, true, 0, 2, 3, 3, 5, false));
								break;
						}
					});
			}
			
			_prop = _prop with 
			{
				Cats = cats
			};
			
			if (_prop.Cats.Any(cat => cat.CatType == CatType.Tower && cat.HP <= 0 && cat.IsEnemy))
			{
				_prop = _prop with 
				{
					State = new GameplayState.Summary()
				};
			} else if (_prop.Cats.Any(cat => cat.CatType == CatType.Tower && cat.HP <= 0 && !cat.IsEnemy))
			{
				_prop = _prop with 
				{
					State = new GameplayState.Retry()
				};
			}
		}
		
		int catId = 0;
		private void _UseCard(CardProperty card)
		{
			switch(card.CardType)
			{
				case CardType.Archer:
					_prop.Cats.Add(new CatProperty(catId++, CatType.Archer, false, GameplayUtility.Length, 1, 1, 3, 10, false));
					break;
				case CardType.Warrior:
					_prop.Cats.Add(new CatProperty(catId++, CatType.Warrior, false, GameplayUtility.Length, 4, 1, 5, 1, false));
					break;
				case CardType.Mage:
					_prop.Cats.Add(new CatProperty(catId++, CatType.Mage, false, GameplayUtility.Length, 2, 2, 3, 5, false));
					break;
				case CardType.Tank:
					_prop.Cats.Add(new CatProperty(catId++, CatType.Tank, false, GameplayUtility.Length, 10, 1, 6, 1, false));
					break;
				case CardType.Gun:
					_prop.Cats.Add(new CatProperty(catId++, CatType.Tank, false, GameplayUtility.Length, 3, 2, 1, 10, false));
					break;
				case CardType.Wizard:
					_prop.Cats.Add(new CatProperty(catId++,CatType.Wizard, false, GameplayUtility.Length, 2, 3, 3, 5, false));
					break;
				case CardType.DoubleDamage:
					break;
				case CardType.MaxHealth:
					break;
				case CardType.FireCard:
					break;
				case CardType.FreezingCard:
					break;
			}
		}
	}
}