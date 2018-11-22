using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthGrid : MonoBehaviour 
{
	void Update () 
	{
		transform.localPosition = Vector3.zero;
		Vector3 pos = transform.position;
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		transform.position = pos;
	}
}
