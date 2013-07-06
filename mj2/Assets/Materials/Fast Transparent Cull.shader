Shader "Fast/Transparent+1 Cull" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		//_Cutoff ("Base Alpha cutoff", Range (0,.9)) = 0.1
	}
	SubShader {
		Tags {"Queue" = "Transparent+1" } 
		Blend SrcAlpha OneMinusSrcAlpha 		
		// Lighting irrelevant?
		Lighting Off
				
		// Semi-transparent
		ZWrite Off

		Pass {
			SetTexture [_MainTex] {
             constantColor [_Color]
             Combine texture * constant, texture * constant
			}
		}

	}
} 
