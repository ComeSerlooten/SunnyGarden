using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShell : Alive
{
    public string id = "";

    Transform Body;
    Transform Hair;
    Transform Hand;
    AnimationInterface Anims;

    [Header("Inventory")]
    [SerializeField] public int maxCapacity = 25;
    public int CarrotInventory = 0;
    public int WoodInventory = 0;


    public bool facingRight = true;

    public void ChangeOrientation(int angle)
    {
        //print("change");
        Body.rotation = (Quaternion.Euler(0, angle, 0));
        Hair.rotation = (Quaternion.Euler(0, angle, 0));
        Hand.rotation = (Quaternion.Euler(0, angle, 0));
    }
    public void Action(bool state, int tool)
    {
        Tool selected = (Tool)tool;   
        switch (selected)
        {
            case Tool.Sword:
                Anims.isAttacking = state;
                Anims.isChopping = false;
                Anims.isDigging = false;
                break;

            case Tool.Axe:
                Anims.isChopping = state;
                Anims.isAttacking = false;
                Anims.isDigging = false;
                break;

            case Tool.Shovel:
                Anims.isDigging = state;
                Anims.isAttacking = false;
                Anims.isChopping = false;
                break;
        }
    }

    private void Awake()
    {
        Anims = GetComponent<AnimationInterface>();
        foreach (Transform go in GetComponentInChildren<Transform>())
        {
            if(go.GetComponent<Animator>())
            {
                if(go.GetComponent<Animator>() == GetComponent<AnimationInterface>().Body)
                {
                    Body = go;
                }
                else if (go.GetComponent<Animator>() == GetComponent<AnimationInterface>().Hair)
                {
                    Hair = go;
                }
                else if (go.GetComponent<Animator>() == GetComponent<AnimationInterface>().Hand)
                {
                    Hand = go;
                }
            }
        }
    }


}
