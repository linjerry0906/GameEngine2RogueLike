using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Completed;

//小川
public class MusicManager : MonoBehaviour
{
    static MusicManager instance;
    Queue<GameObject> barBox1;
    Queue<GameObject> barBox2;

    [SerializeField] GameObject[] generateObj;
    [SerializeField] float barSpeed;
    [SerializeField] float interval;
    [SerializeField] HeartMove heartMove;
    [SerializeField] DistanceCheck distanceCheck;
    [SerializeField] AudioClip clip;

    public static bool isMove = true;

    public static MusicManager Instance { get { return instance; } }
    public float BarSpeed { get { return barSpeed; } }
    public Queue<GameObject> this[int index]
    {
        get
        {
            if (index == 0) return barBox1;
            else if (index == 1) return barBox2;
            else return null;
        }
    }
    public bool Check { get { return distanceCheck.Check(); } }
    bool isMoving;

    void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<MusicManager>();
        }
    }

    // Use this for initialization
    void Start()
    {
        barBox1 = new Queue<GameObject>();
        barBox2 = new Queue<GameObject>();
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(Generate());
            StartCoroutine(heartMove.Move(interval));
            isMoving = true;
        }
        isMove = true;
    }

    IEnumerator Generate()
    {
        while (true)
        {
            for (int i = 0; i < generateObj.Length; i++)
            {
                if (!SoundManager.instance.IsBgmPlaying) SoundManager.instance.BgmPlay(clip);

                var obj = Instantiate(generateObj[i].transform.GetChild(0).gameObject, generateObj[i].transform.position,
                   generateObj[i].transform.GetChild(0).rotation);
                obj.SetActive(true);
                obj.transform.localScale = generateObj[i].transform.GetChild(0).localScale;

                if (i == 0)
                {
                    barBox1.Enqueue(obj);
                }
                else
                {
                    barBox2.Enqueue(obj);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void Dequeue()
    {
        if (barBox1.Count == 0 || barBox2.Count == 0) return;
        var bar1 = barBox1.Dequeue();
        var bar2 = barBox2.Dequeue();
        Destroy(bar1);
        Destroy(bar2);
        isMove = false;
    }

    public void Touched()
    {
        if (barBox1.Count == 0 || barBox2.Count == 0) return;
        var bar1 = barBox1.Dequeue();
        var bar2 = barBox2.Dequeue();
        bar1.GetComponent<BarAnime>().TouchedDead();
        bar2.GetComponent<BarAnime>().TouchedDead();
        isMove = false;
    }
}
