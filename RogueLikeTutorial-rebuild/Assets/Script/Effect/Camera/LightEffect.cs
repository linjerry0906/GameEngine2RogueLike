using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour 
{
	[SerializeField] private Camera lightMapCamera;
	[SerializeField] private Material lightMapMaterial;
	private RenderTexture renderTexture;

	private void Awake() 
	{
		renderTexture = RenderTexture.GetTemporary (Screen.width, Screen.height);
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		lightMapCamera.targetTexture = renderTexture;
		lightMapMaterial.SetFloat("_resoX", Screen.currentResolution.width);
		lightMapMaterial.SetFloat("_resoY", Screen.currentResolution.height);
	}
	
	void OnRenderImage(RenderTexture src,RenderTexture dest)
    {
        lightMapMaterial.SetTexture ("_LightMap", renderTexture);
        Graphics.Blit (src, dest, lightMapMaterial);
    }
}
