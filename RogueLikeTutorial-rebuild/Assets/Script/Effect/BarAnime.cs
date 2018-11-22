using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarAnime : MonoBehaviour 
{
	private Animator animator;

	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	public void Dead()
	{
		animator.SetBool("IsDead", true);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if(other.CompareTag("Heart"))
			Dead();
	}
}
