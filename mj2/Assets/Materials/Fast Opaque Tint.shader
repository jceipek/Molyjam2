Shader "Fast/Texture Tint" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	Category {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off Lighting Off ZWrite On
			
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "texcoord", texcoord0
		}
			
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					combine texture +- primary
				}
			}
		}
	}
}