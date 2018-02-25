using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    int count = 0;
    public GameObject[] groups;
    GameObject currentGroup;
    public int camwidth;
    // Use this for initialization
    void Start()
    {
        camwidth = (int)Mathf.Ceil(2*Camera.main.orthographicSize*Camera.main.aspect);
        spawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGroup != null)
        {
            currentGroup.GetComponent<Group>().handleMovement();
        }
    }

    public void spawnNext()
    {
        int i = Random.Range(0, groups.Length);
        GameObject newGroup = (GameObject)Instantiate(groups[i], transform.position, Quaternion.identity);
        currentGroup = newGroup;

        newGroup.name = count.ToString();
        count += 1;
    }
}
