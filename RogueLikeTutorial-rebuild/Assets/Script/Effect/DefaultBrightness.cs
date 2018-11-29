using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBrightness : MonoBehaviour 
{
	[SerializeField] private int maxIntansity;
	[SerializeField] private int minIntansity;
	void Start () 
	{
		int intansity = Random.Range(minIntansity, maxIntansity);
		GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, intansity / 255.0f);
	}
}
