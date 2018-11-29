using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//小川
public class HeartMove : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] FloorEffectManager floorEffectManager;
    HeartAnime anim;
    bool isStart;

    // Use this for initialization
    void Start()
    {
        anim = gameObject.AddComponent<HeartAnime>();
        anim.SetSpeed(speed);
        floorEffectManager.SetSpeed(speed);
    }

    public IEnumerator Move(float wait)
    {
        while (true)
        {
            anim.Play();
            floorEffectManager.ReverseColor();
            yield return new WaitForSeconds(wait);
        }
    }
}
