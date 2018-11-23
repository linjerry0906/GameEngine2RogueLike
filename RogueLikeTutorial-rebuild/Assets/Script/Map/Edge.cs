using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
	private Vector2Int start;
    private Vector2Int end;


    public Edge(Vector2Int first, Vector2Int second)
    {
        start = first;
        end = second;
    }

    public Vector2Int Start
    {
        get { return start; }
    }

    public Vector2Int End
    {
        get { return end; }
    }
}
