using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotGrowth : MonoBehaviour
{
    [SerializeField] GameObject smallCarrot;
    [SerializeField] GameObject bigCarrot;
    [SerializeField] ParticleSystem Dirt;
    [SerializeField] int level = 2;
    [SerializeField] int harvestLevel = 1;
    [SerializeField] public bool isHarvestable = true;
    [SerializeField] [Range(0, 1)] float ratioToMidLevel = 0.5f;
    
    [Space]
    [SerializeField] GameObject carrotItem;
    [SerializeField] float digStrength = 5;
    public bool gotHarvested = false;
    public bool respawning = false;

    private float counter = 0;
    private float betterCounter = 0;

    private bool smallCarrotOn = false;
    private bool bigCarrotOn = false;
    
    [SerializeField] float timeToFull = 2f;
    // Start is called before the first frame update
    void Start()
    {
        smallCarrot.SetActive(false);
        bigCarrot.SetActive(true);

        Vector3 position = transform.parent.position;
        position.x = Mathf.RoundToInt(position.x);
        position.y = Mathf.RoundToInt(position.y);

        CarrotGrowth c = CarrotManager.instance.CarrotFinder((int)position.x, (int)position.y);

        if (c != null && c != this)
        {
            CarrotManager.instance.UsableCarrots.Remove(this);
            Destroy(this.transform.parent.gameObject);
        }

        transform.parent.position = position;

    }

    public void SendState(string connectId)
    {
        print("carrotInfo sent");
        if (level != 2)
        {
            GameManager.instance.pioconnection.Send("CarrotInfo", connectId, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), level, counter);
        }
    }

    public void AdjustAtSpawn(int lvl, float count)
    {
        level = lvl;
        counter = count;
    }

    public void CarrotItemSpawn(GameObject player)
    {
        int factor = (player.transform.position.x > transform.position.x)? 1 : -1;
        Vector2 impulse = (2*Vector2.up + factor * Vector2.right).normalized;
        GameObject carrot = Instantiate(carrotItem, transform.position + Vector3.up * 0.1f, Quaternion.identity);
        carrot.GetComponent<Carrot_Item>().groundY = transform.position.y;

        carrot.GetComponent<Rigidbody2D>().AddForce(digStrength*impulse, ForceMode2D.Impulse);

    }

    // Update is called once per frame
    void Update()
    {

        isHarvestable = (level >= harvestLevel);
        
        if (gotHarvested && !isHarvestable) { gotHarvested = false; }

        if(gotHarvested)
        {
            Instantiate(Dirt, transform.position + Vector3.forward*-0.002f, Quaternion.Euler(0, 0, 45));
            level = 0;
            counter = 0;
            betterCounter = 0;

            //CarrotItemSpawn();

            gotHarvested = false;
            
        }
        
        if(level < 2)
        {
            counter += Time.deltaTime;
            if (counter >= 0.1f)
            {
                counter = 0;
                betterCounter += 0.1f;
            }
        }

        if ((level == 0) && smallCarrot && bigCarrot)
        {
            smallCarrot.SetActive(false);
            smallCarrotOn = false;

            bigCarrot.SetActive(false);
            bigCarrotOn = false;
        }

        if ((level == 0) && (betterCounter >= ((timeToFull * ratioToMidLevel))) && !smallCarrotOn)
        {
            Instantiate(Dirt, transform.position + Vector3.forward * -0.002f, Quaternion.Euler(0, 0, 45));
            smallCarrotOn = true;
            smallCarrot.SetActive(true);
            level++;
        }

        
        if ((level == 1) && (betterCounter >= (timeToFull)) && !bigCarrotOn)
        {
            bigCarrotOn = true;
            bigCarrot.SetActive(true);

            smallCarrot.SetActive(false);
            smallCarrotOn = false;

            level++;

            counter = 0;
            betterCounter = 0;
        }


    }
}
