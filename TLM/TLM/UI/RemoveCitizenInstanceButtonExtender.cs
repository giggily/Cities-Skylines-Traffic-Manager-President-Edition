﻿using ColossalFramework;
using ColossalFramework.UI;
using CSUtil.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficManager.Manager.Impl;
using TrafficManager.UI.MainMenu;
using UnityEngine;

namespace TrafficManager.UI {
	public class RemoveCitizenInstanceButtonExtender : MonoBehaviour {
		private IList<UIButton> buttons;

		public void Start() {
			buttons = new List<UIButton>();

			var citizenInfoPanel = GameObject.Find("(Library) CitizenWorldInfoPanel").GetComponent<CitizenWorldInfoPanel>();
			if (citizenInfoPanel != null) {
				buttons.Add(AddRemoveCitizenInstanceButton(citizenInfoPanel));
			}

			var touristInfoPanel = GameObject.Find("(Library) TouristWorldInfoPanel").GetComponent<TouristWorldInfoPanel>();
			if (touristInfoPanel != null) {
				buttons.Add(AddRemoveCitizenInstanceButton(touristInfoPanel));
			}
		}

		public void OnDestroy() {
			if (buttons == null) {
				return;
			}

			foreach (UIButton button in buttons) {
				Destroy(button.gameObject);
			}
		}

		protected UIButton AddRemoveCitizenInstanceButton(WorldInfoPanel panel) {
			UIButton button = UIView.GetAView().AddUIComponent(typeof(RemoveCitizenInstanceButton)) as RemoveCitizenInstanceButton;
			
			button.AlignTo(panel.component, UIAlignAnchor.TopRight);
			button.relativePosition += new Vector3(- button.width - 80f, 50f);

			return button;
		}

		public class RemoveCitizenInstanceButton : LinearSpriteButton {
			public override void Start() {
				base.Start();
				width = Width;
				height = Height;
			}

			public override void HandleClick(UIMouseEventParameter p) {
				InstanceID instance = WorldInfoPanel.GetCurrentInstanceID();
				Log._Debug($"Current citizen: {instance.Citizen}");
				if (instance.Citizen != 0) {
					ushort citizenInstanceId = 0;
					Constants.ServiceFactory.CitizenService.ProcessCitizen(instance.Citizen, delegate (uint citId, ref Citizen cit) {
						citizenInstanceId = cit.m_instance;
						return true;
					});

					Log._Debug($"Current citizen: {instance.Citizen} Instance: {citizenInstanceId}");
					if (citizenInstanceId != 0) {
						Constants.ServiceFactory.SimulationService.AddAction(() => Constants.ServiceFactory.CitizenService.ReleaseCitizenInstance(citizenInstanceId));
					}
				}
			}

			public override bool Active {
				get {
					return false;
				}
			}

			public override Texture2D AtlasTexture {
				get {
					return TextureResources.RemoveButtonTexture2D;
				}
			}

			public override string ButtonName {
				get {
					return "RemoveCitizenInstance";
				}
			}

			public override string FunctionName {
				get {
					return "RemoveCitizenInstanceNow";
				}
			}

			public override string[] FunctionNames {
				get {
					return new string[] { "RemoveCitizenInstanceNow" };
				}
			}

			public override string Tooltip {
				get {
					return Translation.GetString("Remove_this_citizen");
				}
			}

			public override bool Visible {
				get {
					return true;
				}
			}

			public override int Width {
				get {
					return 30;
				}
			}

			public override int Height {
				get {
					return 30;
				}
			}

			public override bool CanActivate() {
				return false;
			}
		}
	}
}
