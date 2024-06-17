using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
	public interface IBattleView
	{
		void RegisterCallback(Action onConfirm);
		void Render(BattleProperty prop);
	}
	
	public record BattleProperty();

	public class BattleView : MonoBehaviour, IBattleView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onConfirm;

		private BattleProperty _prop;

		void IBattleView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_button.onClick.AddListener(_OnConfirm);
		}

		void IBattleView.Render(BattleProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			_Render(prop);
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(BattleProperty prop)
		{
			
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}
