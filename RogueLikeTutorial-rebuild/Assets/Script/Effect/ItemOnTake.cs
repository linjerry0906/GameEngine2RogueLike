using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnTake : MonoBehaviour 
{
	[SerializeField] private ParticleSystem particle;

	public void OnTake(Transform taker)
	{
		particle.transform.parent = taker;
		particle.transform.localPosition = Vector3.zero;
		particle.Play();
	}
}
