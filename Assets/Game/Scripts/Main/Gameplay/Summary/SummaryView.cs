using System;
using UnityEngine;
using UnityEngine.UI;

namespace Summary
{
	public interface ISummaryView
	{
		void RegisterCallback(Action onConfirm);
		void Render(SummaryProperty prop);
	}

	public class SummaryView : MonoBehaviour, ISummaryView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private SummaryProperty _prop;

		void ISummaryView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
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
				case SummaryState.Confirm:
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
			
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
