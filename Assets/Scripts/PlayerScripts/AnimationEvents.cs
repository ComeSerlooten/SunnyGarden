using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    PlayerController parent;
    SkeletonControl skell;
    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponentInParent<PlayerController>();
        skell = GetComponentInParent<SkeletonControl>();
    }

    public void HitFrame()
    {
        if(parent)
        parent.ToolUse();
    }

    public void HitEnded()
    {
        if(parent)
        parent.ToolUseReset();
    }

    public void SkellyAttack()
    {
        if(skell)
        { skell.AttackFrame(); }
    }

    public void SkellyAttackEnded()
    {
        if(skell)
        { skell.AttackFrameReset(); }
    }
}
