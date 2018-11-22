using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEffect : MonoBehaviour 
{
	private static MaterialPropertyBlock props;

	void Awake()
	{
		if(props == null)
			props = new MaterialPropertyBlock();
	}

	void Start () 
	{
		Vector3 pos = transform.position;
		int value = Mathf.FloorToInt(Mathf.Abs(pos.x)) + Mathf.FloorToInt(Mathf.Abs(pos.y));
		value %= 2;
		props.SetInt("_ColorIndex", value);

		Renderer renderer = GetComponent<Renderer>();
		renderer.SetPropertyBlock(props);
	}
}
