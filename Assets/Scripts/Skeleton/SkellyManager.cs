using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkellyManager : MonoBehaviour
{

    public List<SkeletonControl> Skeletons;
    public static SkellyManager instance;



    private void Awake()
    {
        instance = this;
    }


    public void SwordSwing(int x, int y)
    {
        foreach (SkeletonControl sk in Skeletons)
        {
            if (sk.currentPosition.x == x && sk.currentPosition.y == y)
            {
                sk.GetComponentInChildren<Skelly_AnimationInterface>().gotHit = true;
                sk.GetComponentInChildren<Animator>().SetBool("gotHurt", true);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
