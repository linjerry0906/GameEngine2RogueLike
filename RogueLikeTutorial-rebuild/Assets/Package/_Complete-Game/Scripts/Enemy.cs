using UnityEngine;
using System.Collections;

namespace Completed
{
    enum EnemyMove
    {
        Normal,
        WallBreak,
        LongDistance,
    };

    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Enemy :MovingObject
    {
        [SerializeField] EnemyMove enemyMove;               //Enemyの行動

        public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
        public int wallDamage = 1;                         //How much damage a player does to a wall when chopping it.
        public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
        public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.

        private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
        protected Transform targetPlayer;                           //Transform to attempt to move toward each turn.
        protected Transform targetWall;//**
        private GameObject Player_Object;

        //  次に行動できるまでのターン数//**
        public int skipCount;
        private int currentCount;

        //  遠距離攻撃のリミットターン数//**
        private int longAttack;

        //Start overrides the virtual Start function of the base class.
        protected override void Start()
        {
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddEnemyToList(this);

            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator>();

            //Find the Player GameObject using it's tag and store a reference to its transform component.
            targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            Player_Object = GameObject.FindGameObjectWithTag("Player");
            targetWall = GameObject.FindGameObjectWithTag("Wall").transform;//**

            currentCount = 0;
            longAttack = 0;
            //Call the start function of our base class MovingObject.
            base.Start();
        }

        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            if (currentCount < skipCount)
            {
                currentCount++;
                return;
            }
            base.AttemptMove<T>(xDir, yDir);
            currentCount = 0;
        }

        public void MoveEnemy()//**
        {
            if (this.gameObject != null || this.gameObject.activeInHierarchy) 
            {
                if (enemyMove == EnemyMove.LongDistance)
                {
                    StartCoroutine(LongDistanceCoroutine());
                }
                else if (enemyMove == EnemyMove.Normal)
                {
                    StartCoroutine(NormalCoroutine());
                }
                else
                {
                    StartCoroutine(WallBreakCoroutine());
                }
            }
            else { return; }
        }

        #region//   Normal
        IEnumerator NormalCoroutine()
        {
            yield return new WaitForEndOfFrame();

            int xDir = 0;
            int yDir = 0;

            targetWall = null;

            #region// Move
            //  Player.X == Enemy.X
            if (Mathf.Abs(targetPlayer.position.x - transform.position.x) < Mathf.Abs(targetPlayer.position.y - transform.position.y))

                yDir = targetPlayer.position.y > transform.position.y ? 1 : -1;

            //  Player.X != Enemy.X
            else
                xDir = targetPlayer.position.x > transform.position.x ? 1 : -1;

            AttemptMove<Player>(xDir, yDir);

            #endregion
        }
        #endregion

        #region//   WallBreak
        IEnumerator WallBreakCoroutine()
        {
            yield return new WaitForEndOfFrame();

            int xDir = 0;
            int yDir = 0;

            //  マップ上にWallがあるか？
            if (GameObject.FindGameObjectWithTag("Wall"))
            {
                targetWall = GameObject.FindGameObjectWithTag("Wall").transform;
            }
            else { targetWall = null; }

            #region// Move-Wall
            if (targetWall != null)
            {
                //  Wallがあるならば壊す
                //  Wall.X == Enemy.X
                if (Mathf.Abs(targetWall.position.x - transform.position.x) < Mathf.Abs(targetWall.position.y - transform.position.y))

                    yDir = targetWall.position.y > transform.position.y ? 1 : -1;

                //  Wall.X != Enemy.X
                else
                    xDir = targetWall.position.x > transform.position.x ? 1 : -1;

                AttemptMove<Destroyable>(xDir, yDir);

            }
            #endregion

            #region// Move-Player
            //  WallがないならPlayerに攻撃する
            else
            {
                targetWall = null;

                //  Player.X == Enemy.X
                if (Mathf.Abs(targetPlayer.position.x - transform.position.x) < Mathf.Abs(targetPlayer.position.y - transform.position.y))

                    yDir = targetPlayer.position.y > transform.position.y ? 1 : -1;

                //  Player.X != Enemy.X
                else
                    xDir = targetPlayer.position.x > transform.position.x ? 1 : -1;

                AttemptMove<Player>(xDir, yDir);
            }
            #endregion
        }
        #endregion

        #region//   LondDistance
        IEnumerator LongDistanceCoroutine()
        {
            yield return new WaitForEndOfFrame();

            int xDir = 0;
            int yDir = 0;

            targetWall = null;

            #region// Move
            if (Mathf.Abs(targetPlayer.position.x - transform.position.x) < Mathf.Abs(targetPlayer.position.y - transform.position.y))
            {
                //  Stay-Attack
                if (targetPlayer.position.x == transform.position.x || targetPlayer.position.y == transform.position.y)
                { longAttack++; yDir = 0;
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; }
                else
                { yDir = targetPlayer.position.y > transform.position.y ? 1 : -1;longAttack = 0;
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                //  Stay-Attack
                if (targetPlayer.position.x == transform.position.x || targetPlayer.position.y == transform.position.y)
                { longAttack++; xDir = 0;
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; }
                else
                { xDir = targetPlayer.position.x > transform.position.x ? 1 : -1;longAttack = 0;
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }

            if (longAttack >= 2)
            {
                Player_Object.GetComponent<Player>().LoseFood(playerDamage);
                animator.SetTrigger("enemyAttack");

                SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);

                longAttack = 0;
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            AttemptMove<Player>(xDir, yDir);

            #endregion
        }
        #endregion

        protected override void OnCantMove<T>(T component)//**
        {
            #region// component==Player
            if (component.tag == "Player")
            {
                Player hitPlayer = component as Player;

                hitPlayer.LoseFood(playerDamage);

                animator.SetTrigger("enemyAttack");

                SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
            }
            #endregion

            #region// component==Wall
            if (component.tag == "Wall")
            {
                //Set hitWall to equal the component passed in as a parameter.
                Destroyable hitWall = component as Destroyable;

                //Call the DamageWall function of the Wall we are hitting.
                hitWall.Damaged(wallDamage);

                animator.SetTrigger("enemyAttack");

                SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
            }
            #endregion

            StartCoroutine(ColorChangeCoroutine());
        }

        IEnumerator ColorChangeCoroutine()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            yield return new WaitForSeconds(1);
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}