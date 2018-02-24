using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    float lastFall = 0;
    float dropTime = 1;
    // Use this for initialization
    void Start()
    {
        // Default position not valid? Then it's game over
        if (!isValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (transform.childCount==0)
		{
			Destroy(gameObject);
		}
    }
	public void handleMovement(){
		// Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.Rotate(0, 0, 90);
        }

        // Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                Time.time - lastFall >= dropTime)
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
                BlockGrid.deleteFullRows();

                // Spawn next Group
                FindObjectOfType<Player>().spawnNext();

            }

            lastFall = Time.time;
        }
	}
    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = BlockGrid.roundVec2(child.position);

            // Not inside Border?
            if (!BlockGrid.insideBorder(v))
                return false;

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
            BlockGrid.grid[(int)v.x, (int)v.y] = child;
        }
    }
}
