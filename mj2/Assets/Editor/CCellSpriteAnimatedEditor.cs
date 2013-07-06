using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CCellSpriteAnimated))]
[CanEditMultipleObjects]
public class CCellSpriteAnimatedEditor : CCellSpriteEditor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
	}
}