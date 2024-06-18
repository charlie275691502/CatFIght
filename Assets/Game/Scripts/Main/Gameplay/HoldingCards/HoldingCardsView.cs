using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.LinqExtension;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace HoldingCards
{
	public interface IHoldingCardsView
	{
		void RegisterCallback(Action onClickCard);
		void Render(HoldingCardsProperty prop);
	}

	public class HoldingCardsView : MonoBehaviour, IHoldingCardsView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private GameObjectPool _pool;
		[SerializeField]
		private Button _closeButton;
		[SerializeField]
		private Transform _folder;

		private List<CardView> _cards = new List<CardView>();
		private HoldingCardsProperty _prop;

		void IHoldingCardsView.RegisterCallback(Action onClickCard)
		{
			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(() => onClickCard?.Invoke());
		}
		
		void IHoldingCardsView.Render(HoldingCardsProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case HoldingCardsState.Open:
					_Open();
					break;

				case HoldingCardsState.Idle:
				case HoldingCardsState.OnClickCard:
					_Render(prop);
					break;

				case HoldingCardsState.Close:
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

		private void _Render(HoldingCardsProperty prop)
		{
			_cards.ForEach(card => _pool.ReturnGameObject(card.gameObject));
			prop.Cards.ForEach(card => {
				var gmo = _pool.GetGameObject();
				var view = gmo.GetComponent<CardView>();
				gmo.transform.parent = _folder;
				view.Show(() => {});
				view.Render(card);
				_cards.Add(view);
				});
		}
	}
}
