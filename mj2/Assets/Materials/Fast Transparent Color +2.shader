Shader "Fast/Transparent+2 Color" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		//_Cutoff ("Base Alpha cutoff", Range (0,.9)) = 0.1
	}
	SubShader {
		Tags {"Queue" = "Transparent+2" "RenderType" = "Transparent" } 
		Blend SrcAlpha OneMinusSrcAlpha 		
		// Lighting irrelevant?
		Lighting Off

		// Render both front and back facing polygons - allow flippage
		Cull Off
		
		// Semi-transparent
		ZWrite Off

		Pass {
			SetTexture [_MainTex] {
             	constantColor [_Color]
           		Combine texture * constant DOUBLE, texture * constant
			}
		}

	}
} 
