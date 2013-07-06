//
// ClearTexture - menu item script for removing white pixel artifacts in Unity3D
// Copyright (C) 2010 Itay Keren
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class NewBehaviourScript : MonoBehaviour 
{
	[MenuItem ("Assets/Clear Texture")]
	static void ClearTexture () 
	{
		Texture2D tex = Selection.activeObject as Texture2D;
		if (tex == null) 
		{
			EditorUtility.DisplayDialog("Texture not selected", "Please select a texture", "Cancel");
			return;
		}
		
		if (tex.format != TextureFormat.ARGB32 && tex.format != TextureFormat.RGBA32) 
		{
			EditorUtility.DisplayDialog("Bad format", "Bad format " + tex.format +". File must be in ARGB32/RGBA32 format", "Cancel");
			return;
		}
	 
		Color[] texPixelsNew = tex.GetPixels();
		Color[] texPixels = tex.GetPixels();
		
		// Clear
		int wd = tex.width;
		int ht = tex.height;
		int ii = 0;
		for (int yy = 0; yy < ht; ++yy)
		{
			for (int xx = 0; xx < wd; ++xx)
			{
				if (texPixels[ii].a == 0)
				{
						//texPixelsNew[ii] = Color.clear;
						//continue;
					Color avg = Color.clear;
					int avgcount = 0;
					Color cc;
					if (xx > 0 && (cc = texPixels[ii-1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (xx < wd-1 && (cc = texPixels[ii+1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (yy > 0 && (cc = texPixels[ii-wd]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (yy < ht-1 && (cc = texPixels[ii+wd]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}

					if (xx > 0 && yy > 0 && (cc = texPixels[ii-wd-1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (xx < wd-1 && yy > 0 && (cc = texPixels[ii-wd+1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (xx > 0 && yy < ht-1 && (cc = texPixels[ii+wd-1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}
					if (xx < wd-1 && yy < ht-1 && (cc = texPixels[ii+wd+1]).a > 0)
					{ 
						avg += cc;
						avgcount++;
					}

					texPixelsNew[ii] = avgcount > 0 ? new Color(avg.r / avgcount, avg.g / avgcount, avg.b / avgcount, 0) : Color.clear;
				}
				++ii;
			}
		}
		
		Texture2D tex2 = new Texture2D (wd, ht, TextureFormat.ARGB32, false);
		tex2.SetPixels(texPixelsNew);
		
		// Save texture (WriteAllBytes is not used here in order to keep compatibility with Unity iPhone)
		byte[] texBytes = tex2.EncodeToPNG();
		string fileName = EditorUtility.SaveFilePanel("Save cleared texture", "", tex.name + "_clear", "png");
		if (fileName.Length > 0) 
		{
			FileStream fs = new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter (fs);
			int texlen = texBytes.Length;
			for (var i = 0; i < texlen; ++i) 
				bw.Write(texBytes[i]);
			bw.Close(); 
		}
		DestroyImmediate(tex2);
   }
	
}
