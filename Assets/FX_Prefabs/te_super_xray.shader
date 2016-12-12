// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:3138,x:32732,y:32896,varname:node_3138,prsc:2|emission-1895-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32200,y:32741,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:6716,x:31970,y:33064,varname:node_6716,prsc:2|DIST-7084-OUT;n:type:ShaderForge.SFN_Slider,id:8639,x:31493,y:32955,ptovrint:False,ptlb:Depth,ptin:_Depth,varname:_Depth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.538462,max:10;n:type:ShaderForge.SFN_ValueProperty,id:1042,x:32169,y:32977,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:_Brightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1895,x:32450,y:33070,varname:node_1895,prsc:2|A-1702-OUT,B-7241-RGB,C-1042-OUT,D-7540-OUT,E-5339-RGB;n:type:ShaderForge.SFN_Fresnel,id:7540,x:31970,y:33294,varname:node_7540,prsc:2|EXP-3220-OUT;n:type:ShaderForge.SFN_Slider,id:3220,x:31554,y:33361,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:_Fresnel,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:50;n:type:ShaderForge.SFN_Tex2d,id:5339,x:32261,y:33293,ptovrint:False,ptlb:TX,ptin:_TX,varname:_TX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:7084,x:31756,y:32918,varname:node_7084,prsc:2|IN-8639-OUT;n:type:ShaderForge.SFN_OneMinus,id:4519,x:31798,y:33140,varname:node_4519,prsc:2|IN-7313-OUT;n:type:ShaderForge.SFN_Slider,id:7313,x:31396,y:33159,ptovrint:False,ptlb:Depth_copy,ptin:_Depth_copy,varname:_Depth_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5982907,max:10;n:type:ShaderForge.SFN_Subtract,id:1702,x:32021,y:32817,varname:node_1702,prsc:2|A-4519-OUT,B-6716-OUT;proporder:7241-8639-1042-3220-5339-7313;pass:END;sub:END;*/

Shader "te_super_xray" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Depth ("Depth", Range(0, 10)) = 1.538462
        _Brightness ("Brightness", Float ) = 1
        _Fresnel ("Fresnel", Range(0, 50)) = 0
        _TX ("TX", 2D) = "white" {}
        _Depth_copy ("Depth_copy", Range(0, 10)) = 0.5982907
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers opengl gles gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _Depth;
            uniform float _Brightness;
            uniform float _Fresnel;
            uniform sampler2D _TX; uniform float4 _TX_ST;
            uniform float _Depth_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 _TX_var = tex2D(_TX,TRANSFORM_TEX(i.uv0, _TX));
                float3 emissive = (((1.0 - _Depth_copy)-saturate((sceneZ-partZ)/(1.0 - _Depth)))*_Color.rgb*_Brightness*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*_TX_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
