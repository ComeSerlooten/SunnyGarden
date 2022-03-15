using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTile : MonoBehaviour
{
    [SerializeField] Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Round(Target.position.x), Mathf.Round(Target.position.y), 0);
    }
}
