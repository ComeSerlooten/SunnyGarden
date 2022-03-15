using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : MonoBehaviour
{
    [SerializeField] PlayerController player;
    Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        Target = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        int direction = (player.facingRight) ? 1 : -1;

        transform.position = new Vector3(Mathf.Round(Target.position.x) + direction, Mathf.Round(Target.position.y), 0);
    }
}
