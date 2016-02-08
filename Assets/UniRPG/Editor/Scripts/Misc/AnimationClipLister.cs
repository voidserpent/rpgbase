using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UniRPGEditor {

public class AnimationClipLister : EditorWindow
{
	[MenuItem("UniRPG/Misc/Animation Clip Lister")]
	public static void ShowIt()
	{
		AnimationClipLister win = EditorWindow.GetWindow<AnimationClipLister>(true);
#if UNITY_4_5 || UNITY_4_6
		win.title = "Animations";
#else
		win.titleContent = new GUIContent("Animations");
#endif

		win.ShowUtility();
	}

	private Animation obj;
	private Vector2 scroll = Vector2.zero;

	void OnGUI()
	{
		obj = (Animation)EditorGUILayout.ObjectField("Animation Object", obj, typeof(Animation), true);

		if (obj)
		{
			string s = "";
			foreach (AnimationState state in obj) s += state.name + "\n";
			scroll = EditorGUILayout.BeginScrollView(scroll);
			EditorGUILayout.TextArea(s);
			EditorGUILayout.EndScrollView();
		}
	}
} }
