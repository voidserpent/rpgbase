﻿// ====================================================================================================================
// DiaQ. Dialogue and Quest Engine for Unity
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================
	
using UnityEngine;
using UnityEditor;
using System.Collections;
using DiaQ;

namespace DiaQEditor
{
	public class DiaQGraphSelectWiz : EditorWindow
	{
		public DiaQGraph selected = null;

		private bool accepted = false;
		private bool lostFocus = false;
		private Vector2 scroll = Vector2.zero;
		private DiaQAsset asset = null;
		private object[] args = null;

		public delegate void OnAcceptEvent(DiaQGraphSelectWiz wiz, object[] args);
		private OnAcceptEvent OnAccept = null;

		public static void ShowWiz(OnAcceptEvent onAccept, DiaQAsset asset, object[] args)
		{
			if (asset == null) return;
			DiaQGraphSelectWiz window = EditorWindow.GetWindow<DiaQGraphSelectWiz>(true);
#if UNITY_4_5 || UNITY_4_6
			window.title = "Select Graph";
#else
			window.titleContent = new GUIContent("Select Graph");
#endif
			window.minSize = new Vector2(300, 350);
			window.maxSize = new Vector2(300, 350);
			window.OnAccept = onAccept;
			window.asset = asset;
			window.args = args;
			window.ShowUtility();
		}

		void OnFocus() { lostFocus = false; }
		void OnLostFocus() { lostFocus = true; }

		void Update()
		{
			if (lostFocus) this.Close();
			if (accepted && OnAccept != null) OnAccept(this, args);
		}

		void OnGUI()
		{
			scroll = GUILayout.BeginScrollView(scroll, false, true);
			{
				if (asset.graphs.Count > 0)
				{
					foreach (DiaQGraph q in asset.graphs)
					{
						if (GUILayout.Toggle(selected == q, q.name, GUI.skin.button, GUILayout.Width(275))) selected = q;
					}
				}
				else
				{
					GUILayout.Label("No Quests found");
				}
			}
			GUILayout.EndScrollView();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (selected == null) GUI.enabled = false;
				if (GUILayout.Button("Accept", GUILayout.Width(60), GUILayout.Height(20))) accepted = true;
				GUI.enabled = true;

				if (GUILayout.Button("Cancel", GUILayout.Width(60), GUILayout.Height(20))) this.Close();
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10);
		}

		// ============================================================================================================
	}
}