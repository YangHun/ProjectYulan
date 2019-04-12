using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRpt : MonoBehaviour
{

  public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(transform.position + dir);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
