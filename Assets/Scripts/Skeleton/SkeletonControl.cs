using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonControl : Alive
{
    public Vector2 currentPosition;
    public Vector2 attackTarget;
    public Vector2 TargetPosition;
    public Vector2 toTargetPosition;
    Vector3 startPos;
    public bool hasTarget;
    public bool facingRight = true;
    Skelly_AnimationInterface Anims;

    [SerializeField] float movementSpeed = 2f;
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] public bool respawnable = true;
    

    public Vector3 distVect;
    GameObject target;
    public bool atTarget = false;
    public bool attacking = false;


    private void Awake()
    {
        Anims = GetComponentInChildren<Skelly_AnimationInterface>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SkellyManager.instance.Skeletons.Add(this);
        startPos = transform.position;
    }

    public void SendPosition(int index, string connectId)
    {
        print("skellyInfo Sent");
        GameManager.instance.pioconnection.Send("Skelly", connectId, index, transform.position.x, transform.position.y, Anims.hitPoints, respawnable );
    }

    void AttackTargetUpdate()
    {
        int factor = (facingRight) ? 1 : -1;
        attackTarget = currentPosition;
        attackTarget.x += factor;
    }

    public void ChangeOrientation(int angle)
    {
        //print("change");
        transform.rotation = (Quaternion.Euler(0, angle, 0));
    }

    void OrientationCheck()
    {
        if (facingRight && toTargetPosition.x < 0)
        {
            ChangeOrientation(180);
            facingRight = false;
        }
        if (!facingRight && toTargetPosition.x > 0)
        {
            ChangeOrientation(0);
            facingRight = true;
        }
    }

    void FindClosestPlayer()
    {
        target = (!PlayerController.instance.GetComponent<AnimationInterface>().Dead) ? PlayerController.instance.gameObject : this.gameObject;
        distVect = (!PlayerController.instance.GetComponent<AnimationInterface>().Dead) ? -(transform.position - PlayerController.instance.transform.position) : Vector3.up*1000f;


        foreach (GameObject shell in GameManager.instance.otherPlayers)
        {
            Vector3 otherDistVect = -(transform.position - shell.transform.position);
            if (otherDistVect.magnitude < distVect.magnitude && (!shell.GetComponent<AnimationInterface>().Dead))
            {
                distVect = otherDistVect;
                target = shell;
            }
        }

    }

    bool AllPlayersDead()
    {
        bool noAlive = true;
        List<AnimationInterface> animators = new List<AnimationInterface>();
        foreach (GameObject sh in GameManager.instance.otherPlayers)
        {
            animators.Add(sh.GetComponent<AnimationInterface>());
        }
        animators.Add(PlayerController.instance.GetComponent<AnimationInterface>());

        foreach (AnimationInterface i in animators)
        { if (!i.isDead) noAlive = false; }

        return noAlive;
    }

    Vector3 AvailableSpot()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 tilePos = transform.position;
        bool available = true;

        if (target.transform.position.x > transform.position.x)
        {
            for (int i = 0; i < 2; i++)
            {
                int tile = i * 2 - 1;
                tilePos = target.transform.position + Vector3.right * tile;
                tilePos.x = (int)Mathf.Round(tilePos.x);
                tilePos.y = (int)Mathf.Round(tilePos.y);
                float tileDist = (transform.position - tilePos).magnitude;

                available = true;
                foreach (SkeletonControl skelly in SkellyManager.instance.Skeletons)
                {
                    if (skelly != this)
                    {
                        int x = (int)Mathf.Round(skelly.transform.position.x);
                        int y = (int)Mathf.Round(skelly.transform.position.y);

                        if (x == tilePos.x && y == tilePos.y) available = false;
                    }
                }
                if (available) break;
            }
        }
        else
        {
            for (int i = 1; i >=0 ; i--)
            {
                int tile = i * 2 - 1;
                tilePos = target.transform.position + Vector3.right * tile;
                tilePos.x = (int)Mathf.Round(tilePos.x);
                tilePos.y = (int)Mathf.Round(tilePos.y);
                float tileDist = (transform.position - tilePos).magnitude;

                available = true;
                foreach (SkeletonControl skelly in SkellyManager.instance.Skeletons)
                {
                    if (skelly != this)
                    {
                        int x = (int)Mathf.Round(skelly.transform.position.x);
                        int y = (int)Mathf.Round(skelly.transform.position.y);

                        if (x == tilePos.x && y == tilePos.y) available = false;
                    }
                }
                if (available) break;
            }
        }
        if (!available) tilePos = transform.position;

        if (tilePos.x == (int)Mathf.Round(transform.position.x) && tilePos.y == (int)Mathf.Round(transform.position.y))
        {
            atTarget = true;
        }
        else
        {
            atTarget = false;
        }

        return tilePos;
    }

    void Attacking()
    {
        if (facingRight && PlayerController.instance.transform.position.x < transform.position.x)
        {
            ChangeOrientation(180);
            facingRight = false;
        }
        if (!facingRight && PlayerController.instance.transform.position.x > transform.position.x)
        {
            ChangeOrientation(0);
            facingRight = true;
            
        }
        Anims.isAttacking = true;
    }

    public void AttackFrame()
    {
        if(!attacking)
        {
            
            GameObject player = PlayerController.instance.gameObject;
            int x = (int)Mathf.Round(player.transform.position.x);
            int y = (int)Mathf.Round(player.transform.position.y);

            //print("Attacking " + x.ToString() + " " + y.ToString());

            if (x == attackTarget.x && y == attackTarget.y && !PlayerController.instance.defending)
            {
                PlayerController.instance.Damaged();
            }
        }
        
        attacking = true;
    }

    public void AttackFrameReset()
    {
        attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        HP = Anims.hitPoints;
        if(GameManager.instance.joinedroom)
        {
            currentPosition.x = (int)Mathf.Round(transform.position.x);
            currentPosition.y = (int)Mathf.Round(transform.position.y);

            if (Anims.Respawn)
            {
                transform.position = startPos;
            }

            if (!Anims.Dead && !Anims.gotHit && !Anims.Hurt && !Anims.Respawn && Anims.hitPoints > 0)
            {
                AttackTargetUpdate();

                Anims.isWalking = (hasTarget && !atTarget);

                FindClosestPlayer();

                if (distVect.magnitude <= detectionRadius)
                {
                    hasTarget = true;
                }
                else
                {
                    hasTarget = false;
                }

                if (hasTarget && !AllPlayersDead())
                {
                    TargetPosition = (Vector2)AvailableSpot();
                    if (!atTarget && !Anims.Attack)
                    {
                        Anims.isAttacking = false;
                        toTargetPosition = -(transform.position - (Vector3)TargetPosition);

                        OrientationCheck();

                        transform.position += movementSpeed * ((Vector3)toTargetPosition).normalized * Time.deltaTime;
                    }
                    else
                    {
                        if(!Anims.gotHit && !Anims.Hurt)
                        Attacking();
                    }
                }
                else
                {
                    Anims.isAttacking = false;
                    atTarget = false;
                }
            }
        }

    }
}
