using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeartAnime : MonoBehaviour 
{
	private Animator animator;

	void Start () 
	{
	}

	public void Play()
	{
		animator.Play("HeartAnime", 0, 0);
	}

	public void SetSpeed(float onePlayPerSecond)
	{
		animator = GetComponent<Animator>();
		animator.speed = 1.0f / onePlayPerSecond;
	}
}
