using System;
using System.Linq;
using Common.LinqExtension;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Deck
{
	public interface IDeckView
	{
		void RegisterCallback(Action<int> onClickCard);
		void Render(GameplayProperty prop);
	}
	
	public class DeckView : MonoBehaviour, IDeckView
	{
		private Action<int> _onClickCard;
		
		[SerializeField]
		private CardView[] _cardViews;
		[SerializeField]
		private Text _countingDownText;

		private GameplayProperty _prop;

		void IDeckView.RegisterCallback(Action<int> onClickCard)
		{
			_onClickCard = onClickCard;
		}

		void IDeckView.Render(GameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Render(GameplayProperty prop)
		{
			for(int i=0; i<_cardViews.Count(); i++)
			{
				var index = i;
				if (i<prop.HandCards.Count())
				{
					_cardViews[i].Show(() => _onClickCard?.Invoke(index));
					_cardViews[i].Render(prop.HandCards[i]);
				} else 
				{
					_cardViews[i].Hide();
				}
			}
			_countingDownText.text = Mathf.CeilToInt(prop.DrawCardsRemainingTime).ToString();
		}
	}
}