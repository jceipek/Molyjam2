using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CCellSpriteMultiQuad))]
[CanEditMultipleObjects]
public class CCellSpriteMultiQuadEditor : CCellSpriteEditor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
	}
}