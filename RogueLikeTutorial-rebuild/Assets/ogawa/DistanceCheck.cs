using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField] GameObject heart;
    [SerializeField] float outDis;
    [SerializeField] float successDis;
    bool isSuccess;

    // Use this for initialization
    void Start()
    {
        isSuccess = false;
    }

    public bool Check()
    {
        if (MusicManager.Instance[0].Peek() == null) return false;
        if (MusicManager.Instance[1].Peek() == null) return false;

        var bar1 = MusicManager.Instance[0].Peek();
        var bar2 = MusicManager.Instance[1].Peek();
        var barPos1 = bar1.transform.position;
        var barPos2 = bar2.transform.position;
        var heartPos = heart.transform.position;
        var bar1_heartDis = Vector2.Distance(heartPos, barPos1);
        var bar2_heartDis = Vector2.Distance(heartPos, barPos2);

            if (bar1_heartDis < successDis && bar2_heartDis < successDis)
            {
                MusicManager.Instance.Dequeue();
                return true;
            }
            else
            {
                MusicManager.Instance.Dequeue();
                return false;
            }
    }
}
