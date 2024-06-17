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
		public record Confirm() : GameplayState;
		public record Summary() : GameplayState;
		public record Close() : GameplayState;
	}

	public record GameplayProperty(GameplayState State, List<CatProperty> Cats);
	public record GameplaySubTabReturn(GameplaySubTabReturnType Type);
	
	public interface IGameplayPresenter
	{
		UniTask Run();
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private IGameplayView _view;

		private GameplayProperty _prop;
		private ISummaryPresenter _summaryPresenter;

		public const int Length = 30;
		
		public GameplayPresenter(IGameplayView view, IBattleView battleView, IDeckView deckView, ISummaryPresenter summaryPresenter)
		{
			_view = view;
			_summaryPresenter = summaryPresenter;

			_view.RegisterCallback(
				battleView,
				deckView,
				() =>
					_ChangeStateIfIdle(new GameplayState.Confirm()));
		}

		async UniTask IGameplayPresenter.Run()
		{
			_prop = new GameplayProperty(new GameplayState.Open(), new List<CatProperty>(){ new CatProperty(1, "Cat", true, 11, 39, 5, 1, 1), new CatProperty(2, "Cat", false, 17, 39, 5, 2, 1) });
			
			while (_prop.State is not GameplayState.Close)
			{
				_TryUpdateBattle();
				_view.Render(_prop);
				switch (_prop.State)
				{
					case GameplayState.Open:
						_prop = _prop with { State = new GameplayState.Idle() };
						break;

					case GameplayState.Idle:
						break;

					case GameplayState.Confirm info:
						_prop = _prop with { State = new GameplayState.Close() };
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
		
		float time = 0f;
		float time_threshold = 1f;
		private void _TryUpdateBattle()
		{
			time += UnityEngine.Time.deltaTime;
			if (time >= time_threshold)
			{
				_UpdateBattle();
				time = 0f;
			}
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
					for (int j=0; j<=cat.Speed; j++)
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
					for (int j=0; j<=cat.Speed; j++)
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
	}
}