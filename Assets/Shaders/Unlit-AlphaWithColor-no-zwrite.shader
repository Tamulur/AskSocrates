// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/TransparentWithColorNoZWrite" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

   Lighting Off Cull Off ZWrite Off ZTest Always Fog { Mode Off } 
   Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		SetTexture [_MainTex] { constantColor [_Color]
                    Combine texture * constant, texture * constant  } 
	}
}
}
