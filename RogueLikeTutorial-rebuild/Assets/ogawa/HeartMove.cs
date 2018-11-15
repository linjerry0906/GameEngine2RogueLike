using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//小川
public class HeartMove : MonoBehaviour
{
    [SerializeField] float magnification;
    [SerializeField] float interval;
    [SerializeField] float waitTime;
    bool isStart;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Move()
    {

        yield return new WaitForSeconds(waitTime);

        Vector3 scale = transform.localScale;
        transform.localScale = transform.localScale * magnification;

        yield return new WaitForSeconds(interval);
        transform.localScale = scale;

        yield break;
    }
}
