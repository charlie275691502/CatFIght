using System;
using System.Collections;
using System.Linq;
using Battle;
using Common;
using Cysharp.Threading.Tasks;
using Deck;
using Optional.Collections;
using Optional.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public interface IGameplayView
	{
		void RegisterCallback(IBattleView battleView, IDeckView deckView, Action<int> onClickCard, Action onClickSetting);
		void Render(GameplayProperty prop);
		UniTask FireProjectile(ProjectileType type, bool isEnemy, int fromPosition, int toPosition, float duration = 0.5f);
		UniTask HitEffect(int catId);
	}
	
	public enum ProjectileType
	{
		Arrow,
		Fireball,
	}
	
	[Serializable]
	public class ProjectilePair
	{
		public ProjectileType ProjectileType;
		public GameObjectPool Pool;
	}

	public class GameplayView : MonoBehaviour, IGameplayView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private GameplayProperty _prop;
		private IBattleView _battleView;
		private IDeckView _deckView;

		[SerializeField]
		private ProjectilePair[] _projectiles;
		[SerializeField]
		private GameObject[] _backgrounds;

		void IGameplayView.RegisterCallback(IBattleView battleView, IDeckView deckView, Action<int> onClickCard, Action onClickSetting)
		{
			_battleView = battleView;
			_deckView = deckView;
			
			_battleView.RegisterCallback();
			_deckView.RegisterCallback(onClickCard);
			
			_button.onClick.RemoveAllListeners();
			_button.onClick.AddListener(() => onClickSetting?.Invoke());
		}

		void IGameplayView.Render(GameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case GameplayState.Open:
					_Open();
					break;

				case GameplayState.Idle:
				case GameplayState.OnClickCard:
				case GameplayState.Summary:
				case GameplayState.Retry:
					_Render(prop);
					break;

				case GameplayState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(GameplayProperty prop)
		{
			_battleView.Render(prop);
			_deckView.Render(prop);
			
			for(int i=0;i <_backgrounds.Count(); i++)
			{
				_backgrounds[i].SetActive(i == prop.BackgroundStage);
			}
		}
		
		async UniTask IGameplayView.FireProjectile(ProjectileType type, bool isEnemy, int fromPosition, int toPosition, float duration = 0.5f)
		{
			var opt = _projectiles
				.FirstOrNone(pair => pair.ProjectileType == type);
			if (opt.HasValue)
			{
				await UniTask.WaitForSeconds(0.5f);
				var gmo = opt.ValueOrFailure().Pool.GetGameObject();
				gmo.transform.localScale = new Vector2(isEnemy ? -1 : 1, 1);
				var from = GameplayUtility.GetWorldPosition(fromPosition);
				var to = GameplayUtility.GetWorldPosition(toPosition);
				for(float time=0f; time <= duration; time+= UnityEngine.Time.deltaTime)
				{
					gmo.transform.localPosition = new Vector2(from + (to - from) * time / 0.5f, 0);
					await UniTask.Yield();
				}
				opt.ValueOrFailure().Pool.ReturnGameObject(gmo);
			}
		}

		async UniTask IGameplayView.HitEffect(int catId)
		{
			await _battleView.HitEffect(catId);
		}
	}
}
