using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarAnime : MonoBehaviour 
{
	[SerializeField] private float delayDead = 1.0f;
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

	public void TouchedDead()
	{
		animator.SetBool("BarTouch", true);
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponent<BarMove>().enabled = false;
		GetComponent<PassCheck>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		Destroy(gameObject, delayDead);
	}
}
