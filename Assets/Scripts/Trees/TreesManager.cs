using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{
    public List<Tree> UsableTrees;
    public List<Log_Item> items;
    public static TreesManager instance;
    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
        foreach (Tree t in FindObjectsOfType<Tree>())
        {
            if (!t.Static)
            UsableTrees.Add(t);
        }
    }

    public Tree TreeFinder(int x, int y)
    {
        foreach (Tree t in UsableTrees)
        {
            if ((int)Mathf.Round(t.transform.position.x) == x && (int)Mathf.Round(t.transform.position.y) == y)
                return t;
        }
        return null;
    }

    public void TreeHit(int x, int y, GameObject player)
    {
       // print("Hit");
        Tree t = TreeFinder(x, y);
        if (t) { t.isHit = true; t.playerHitter = player; }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
