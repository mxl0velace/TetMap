using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    float lastFall = 0;
    float dropTime = 1;
    float lastMove = 0;
    float moveTime = 0.2f;
    Player player;
    int maxBlockOffset = 2;
    int secLength = 10;
    // Use this for initialization
    void Start()
    {
        // Default position not valid? Then it's game over
        if (!isValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
        player = FindObjectOfType<Player>();
        updateSight();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
    public void handleMovement()
    {
        //Debug.Log("HANDLE MOVEMENT");
        // Move Left
        if (Input.GetAxis("Horizontal") < 0 &&
                Time.time - lastMove >= moveTime || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("LEFT RECV");
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
                player.transform.position += new Vector3(-1, 0, 0);
                updateSight();
            }
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
            //Debug.Log("LEFT FAIL");
            lastMove = Time.time;
        }

        // Move Right
        else if (Input.GetAxis("Horizontal") > 0 &&
                Time.time - lastMove >= moveTime || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);


            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
                player.transform.position += new Vector3(1, 0, 0);
                updateSight();

            }
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
            lastMove = Time.time;
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //Debug.Log(transform.position.ToString());
            transform.Rotate(0, 0, -90);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.Rotate(0, 0, 90);
                //Debug.Log("ROT FAIL");
            }
        }

        // Move Downwards and Fall
        if (Input.GetKeyDown(KeyCode.DownArrow) ||
                Time.time - lastFall >= dropTime || Input.GetKeyDown(KeyCode.S))
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                //BlockGrid.deleteFullRows();
                float x = BlockGrid.roundVec2(transform.position).x;
                int sectionStart = Mathf.RoundToInt(x/secLength)*secLength;
                BlockGrid.deleteFullSections(sectionStart,secLength,maxBlockOffset);

                // Spawn next Group
                player.spawnNext();

            }

            lastFall = Time.time;
        }
    }
    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = BlockGrid.roundVec2(child.position);

            // At floor?
            if (BlockGrid.atFloor(v))
                return false;

            // If out of bounds, wrap around
            if (v.x < 0)
            {
                v.x += BlockGrid.w;
            }
            if (v.x >= BlockGrid.w)
            {
                v.x -= BlockGrid.w;
            }
            //Debug.Log("x " + v.x);
            //Debug.Log("y " + v.y);
            // Block in grid cell (and not part of same group)?
            if (BlockGrid.grid[(int)v.x, (int)v.y] != null &&
                BlockGrid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }
    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < BlockGrid.h; ++y)
            for (int x = 0; x < BlockGrid.w; ++x)
                if (BlockGrid.grid[x, y] != null)
                    if (BlockGrid.grid[x, y].parent == transform)
                        BlockGrid.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = BlockGrid.roundVec2(child.position);
            //If out of bounds, wrap around
            if (v.x < 0)
            {
                v.x += BlockGrid.w;
                //child.transform.position += new Vector3(BlockGrid.w,0);
            }
            if (v.x >= BlockGrid.w)
            {
                v.x -= BlockGrid.w;
                //child.transform.position -= new Vector3(BlockGrid.w,0);
            }
            BlockGrid.grid[(int)v.x, (int)v.y] = child;
        }
    }
    void updateSight()
    {
        transform.position = new Vector3((transform.position.x + BlockGrid.w) % BlockGrid.w, transform.position.y);
        int minCameraX = player.camwidth / 2 + maxBlockOffset;
        player.transform.position = new Vector3((transform.position.x - minCameraX + BlockGrid.w) % BlockGrid.w + minCameraX, player.transform.position.y);
    }
}
