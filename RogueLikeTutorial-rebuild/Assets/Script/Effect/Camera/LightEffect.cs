using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour 
{
	[SerializeField] private Camera lightMapCamera;
	[SerializeField] private Camera emissionCamera;
	[SerializeField] private Material lightMapMaterial;
	[SerializeField] private Material blendMaterial;
	[SerializeField] private float updateSpeed = 0.2f;
	private RenderTexture lightMap;
	private RenderTexture emissionMap;
	private List<RenderTexture> buffers;
	private int currentBuffer;
	private Vector3 shaderOffset;
	private Vector3 lastOffset;

	private void Awake() 
	{
		lightMap = CreateRenderTexture();
		emissionMap = CreateRenderTexture();
		RenderTexture frameBuffer1 = CreateRenderTexture();
		RenderTexture frameBuffer2 = CreateRenderTexture();
		RenderTexture frameBuffer3 = CreateRenderTexture();
		buffers = new List<RenderTexture>();
		buffers.Add(frameBuffer1);
		buffers.Add(frameBuffer2);
		buffers.Add(frameBuffer3);
		lightMapCamera.targetTexture = lightMap;
		lightMapMaterial.SetFloat("_resoX", (float)Screen.currentResolution.width);
		lightMapMaterial.SetFloat("_resoY", (float)Screen.currentResolution.height);

		emissionCamera.targetTexture = emissionMap;
		lightMapMaterial.SetTexture ("_Light", lightMap);
		blendMaterial.SetTexture ("_EmissionMap", emissionMap);
		blendMaterial.SetTexture ("_Light", lightMap);
		shaderOffset = Vector3.zero;
		lastOffset = Vector3.zero;
		SetBlendOffset(Vector3.zero);
		StartCoroutine(UpdateFrameBuffer(updateSpeed));
	}

	private RenderTexture CreateRenderTexture()
	{
		RenderTexture rt = RenderTexture.GetTemporary (Screen.width, Screen.height);
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.filterMode = FilterMode.Point;
		return rt;
	}

	private IEnumerator UpdateFrameBuffer(float second)
	{
		while(true)
		{
			yield return new WaitForSeconds(second);
			
			SwitchBuffer();
		}
	}

	private void SwitchBuffer()
	{
		currentBuffer++;
		currentBuffer %= buffers.Count;
		shaderOffset = Vector3.zero;
		lightMapMaterial.SetVector("_BlendOffSet", new Vector4(shaderOffset.x, shaderOffset.y, shaderOffset.z, 0));
	}
	
	void OnRenderImage(RenderTexture src,RenderTexture dest)
    {
		int lastFrameIndex = (currentBuffer + 1) % buffers.Count;
		int oldestFrameIndex = (currentBuffer + 2) % buffers.Count;

		lightMapMaterial.SetTexture ("_LastFrame", buffers[lastFrameIndex]);
		lightMapMaterial.SetTexture ("_OldFrame", buffers[oldestFrameIndex]);
        Graphics.Blit (src, buffers[currentBuffer], lightMapMaterial);		//Current frame light map

		blendMaterial.SetTexture ("_LightMap", buffers[currentBuffer]);
		Graphics.Blit (src, dest, blendMaterial);
    }

	public void SetBlendOffset(Vector3 offset)
	{
		if(offset == Vector3.zero) lastOffset = Vector3.zero;
		shaderOffset += offset - lastOffset;
		lastOffset = offset;
		lightMapMaterial.SetVector("_BlendOffSet", new Vector4(shaderOffset.x, shaderOffset.y, shaderOffset.z, 0));
	}
}
