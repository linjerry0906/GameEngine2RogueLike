using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//小川
[RequireComponent(typeof(Rigidbody2D))]
public class BarMove : MonoBehaviour
{
    enum DirectionBar { Right, Left }
    [SerializeField] DirectionBar dir;
    Rigidbody2D rb2D;

    // Use this for initialization
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = DirectionBarToVector2(dir) * MusicManager.Instance.BarSpeed;
    }

    Vector2 DirectionBarToVector2(DirectionBar dir)
    {
        if (dir == DirectionBar.Left) return Vector2.left;
        else return Vector2.right;
    }
}
