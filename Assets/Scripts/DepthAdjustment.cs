using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthAdjustment : MonoBehaviour
{
    [SerializeField] bool isMobile = false;
    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 100);
    }

    private void Update()
    {
        if (isMobile)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 100);
    }

}
