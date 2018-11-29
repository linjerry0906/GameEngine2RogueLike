using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrace : MonoBehaviour 
{
	[SerializeField] private GameObject player;
	[SerializeField] private Vector3 offset = new Vector3(0, 0, -10.0f);
	private Vector3 movOffset;
	private Vector3 lastFrame;

	private void Start() 
	{
		enabled = false;
	}

	public void Init() 
	{
		Vector3 pos = player.transform.position;
		transform.position = pos + offset;
		lastFrame = transform.position;
		movOffset = Vector3.zero;
		enabled = true;
	}
	
	void Update () 
	{
		Vector3 pos = player.transform.position;
		transform.position = pos + offset;

		UpdateMoveOffset();
	}

	private void UpdateMoveOffset()
	{
		if(transform.position == lastFrame) return;
		
		movOffset = transform.position - lastFrame;

		Vector3 pos = transform.position - offset;
		pos.x = Mathf.RoundToInt(pos.x) + offset.x;
		pos.y = Mathf.RoundToInt(pos.y) + offset.y;
		pos.z = offset.z;
		if((transform.position - pos).magnitude < 0.01f)
		{
			transform.position = pos;
			lastFrame = pos;
			movOffset = Vector3.zero;
		}

		GetComponent<LightEffect>().SetBlendOffset(movOffset);
		GetComponent<LightEffect>().SwitchBuffer();
	}
}
