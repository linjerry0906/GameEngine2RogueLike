//2018.11.08    金　淳元
//プレイヤーHP管理クラス
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hp : MonoBehaviour {

    [SerializeField]
    private int hp_max = 3;
    [SerializeField]
    private int hp;
    [SerializeField]
    private bool isDead;

    private void Start()
    {
        hp = hp_max;
        isDead = false;
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        DeathUpdate();
    }

    private void DeathUpdate()
    {
        if (hp < 1) isDead = true;
    }
	
    public void Recover(int recover)
    {
        hp += recover;
        if (hp >= hp_max) hp = hp_max;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
