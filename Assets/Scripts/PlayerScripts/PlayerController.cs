using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;


public class Alive : MonoBehaviour
{
    public int HP;
}

public class PlayerController : Alive
{
    public static PlayerController instance;
    
    public bool canMove = true;
    public bool isMoving = false;
    [SerializeField] float speed = 1f;
    int sprintFactor;
    public bool sprinting;
    float counter = 0;
    bool alreadyHit = false;
    public bool defending = false;
    [SerializeField] public GameObject skellySoul;
    [SerializeField] public ParticleSystem DamageParticles;

    [Header("Inventory")]
    [SerializeField] public int maxCapacity = 25;
    public int CarrotInventory = 0;
    public int WoodInventory = 0;
    [SerializeField] public int startHP = 10;

    public bool alive = true;
    

    [Header("References")]
    AnimationInterface Anims;
    [SerializeField] Transform TargetTile;

    
    

    [Header("BodyParts")]
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Hair;
    [SerializeField] GameObject Hand;
    public bool facingRight = true;

    [Header("Walking Bounds :")]
    [SerializeField] int xPlus = 30;
    [SerializeField] int xMinus = -30;
    [SerializeField] int yPlus = 30;
    [SerializeField] int yMinus = -30;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Anims = GetComponent<AnimationInterface>();
        GameManager.instance.ActivePlayer = this.gameObject;
        HP = startHP;
    }
    void PositionClamper()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, xMinus, xPlus);
        position.y = Mathf.Clamp(position.y, yMinus, yPlus);
        transform.position = position;

    }

    void ActionActivation(bool state)
    {
        switch(GUIManager.instance.selectedTool)
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

    bool CheckIfCanMove()
    {
        return (!(Anims.Chop || Anims.Attack || Anims.Dig || !alive));
    }

    public void ToolUse()
    {
        if (!alreadyHit)
        GameManager.instance.pioconnection.Send("ActionHit", (int)(Mathf.Round(TargetTile.transform.position.x)), (int)(Mathf.Round(TargetTile.transform.position.y)), (int)GUIManager.instance.selectedTool);

        

        alreadyHit = true;
    }

    public void ToolUseReset()
    {
        alreadyHit = false;
    }

    public void ChangeOrientation(int angle)
    {
        //print("change");
        Body.transform.rotation = (Quaternion.Euler(0, angle, 0));
        Hair.transform.rotation = (Quaternion.Euler(0, angle, 0));
        Hand.transform.rotation = (Quaternion.Euler(0, angle, 0));
    }

    public void Damaged()
    {
        if (!Anims.Hurt)
        {
            HP--;

            Instantiate(DamageParticles, transform.position + Vector3.up * 0.25f - Vector3.forward, Quaternion.identity);

            GameManager.instance.pioconnection.Send("Damaged", HP);
            Anims.gotHit = true;

            if (HP <= 0 && alive)
            {
                GameManager.instance.pioconnection.Send("Died");
                Anims.isDead = true;
                alive = false;
                GUIManager.instance.HUD.gameObject.SetActive(false);
                GUIManager.instance.RespawnScreen.gameObject.SetActive(true);
                GameObject soul = Instantiate(skellySoul, transform.position, Quaternion.identity);
                soul.transform.SetParent(SkellyManager.instance.transform);
            }
            
        }
       
    }

    public void Respawn()
    {
        GameManager.instance.pioconnection.Send("RespawnPlayer");
        Anims.respawn = true;
        alive = true;
        transform.position = Vector3.zero;
        HP = startHP;

        GUIManager.instance.HUD.gameObject.SetActive(true);
        GUIManager.instance.RespawnScreen.gameObject.SetActive(false);

        //SpawnSkeleton
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.joinedroom)
        {
            if (alive)
            {
                counter += Time.deltaTime;
                if (counter >= 0.01f)
                {
                    counter = 0;
                    if (isMoving)
                        GameManager.instance.pioconnection.Send("Position", transform.position.x, transform.position.y);
                }


                if (CheckIfCanMove())
                {
                    if (sprinting != Input.GetKey(KeyCode.LeftShift))
                    {
                        GameManager.instance.pioconnection.Send("Sprinting", Input.GetKey(KeyCode.LeftShift));
                    }
                    sprinting = Input.GetKey(KeyCode.LeftShift);
                    Anims.isSprinting = sprinting;
                    sprintFactor = (sprinting) ? 2 : 1;


                    float xMove = Input.GetAxis("Horizontal") * Time.deltaTime * speed * sprintFactor;
                    float yMove = Input.GetAxis("Vertical") * Time.deltaTime * speed * sprintFactor;

                    if (xMove != 0)
                    {
                        if (xMove > 0)
                        {
                            if (!facingRight)
                            {
                                GameManager.instance.pioconnection.Send("FacingRight", true);
                                ChangeOrientation(0);
                            }
                            facingRight = true;
                        }
                        else
                        {
                            if (facingRight)
                            {
                                GameManager.instance.pioconnection.Send("FacingRight", false);
                                ChangeOrientation(180);
                            }
                            facingRight = false;
                        }
                    }



                    if (xMove != 0 || yMove != 0)
                    {
                        if (!isMoving)
                        {
                            GetComponent<AnimationInterface>().isMoving = true;
                            GameManager.instance.pioconnection.Send("Moving", true);
                        }
                        isMoving = true;
                    }
                    else
                    {
                        if (isMoving)
                        {
                            GetComponent<AnimationInterface>().isMoving = false;
                            GameManager.instance.pioconnection.Send("Moving", false);
                        }
                        isMoving = false;
                    }

                    if (xMove != 0 && yMove != 0)
                    {
                        xMove /= 1.41421f;
                        yMove /= 1.41421f;
                    }

                    transform.position += new Vector3(xMove, yMove, 0);
                }


                if (Input.GetKeyUp(KeyCode.Space))
                {
                    ActionActivation(false);
                    GameManager.instance.pioconnection.Send("Action", false, (int)GUIManager.instance.selectedTool);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ActionActivation(true);
                    GameManager.instance.pioconnection.Send("Action", true, (int)GUIManager.instance.selectedTool);
                }
            }
            

        }


        //PositionClamper();
    }
}
