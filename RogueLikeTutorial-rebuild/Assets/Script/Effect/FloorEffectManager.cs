using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEffectManager : MonoBehaviour 
{
	[SerializeField] private float currentLightUp = 0;
	private Material material;

	void Start () 
	{
		material = GetComponent<Renderer>().sharedMaterial;
		StartCoroutine(DebugUpdateColor());
	}
	
	private IEnumerator DebugUpdateColor() 
	{
		while(true)
		{
			yield return new WaitForSeconds(1.0f);
			++currentLightUp;
			currentLightUp %= 2;
			material.SetFloat("_CurrentLightUp", currentLightUp);
		}
	}
}
