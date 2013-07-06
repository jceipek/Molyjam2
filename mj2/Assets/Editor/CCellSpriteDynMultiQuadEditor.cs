using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CCellSpriteDynMultiQuad))]
[CanEditMultipleObjects]
public class CCellSpriteDynMultiQuadEditor : CCellSpriteEditor
{
	public override void OnInspectorGUI ()
	{
		CCellSpriteDynMultiQuad sprite = target as CCellSpriteDynMultiQuad;
		//Debug.Log(sprite.m_triOnOff);
		if (sprite != null && sprite.m_sqOnOff != null && sprite.m_sqOnOff.Length == sprite.m_quadsHoriz * sprite.m_quadsVert)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Layout:", GUILayout.Width(60f));
			if (GUILayout.Button("Reset", GUILayout.Height(20f), GUILayout.Width(70f)))
			{
				Debug.Log("Reset");
				sprite.setAllSquares(!sprite.m_sqOnOff[0]);
			}
			EditorGUILayout.EndHorizontal();

			//Debug.Log(sprite.m_quadsVert+ " " +sprite.m_quadsVert);
			for (int y = 0; y < sprite.m_quadsVert; ++y)
			{
				EditorGUILayout.BeginHorizontal();
				for (int x = 0; x < sprite.m_quadsHoriz; ++x)
				{
					bool newval = GUILayout.Toggle(sprite.m_sqOnOff[y * sprite.m_quadsHoriz + x], GUIContent.none, GUILayout.Height(12f), GUILayout.Width(12f));
					if (newval != sprite.m_sqOnOff[y * sprite.m_quadsHoriz + x])
						sprite.setSquareOnOff(x, y, newval);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.BeginHorizontal();
		bool newopt = GUILayout.Toggle(sprite.m_optimizeRectangles, GUIContent.none, GUILayout.Height(12f), GUILayout.Width(12f));
		if (newopt != sprite.m_optimizeRectangles)
			sprite.setOptimized(newopt);
		GUILayout.Label("optimized", GUILayout.Width(60f), GUILayout.Height(20f));
		EditorGUILayout.EndHorizontal();
		/*EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Apply Layout", GUILayout.Height(20f), GUILayout.Width(100f)))
		{
			Debug.Log("Apply");
			sprite.apply();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();*/
		base.OnInspectorGUI();
	}
}