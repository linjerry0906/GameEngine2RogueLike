using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheck : MonoBehaviour
{
    bool isPass;

    private void Start()
    {
        isPass = false;
    }

    public bool IsPass
    {
        get { return isPass; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            isPass = true;
        }
    }
}
