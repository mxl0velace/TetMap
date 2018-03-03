using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid
{

    public static int w = 50;
    public static int h = 20;
    public static Transform[,] grid = new Transform[w, h];

    public static Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }
    public static bool atFloor(Vector2 pos)
    {
        return (int)pos.y < 0;
    }
    public static void removeBlocks(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            MonoBehaviour.Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }
    public static void removeSection(int start, int length, int y)
    {
        for (int x = 0; x < length; x++)
        {
            MonoBehaviour.Destroy(grid[(start + x) % w, y].gameObject);
            grid[(start + x) % w, y] = null;
        }
    }
    public static void decreaseRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] != null)
            {
                // Move one towards bottom
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    public static void decreaseSection(int start, int length, int y)
    {
        for (int x = 0; x < length; ++x)
        {
            if (grid[(start + x) % w, y] != null)
            {
                // Move one towards bottom
                grid[(start + x) % w, y - 1] = grid[(start + x) % w, y];
                grid[(start + x) % w, y] = null;

                // Update Block position
                grid[(start + x) % w, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    public static void decreaseRowsAbove(int y)
    {
        for (int i = y; i < h; ++i)
            decreaseRow(i);
    }
    public static void decreaseSectionRowsAbove(int start, int length, int y)
    {
        for (int i = y; i < h; ++i)
            decreaseSection(start, length, i);
    }
    public static bool isRowFull(int y)
    {
        for (int x = 0; x < w; ++x)
            if (grid[x, y] == null)
                return false;
        return true;
    }
    public static bool isSectionFull(int start, int length, int y)
    {
        for (int x = 0; x < length; ++x)
        {
            if (grid[start + x % w, y] == null)
            {
                return false;
            }
        }
        return true;
    }
    public static void deleteFullRows()
    {
        for (int y = 0; y < h; ++y)
        {
            if (isRowFull(y))
            {
                removeBlocks(y);
                decreaseRowsAbove(y + 1);
                --y;
            }
        }
    }
    public static void deleteFullSections(int start, int secLength, int maxBlockOffset)
    {
        int behind = (int)((Mathf.Floor((float)(start - maxBlockOffset) / secLength) * secLength) + w) % w;
        for (int y = 0; y < h; y++)
        {
            if (isSectionFull(behind, secLength, y))
            {
                removeSection(behind, secLength, y);
                decreaseSectionRowsAbove(behind, secLength, y + 1);
                --y;
            }
        }
        int centre = (int)(Mathf.Floor((float)start / secLength) * secLength + w) % w;
        if (centre != behind)
        {
            for (int y = 0; y < h; y++)
            {
                if (isSectionFull(centre, secLength, y))
                {
                    removeSection(centre, secLength, y);
                    decreaseSectionRowsAbove(centre, secLength, y + 1);
                    --y;
                }
            }
        }
        int front = (int)((Mathf.Floor((float)(start + maxBlockOffset) / secLength) * secLength) + w) % w;
        if (front != centre)
        {
            for (int y = 0; y < h; y++)
            {
                if (isSectionFull(front, secLength, y))
                {
                    removeSection(front, secLength, y);
                    decreaseSectionRowsAbove(front, secLength, y + 1);
                    --y;
                }
            }
        }
    }
}
