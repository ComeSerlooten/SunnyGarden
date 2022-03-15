using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotManager : MonoBehaviour
{
    public List<CarrotGrowth> UsableCarrots;
    public List<Carrot_Item> items;
    public static CarrotManager instance;


    void Awake()
    {
        instance = this;
        foreach (CarrotGrowth c in FindObjectsOfType<CarrotGrowth>())
        {
            UsableCarrots.Add(c);
        }
    }

    public CarrotGrowth CarrotFinder(int x, int y)
    {
        foreach (CarrotGrowth c in UsableCarrots)
        {
            if ((int)Mathf.Round(c.transform.position.x) == x && (int)Mathf.Round(c.transform.position.y) == y)
                return c;
        }
        return null;
    }

    public void CarrotDig(int x, int y, GameObject player)
    {
        // print("Hit");
        CarrotGrowth c = CarrotFinder(x, y);
        if (c) 
        {
            c.gotHarvested = true; 
            if(c.isHarvestable)
            c.CarrotItemSpawn(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
