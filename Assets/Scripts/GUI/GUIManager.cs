using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Tool
{
    Sword,
    Shovel,
    Axe
}

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;
    public Tool selectedTool = Tool.Sword;

    [SerializeField] public Transform HUD;
    [SerializeField] public Transform RespawnScreen;

    [SerializeField] Transform SelectionIcon;
    [Space]
    [SerializeField] Transform Sword;
    [SerializeField] Transform Shovel;
    [SerializeField] Transform Axe;

    [SerializeField] Text CarrotAmount;
    [SerializeField] Text LogAmount;



    private void Awake()
    {
        instance = this;
    }

    public void UpdateInventory()
    {
        CarrotAmount.text = "X " + PlayerController.instance.CarrotInventory;
        LogAmount.text = "X " + PlayerController.instance.WoodInventory;
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (selectedTool)
        {
            case Tool.Sword:
                SelectionIcon.position = Sword.position;
                break;

            case Tool.Shovel:
                SelectionIcon.position = Shovel.position;
                break;

            case Tool.Axe:
                SelectionIcon.position = Axe.position;
                break;
        }
    }

    public void SelectShovel()
    {
        selectedTool = Tool.Shovel;
        SelectionIcon.position = Shovel.position;
    }

    public void SelectAxe()
    {
        selectedTool = Tool.Axe;
        SelectionIcon.position = Axe.position;
    }

    public void SelectSword()
    {
        selectedTool = Tool.Sword;
        SelectionIcon.position = Sword.position;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
