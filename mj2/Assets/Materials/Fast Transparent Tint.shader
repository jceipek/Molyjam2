Shader "Fast/Transparent Tint" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent"} 
		Blend SrcAlpha OneMinusSrcAlpha 		
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "texcoord", texcoord0
		}
		
		// Lighting irrelevant?
		Lighting Off

		// Render both front and back facing polygons - allow flippage
		Cull Off
		
		// Semi-transparent
		ZWrite Off

		Pass {
			SetTexture [_MainTex] {
             Combine texture +- primary, texture * primary
			}
		}

	}
} 
