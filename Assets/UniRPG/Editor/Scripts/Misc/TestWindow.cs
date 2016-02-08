using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UniRPGEditor {

public class TestWindow : EditorWindow
{
	[MenuItem("UniRPG/Misc/Test")]
	public static void ShowIt()
	{
		TestWindow win = EditorWindow.GetWindow<TestWindow>(true);
#if UNITY_4_5 || UNITY_4_6
		win.title = "TESTING";
#else
		win.titleContent = new GUIContent("TESTING");
#endif
		win.c = AnimationCurve.Linear(1f, 1f, 10f, 10f);
		win.ShowUtility();
	}

	private AnimationCurve c;

	void OnGUI()
	{
		GUILayout.Label("This is a test/ debug area. Nothing to see here.");
		EditorGUILayout.Space();
		c = EditorGUILayout.CurveField(c);
	}
} }