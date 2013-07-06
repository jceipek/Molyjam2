Shader "Fast/Opaque Color" {
	Properties {
	    _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
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
					constantColor [_Color]
		            Combine texture * constant DOUBLE
				}
			}
		}
	}
}