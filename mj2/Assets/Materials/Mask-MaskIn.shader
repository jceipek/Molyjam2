// Bitmap based mask - only displays pixels behind nonzero alpha
// Itay Keren, Untame Games
Shader "Mask/Mask-In" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader {
		Tags {"Queue" = "Geometry-20" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 100
		
		Pass {
			// LEqual / Greater
			Alphatest LEqual [_Cutoff]
			AlphaToMask True
			ColorMask 0
			Cull Off
			Lighting Off
			SetTexture [_MainTex] {
				Combine texture * primary
			} 
		}	
	}
}
