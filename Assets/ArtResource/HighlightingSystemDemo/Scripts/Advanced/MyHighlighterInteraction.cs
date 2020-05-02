using System;
using UnityEngine;
using HighlightingSystem;

namespace MyHighlightingSystemDemo
{
	public class MyHighlighterInteraction : MonoBehaviour
	{
		// Hover color
		public Color hoverColor = Color.red;

		
		void Update()
		{

		}

		// RaycastController should trigger this method via onHover event
		public void OnHover(RaycastHit hitInfo)
		{
			Transform tr = hitInfo.collider.transform;
			if (tr == null) { return; }

			var highlighter = tr.GetComponentInParent<Highlighter>();
			if (highlighter == null) { return; }

			// Hover
			highlighter.Hover(hoverColor);

			// Switch tween
			// if (Input.GetButtonDown(buttonFire1))
			// {
			// 	highlighter.tween = !highlighter.tween;
			// }

			// Toggle overlay
			// if (Input.GetButtonUp(buttonFire2))
			// {
			// 	highlighter.overlay = !highlighter.overlay;
			// }
		}

		// 
		private void TriggerAll(int action)
		{
			var highlighters = HighlighterCore.highlighters;
			for (int i = 0; i < highlighters.Count; i++)
			{
				var highlighter = highlighters[i] as Highlighter;
				if (highlighter != null)
				{
					switch (action)
					{
						case 0:
							highlighter.ConstantSwitch();
							break;
						case 1:
							highlighter.ConstantSwitchImmediate();
							break;
						case 2:
							highlighter.Off();
							break;
					}
				}
			}
		}
	}
}