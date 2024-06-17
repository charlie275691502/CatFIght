using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
	public interface IBattleView
	{
		void RegisterCallback(Action onConfirm);
		void Render(GameplayProperty prop);
	}

	public class BattleView : MonoBehaviour, IBattleView
	{
		[SerializeField]
		private Button _button;
		[SerializeField]
		private GameObjectPool _pool;

		private Action _onConfirm;

		private GameplayProperty _prop;
		private Dictionary<int, CatView> _catViews = new Dictionary<int, CatView>();

		void IBattleView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void IBattleView.Render(GameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(GameplayProperty prop)
		{
			foreach(var cat in prop.Cats)
			{
				if (_catViews.ContainsKey(cat.Id))
				{
					_catViews[cat.Id].Render(cat);
				} else 
				{
					var gmo = _pool.GetGameObject();
					var view = gmo.GetComponent<CatView>();
					_catViews[cat.Id] = view;
					view.Render(cat);
				}
			}
			
			var catIds = prop.Cats.Select(cat => cat.Id).ToList();
			foreach(var kvp in _catViews.ToArray())
			{
				if (catIds.Contains(kvp.Key) == false)
				{
					_pool.ReturnGameObject(kvp.Value.gameObject);
					_catViews.Remove(kvp.Key);
				}
			}
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
