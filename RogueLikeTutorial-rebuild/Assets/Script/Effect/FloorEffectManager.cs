using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEffectManager : MonoBehaviour 
{
	[SerializeField] private float currentLightUp = 0;
	[SerializeField] private float effectValue = 0.7f;
	private Material material;
	private Animator animator;

	void Start () 
	{
		animator = GetComponent<Animator>();
		material = GetComponent<Renderer>().sharedMaterial;
	}

	public void SetSpeed(float onePlayPerSecond)
	{
		animator = GetComponent<Animator>();
		animator.SetFloat("speed", 1.0f / onePlayPerSecond);
	}

	private void Update() 
	{
		SetEffectValue();
	}

	public void ReverseColor()
	{
		++currentLightUp;
		currentLightUp %= 2;
		material.SetFloat("_CurrentLightUp", currentLightUp);
		animator.Play("Grid_Anime", 0, 0);
	}

	private void SetEffectValue()
	{
		material.SetFloat("_EffectValue", effectValue);
	}
}
