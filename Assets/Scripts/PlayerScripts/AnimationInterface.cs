using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour
{
    [SerializeField] public Animator Body;
    [SerializeField] public Animator Hair;
    [SerializeField] public Animator Hand;

    [Space] 
    [Header("Current State of the Animation")]
    public bool Idle = false;
    public bool Walk = false;
    public bool Run = false;
    public bool Chop = false;
    public bool Dig = false;
    public bool Attack = false;
    public bool Hurt = false;
    public bool Dead = false;

    [Space]
    [Header("Activate Animator Booleans")]
    public bool isMoving = false;
    public bool isSprinting = false;
    public bool isChopping = false;
    public bool isDigging = false;
    public bool isAttacking = false;
    public bool gotHit = false;
    public bool isDead = false;
    public bool respawn = false;

    public void SetBool(string boolName, bool state)
    {
        Body.SetBool(boolName, state);
        Hair.SetBool(boolName, state);
        Hand.SetBool(boolName, state);
    }

    public void ChangeHair(Animator NewHair)
    {
        Hand = NewHair;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetBool("isMoving", isMoving);
        SetBool("isSprinting", isSprinting);
        SetBool("isChopping", isChopping);
        SetBool("isDigging", isDigging);
        SetBool("isAttacking", isAttacking);
        SetBool("gotHit", gotHit);
        SetBool("isDead", isDead);
        SetBool("respawn", respawn);

        Idle = Body.GetCurrentAnimatorStateInfo(0).IsTag("IDLE");
        Chop = Body.GetCurrentAnimatorStateInfo(0).IsTag("CHOP");
        Dig = Body.GetCurrentAnimatorStateInfo(0).IsTag("DIG");
        Walk = Body.GetCurrentAnimatorStateInfo(0).IsTag("WALK");
        Run = Body.GetCurrentAnimatorStateInfo(0).IsTag("RUN");
        Hurt = Body.GetCurrentAnimatorStateInfo(0).IsTag("HURT");
        Attack = Body.GetCurrentAnimatorStateInfo(0).IsTag("ATTACK");
        Dead = Body.GetCurrentAnimatorStateInfo(0).IsTag("DEAD");

        if (Chop) { isDigging = false; isAttacking = false; /*isChopping = false;*/ }
        if (Attack) { isDigging = false; isChopping = false; /*isAttacking = false;*/ }
        if (Dig) { isChopping = false; isAttacking = false; /*isDigging = false;*/ }

        if (Hurt || Dead) { gotHit = false; isDigging = false; isChopping = false; isAttacking = false; }

        if (respawn) { isDead = false; respawn = false; }

        
        


    }
}
