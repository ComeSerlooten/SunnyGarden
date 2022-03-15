using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image HealthBarFilling;
    [SerializeField] Color FullColor;
    [SerializeField] Color EmptyColor;

    public int maxHP;
    public int HP;
    Alive target;
    int previousHP;
    // Start is called before the first frame update
    void Start()
    {
        maxHP = PlayerController.instance.startHP;
        target = GetComponentInParent<Alive>();
        SetHealthBar();
    }

    void SetHealthBar()
    {
        float ratio = (float)HP / maxHP;
        HealthBarFilling.fillAmount = ratio;
        HealthBarFilling.color = Color.Lerp(EmptyColor, FullColor, ratio);
        
        GetComponent<Image>().color = (HP == 0)? new Color(0, 0, 0, 0) : Color.black;
        
    }

    // Update is called once per frame
    void Update()
    {
        HP = target.HP;

        if (HP != previousHP)
            SetHealthBar(); 



        previousHP = HP;
    }
}
