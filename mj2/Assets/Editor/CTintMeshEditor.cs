using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(CTintMesh))]
[CanEditMultipleObjects]
public class CTintMeshEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		
		DrawDefaultInspector();

		CTintMesh tm = target as CTintMesh;
		bool tint = false;

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Gradient", GUILayout.Width(60f));
		bool gradient;
		if ((gradient = EditorGUILayout.Toggle(tm.m_gradient)) != tm.m_gradient)
		{
			tm.m_gradient = gradient;
			tint = true;
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Label("Tint:", GUILayout.Width(100f));

		Color c;

		if (tm.m_gradient)
		{
			EditorGUILayout.BeginHorizontal();
			if ((c = EditorGUILayout.ColorField(tm.m_mainTopLeft, GUILayout.Width(100f))) != tm.m_mainTopLeft)
			{
				tint = true;
				tm.m_mainTopLeft = c;
			}
			if ((c = EditorGUILayout.ColorField(tm.m_topRight, GUILayout.Width(100f))) != tm.m_topRight)
			{
				tint = true;
				tm.m_topRight = c;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if ((c = EditorGUILayout.ColorField(tm.m_bottomLeft, GUILayout.Width(100f))) != tm.m_bottomLeft)
			{
				tint = true;
				tm.m_bottomLeft = c;
			}
			if ((c = EditorGUILayout.ColorField(tm.m_bottomRight, GUILayout.Width(100f))) != tm.m_bottomRight)
			{
				tint = true;
				tm.m_bottomRight = c;
			}
			EditorGUILayout.EndHorizontal();

			Vector2 off;
			if ((off = EditorGUILayout.Vector2Field("Offset", tm.m_gradientOffset)) != tm.m_gradientOffset)
			{
				tm.m_gradientOffset = off;
				tint = true;
			}

		}
		else
		{
			if ((c = EditorGUILayout.ColorField(tm.m_mainTopLeft, GUILayout.Width(100f))) != tm.m_mainTopLeft)
			{
				tint = true;
				tm.m_mainTopLeft = c;
			}
		}

		if (tint)
			tm.tintCell();
	}
}