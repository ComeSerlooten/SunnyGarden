using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] public bool Static = false;

    [Space]
    [SerializeField] Sprite trunkFull;
    [SerializeField] Sprite trunkDamaged;
    [SerializeField] Sprite trunkCut;
    [SerializeField] ParticleSystem DamageParticles;
    [SerializeField] ParticleSystem LeafParticles;
    [SerializeField] ParticleSystem LeafDamageParticles;
    [Space(5)]

    public GameObject playerHitter;
    [SerializeField] GameObject logItem;
    [SerializeField] float chopStrength = 5;
    [Space(5)]

    [SerializeField] int maxHit = 3;
    [SerializeField] Animator treetop;
    public int hitPoints = 3;

    [SerializeField] float timeBeforeRespawn = 10f;
    private float counter = 0;
    private bool counting = false;

    public bool isCut = false;
    public bool isHit = true;
    public bool isHittable = false;

    
    void Start()
    {
        hitPoints = maxHit;
        GetComponent<SpriteRenderer>().sprite = trunkFull;
        treetop.SetFloat("SwaySpeed", ((Random.Range(0, 1) * 2) - 1) * treetop.GetFloat("SwaySpeed") );
        treetop.SetFloat("SwaySpeed", Random.Range(- treetop.GetFloat("SwaySpeed")/5, treetop.GetFloat("SwaySpeed")/5));

        Vector3 position = transform.parent.position;
        position.x = Mathf.RoundToInt(position.x);
        position.y = Mathf.RoundToInt(position.y);

        Tree t = TreesManager.instance.TreeFinder((int)position.x, (int)position.y);

        if (t !=null && t != this)
        {
            TreesManager.instance.UsableTrees.Remove(this);
            Destroy(this.transform.parent.gameObject);
        }

        transform.parent.position = position;

        //treetop.SetFloat("SwayOffset", Random.value);
    }

    public void LogItemSpawn(GameObject player)
    {
        int factor = (player.transform.position.x > transform.position.x) ? 1 : -1;
        Vector2 impulse = (1 * Vector2.up + factor * Vector2.right).normalized;
        GameObject log = Instantiate(logItem, transform.position + Vector3.up * 0.1f, Quaternion.identity);
        log.GetComponent<Log_Item>().groundY = transform.position.y - 0.25f;

       log.GetComponent<Rigidbody2D>().AddForce(chopStrength * impulse, ForceMode2D.Impulse);

    }

    public void TreeDown()
    {
        Instantiate(DamageParticles, transform.position, Quaternion.identity);
        Instantiate(LeafDamageParticles, transform.position + Vector3.up, Quaternion.identity);

        isCut = true;
        treetop.SetBool("isCut", true);
        hitPoints = 0;


        Color32 color = treetop.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        treetop.GetComponent<SpriteRenderer>().color = color;
        counting = true;
        GetComponent<SpriteRenderer>().sprite = trunkCut;

        treetop.SetFloat("RegrowSpeed", 0.0f);
    }

    public void SendState(string connectId)
    {
        print("trreInfo sent");
        if(hitPoints != maxHit || isCut)
        GameManager.instance.pioconnection.Send("TreeInfo", connectId, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), hitPoints, isCut, counter);
    }

    public void AdjustAtSpawn(int hp, bool cut, float count)
    {
        hitPoints = hp;
        if (hp != maxHit) GetComponent<SpriteRenderer>().sprite = trunkDamaged;
        if (cut)
        {
            TreeDown();
            counter = count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        isHittable = treetop.GetCurrentAnimatorStateInfo(0).IsTag("IDLE");

        
        

        if (isHit && isHittable && hitPoints == 1 )
        {
            LogItemSpawn(playerHitter);
            TreeDown();
        }

        treetop.SetBool("Hit", isHit);
        if (isHit && isHittable && hitPoints > 1)
        {
            playerHitter = null;
            Instantiate(DamageParticles,transform.position,Quaternion.identity);
            Instantiate(LeafDamageParticles, transform.position+Vector3.up, Quaternion.identity);

            isHit = false;
            GetComponent<SpriteRenderer>().sprite = trunkDamaged;
            hitPoints--;
        }

        

        if (counting)
        {
            isHit = false;
            counter += Time.deltaTime;
            if (counter >= timeBeforeRespawn)
            {
                treetop.GetComponent<SpriteRenderer>().color = Color.white;
                GetComponent<SpriteRenderer>().sprite = trunkFull;

                Instantiate(LeafParticles, transform.position, Quaternion.identity);

                counter = 0;
                counting = false;
                isCut = false;
                treetop.SetBool("isCut", false);
                hitPoints = maxHit;
                treetop.SetFloat("RegrowSpeed", 0.8f);
            }
        }

    }
}
