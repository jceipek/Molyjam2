using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CCellSprite))]
[CanEditMultipleObjects]
public class CCellSpriteEditor : Editor
{
	
	string m_resizeSheetX = "";
	string m_resizeSheetY = "";
	
	public override void OnInspectorGUI ()
	{
		CCellSprite sprite = target as CCellSprite;
		
		EditorGUILayout.Separator();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Position", GUILayout.Width(60f));
		if (GUILayout.Button("Snap", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Snap");
			sprite.snap();
		}
		if (GUILayout.Button("Front", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Fwd");
			sprite.changeZ(-0.01f);
		}
		if (GUILayout.Button("Back", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Back");
			sprite.changeZ(0.01f);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Flip", GUILayout.Width(60f));
		if (GUILayout.Button("Rotate", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Rotate");
			sprite.rotate(true);
		}
		if (GUILayout.Button("Horizontal", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Flip Horiz");
			sprite.flip(true, false);
		}
		if (GUILayout.Button("Vertical", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Flip Vertical");
			sprite.flip(false, true);
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();

		DrawDefaultInspector();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(110f);
		GUILayout.Space(30f);
		if (GUILayout.Button("Up", GUILayout.Height(20f), GUILayout.Width(60f)))
		{
			sprite.setCell(sprite.m_currentCell - Mathf.RoundToInt(sprite.m_numCells.x));
			Debug.Log("Up");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Map cell:", GUILayout.Width(100f));
		if (GUILayout.Button("Left", GUILayout.Height(20f), GUILayout.Width(60f)))
		{
			Debug.Log("Left");
			sprite.setCell(sprite.m_currentCell - 1);
		}

		if (GUILayout.Button("Right", GUILayout.Height(20f), GUILayout.Width(60f)))
		{
			Debug.Log("Right");
			sprite.setCell(sprite.m_currentCell + 1);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(110f);
		GUILayout.Space(30f);
		if (GUILayout.Button("Down", GUILayout.Height(20f), GUILayout.Width(60f)))
		{
			sprite.setCell(sprite.m_currentCell + Mathf.RoundToInt(sprite.m_numCells.x));
			Debug.Log("Down");	
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Atlas:", GUILayout.Width(60f));
		
		m_resizeSheetX = GUILayout.TextField(m_resizeSheetX, GUILayout.Width(60f));
		m_resizeSheetY = GUILayout.TextField(m_resizeSheetY, GUILayout.Width(60f));
		
		if (GUILayout.Button("Resize", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Resize " + int.Parse(m_resizeSheetX) + "," + int.Parse(m_resizeSheetY));
			sprite.resizeAtlas(int.Parse(m_resizeSheetX), int.Parse(m_resizeSheetY));
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(60f);
		if (GUILayout.Button("Double", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Double");
			sprite.resizeAtlas(Mathf.RoundToInt(sprite.m_atlasSize.x * 2), 
							   Mathf.RoundToInt(sprite.m_atlasSize.y * 2));
		}
		if (GUILayout.Button("Half", GUILayout.Height(20f), GUILayout.Width(70f)))
		{
			Debug.Log("Half");
			sprite.resizeAtlas(Mathf.RoundToInt(sprite.m_atlasSize.x / 2), 
							   Mathf.RoundToInt(sprite.m_atlasSize.y / 2));
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();

		
	}
}
