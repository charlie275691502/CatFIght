using System;
using System.Linq;
using Common.LinqExtension;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Summary
{
	public interface ISummaryView
	{
		void RegisterCallback(Action<int> onClickCard);
		void Render(SummaryProperty prop);
	}

	public class SummaryView : MonoBehaviour, ISummaryView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private CardView[] _cards;

		private SummaryProperty _prop;

		void ISummaryView.RegisterCallback(Action<int> onClickCard)
		{
			_cards.ForEach((view, index) => view.Show(() => onClickCard?.Invoke(index)));
		}

		void ISummaryView.Render(SummaryProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case SummaryState.Open:
					_Open();
					break;

				case SummaryState.Idle:
				case SummaryState.OnClickCard:
					_Render(prop);
					break;

				case SummaryState.Close:
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

		private void _Render(SummaryProperty prop)
		{
			_cards.ZipForEach(prop.Cards,
				(view, viewData) => view.Render(viewData));
		}
	}
}
