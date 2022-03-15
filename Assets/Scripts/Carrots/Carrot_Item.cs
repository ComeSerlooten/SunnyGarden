using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot_Item : MonoBehaviour
{
    [SerializeField] float timeBeforePickup;
    [SerializeField] float pickupRadius;
    public float groundY;
    public bool grounded;
    float counter = 0;
    public int id;
    bool picked = false;

    Vector3 distVect;
    GameObject target;

    Rigidbody2D rgbd;
    DepthAdjustment depth;
    // Start is called before the first frame update
    void Start()
    {
        CarrotManager.instance.items.Add(this);
        id = CarrotManager.instance.items.Count;
        rgbd = GetComponent<Rigidbody2D>();
        depth = GetComponent<DepthAdjustment>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < timeBeforePickup && grounded)
        {
            counter += Time.deltaTime;
        }

        if (!grounded)
        {
            depth.enabled = false;
        }

        if (!picked)
        {
            Vector3 position = transform.position;
            position.y = Mathf.Clamp(position.y, groundY, groundY + 10);
            transform.position = position;
        }

        if(transform.position.y == groundY && !grounded)
        {
            grounded = true;
            rgbd.gravityScale = 0;
            rgbd.velocity = Vector2.zero;
            depth.enabled = true;

        }

        if (counter>= timeBeforePickup && !picked)
        {
            target = PlayerController.instance.gameObject;
            distVect = -(transform.position - PlayerController.instance.transform.position);


            foreach (GameObject shell in GameManager.instance.otherPlayers)
            {
                Vector3 otherDistVect = -(transform.position - shell.transform.position);
                if(otherDistVect.magnitude < distVect.magnitude)
                {
                    distVect = otherDistVect;
                    target = shell;
                }
            }


            if (distVect.magnitude<= pickupRadius)
            {
                picked = true;
            }
        }

        if (picked)
        {
            distVect = -(transform.position - target.transform.position);

            transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.05f);

            if(distVect.magnitude < 0.5f)
            {
                if (target.GetComponent<PlayerController>())
                {
                    target.GetComponent<PlayerController>().CarrotInventory++;
                }
                else if (target.GetComponent<PlayerShell>())
                {
                    target.GetComponent<PlayerShell>().CarrotInventory++;
                }

                if (target == PlayerController.instance.gameObject)
                GUIManager.instance.UpdateInventory();

                CarrotManager.instance.items.Remove(this);
                Destroy(this.gameObject);
            }

        }

        

    }
}
