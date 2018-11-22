using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//小川
public class HeartMove : MonoBehaviour
{
    [SerializeField] float speed;
    HeartAnime anim;
    bool isStart;
    // Use this for initialization
    void Start()
    {
        anim = gameObject.AddComponent<HeartAnime>();
        anim.SetSpeed(speed);
    }

    public IEnumerator Move(float wait)
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            anim.Play();
        }
    }
}
