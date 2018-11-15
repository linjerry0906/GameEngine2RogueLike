﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeartAnime : MonoBehaviour 
{
	private Animator animator;

	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	public void Play()
	{
		animator.Play("HeartAnime", 0, 0);
	}

	public void SetSpeed(float onePlayPerSecond)
	{
		animator.speed = 1.0f / onePlayPerSecond;
	}
}