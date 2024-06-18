using System;
using System.Linq;
using Common.LinqExtension;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Retry
{
	public interface IRetryView
	{
		void RegisterCallback(Action onClickCard);
		void Render(RetryProperty prop);
	}

	public class RetryView : MonoBehaviour, IRetryView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _closeButton;

		private RetryProperty _prop;

		void IRetryView.RegisterCallback(Action onClickCard)
		{
			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(() => onClickCard?.Invoke());
		}

		void IRetryView.Render(RetryProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case RetryState.Open:
					_Open();
					break;

				case RetryState.Idle:
				case RetryState.OnClickClose:
					_Render(prop);
					break;

				case RetryState.Close:
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

		private void _Render(RetryProperty prop)
		{
			
		}
	}
}
