using UnityEngine;
using UnityEditor;


public class DisableEditOnPlayAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(DisableEditOnPlayAttribute))]

public class DisableEditOnPlayDrawer : PropertyDrawer
{
	//ゲーム実行中グレーアウト
	public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel)
	{
		EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
		EditorGUI.PropertyField(aPosition, aProperty, aLabel, true);
		EditorGUI.EndDisabledGroup();
	}
}