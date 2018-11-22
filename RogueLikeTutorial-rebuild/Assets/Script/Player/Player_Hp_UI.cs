//2018.11.22    金　淳元
//プレイヤーのHP　UI表示
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Completed
{
    public class Player_Hp_UI : MonoBehaviour
    {
        [SerializeField]
        private List<Image> heart_List;
        [SerializeField]
        private float alpha_count;
        [SerializeField]
        private float alpha;
        [SerializeField]
        private int num;
        [SerializeField]
        private bool recover = false;
        [SerializeField]
        private bool lose = false;


        // Use this for initialization
        void Start()
        {
            heart_List = new List<Image>();
            for (int i = 0; i < transform.childCount; i++)
            {
                heart_List.Add(transform.GetChild(i).GetComponent<Image>());
            }
        }

        private void Update()
        {
            LoseUpdate();
            RecoverUpdate();
        }

        public void Lose(int hp)
        {
            if (recover) recover = false;
            num = 2 - hp;
            lose = true;
            alpha = heart_List[num].color.a;
        }

        public void Recover(int hp)
        {
            if (lose) lose = false;
            num = 3 - hp;
            recover = true;
            alpha = heart_List[num].color.a;
        }

        private void LoseUpdate()
        {
            if (!lose) return;

            alpha -= alpha_count;
            heart_List[num].color = new Color(1, 1, 1, alpha);

            if (alpha <= 0.0f)
            {
                alpha = 0.0f;
                lose = false;
            }

        }

        private void RecoverUpdate()
        {
            if (!recover) return;

            alpha += alpha_count;
            heart_List[num].color = new Color(1, 1, 1, alpha);

            if (alpha >= 1.0f)
            {
                alpha = 1.0f;
                recover = false;
            }
        }


    }
}