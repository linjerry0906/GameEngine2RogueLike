using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrace : MonoBehaviour 
{
	[SerializeField] private GameObject player;
	[SerializeField] private Vector3 offset = new Vector3(0, 0, -10.0f);
	private Vector3 movOffset;
	private Vector3 lastFrame;
	private Vector3 destination;

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
		Vector3 dest = pos + offset;
		if(dest != destination)
		{
			StopAllCoroutines();
			StartCoroutine(Move(dest));
		}

		UpdateMoveOffset();
	}

	private IEnumerator Move(Vector3 dest)
	{
		float rate = 0;
		Vector3 startPos = transform.position;
		destination = dest;
		while(rate < 0.5f)
		{
			rate += Time.deltaTime;
			rate = rate >= 0.5f ? 0.5f: rate;
			float useRate = (1 - rate * 2);
			useRate *= useRate;
			transform.position = Vector3.Lerp(startPos, dest, 1 - useRate);
			yield return null;
		}
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
	}
}
