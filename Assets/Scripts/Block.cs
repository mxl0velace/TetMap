using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject ghostblock;
    GameObject ghost;

    // Use this for initialization
    void Start()
    {
        ghost = Instantiate(ghostblock, transform, false);
        ghost.transform.position = new Vector3(transform.position.x + BlockGrid.w, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        ghost.transform.position = new Vector3(transform.position.x + BlockGrid.w, transform.position.y); ;
    }
}
