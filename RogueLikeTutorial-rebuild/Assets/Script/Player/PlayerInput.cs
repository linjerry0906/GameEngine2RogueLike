//2018.11.15    金　淳元
//プレイヤークラス
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

namespace Completed
{
    //MovingObjectを継承
    public class PlayerInput : MovingObject
    {
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public int attack = 1;                      //攻撃力
        public int attack_up_count = 0;             //攻撃力上昇カウント(アイテム)
        public int hp = 3;                          //HP
        public Text hpText;                         //HpのUI
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip recoverSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip recoverSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip attackItemSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip attackItemSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component.
        [SerializeField]
        private Player_Hp_UI hp_ui;

        protected override void Start()
        {
            animator = GetComponent<Animator>();

            base.Start();
        }

        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {

        }

        private void Update()
        {

            //If it's not the player's turn, exit the function.
            if (!GameManager.instance.playersTurn) return;

            int horizontal = 0;     
            int vertical = 0;       

            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));

            //Animatorの左右回転
            FlipAnimation(horizontal);

            if (horizontal != 0) vertical = 0;
            if (horizontal != 0 || vertical != 0)
            {
                CheckNote();        //リズムに合わせてInput確認
                //移動＋移動しようとする場所にオブジェクトがあれば攻撃
                AttemptMove<Destroyable>(horizontal, vertical);
            }
        }

        //移動＋移動しようとする場所にオブジェクトがあれば攻撃
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            base.AttemptMove<T>(xDir, yDir);

            RaycastHit2D hit;

            if (Move(xDir, yDir, out hit))
            {
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            }

            GameManager.instance.playersTurn = false;
        }

        protected override void OnCantMove<T>(T component)
        {
            Destroyable hitWall = component as Destroyable;

            CheckAttack();//プレイヤーの攻撃力Check
            hitWall.Damaged(attack);//攻撃

            animator.SetTrigger("playerChop");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Exit")
            {
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }

            else if (other.tag == "Recover")
            {
                //回復アイテム
                SoundManager.instance.RandomizeSfx(recoverSound1, recoverSound2);
                other.gameObject.SetActive(false);
                if (hp == 3) return;
                hp++;
                hp_ui.Recover(hp);
            }

            else if (other.tag == "Attack")
            {
                //攻撃力UPアイテム
                attack = 2;
                attack_up_count = 3;
                SoundManager.instance.RandomizeSfx(recoverSound1, recoverSound2);
                other.gameObject.SetActive(false);
            }
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void LoseHp(int damage)
        {
            hp -= damage;

            hp_ui.Lose(hp);

            animator.SetTrigger("playerHit");

            CheckIfGameOver();
        }

        private void CheckIfGameOver()
        {
            if (hp <= 0)
            {
                SoundManager.instance.PlaySingle(gameOverSound);

                SoundManager.instance.musicSource.Stop();

                GameManager.instance.GameOver();
            }
        }

        //プレイヤーの攻撃力Check
        private void CheckAttack()
        {
            if (attack_up_count == 0) attack = 1;
            if (attack_up_count > 0) attack_up_count--;
        }

        private void FlipAnimation(int horizontal)
        {
            if (horizontal == 1) transform.localScale = new Vector3(1, 1, 1);
            if (horizontal == -1) transform.localScale = new Vector3(-1, 1, 1);
        }

        //TODO:小川さんのCheckを呼び出し、HPを減らす
        private void CheckNote()
        {

        }
    }
}
