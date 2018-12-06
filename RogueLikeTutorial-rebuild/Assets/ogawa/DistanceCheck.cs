using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField] GameObject heart;
    [SerializeField] float outDis;
    [SerializeField] float successDis;
    bool isSuccess;

    public static bool isMove = true;

    // Use this for initialization
    void Start()
    {
        isSuccess = false;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Check());
        }
        if (MusicManager.Instance[0].Count > 0 && MusicManager.Instance[1].Count > 0)
        {
            var bar1 = MusicManager.Instance[0].Peek();
            var bar2 = MusicManager.Instance[1].Peek();

            if (bar1.GetComponent<PassCheck>().IsPass && bar2.GetComponent<PassCheck>().IsPass)
            {
                var bar1_bar2 = Vector2.Distance(bar1.transform.position, bar2.transform.position);

                if (bar1_bar2 > outDis)
                {
                    isMove = true;
                    MusicManager.Instance.Dequeue();
                }
                else { isMove = false; }
            }
        }
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
            MusicManager.Instance.Touched();
            return true;
        }
        else
        {
            MusicManager.Instance.Touched();
            return false;
        }
    }
}
