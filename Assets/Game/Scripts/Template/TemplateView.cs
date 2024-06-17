using System;
using UnityEngine;
using UnityEngine.UI;

namespace Template
{
	public interface ITemplateView
	{
		void RegisterCallback(Action onConfirm);
		void Render(TemplateProperty prop);
	}

	public class TemplateView : MonoBehaviour, ITemplateView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private TemplateProperty _prop;

		void ITemplateView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void ITemplateView.Render(TemplateProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case TemplateState.Open:
					_Open();
					break;

				case TemplateState.Idle:
				case TemplateState.Confirm:
					_Render(prop);
					break;

				case TemplateState.Close:
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

		private void _Render(TemplateProperty prop)
		{

		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
