using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Readonly Static Fields
    readonly static int width = 5;
    readonly static int height = 4;
    readonly static int cellSize = 24;
    //readonly static int[] entryXPositions = new int[4] { 0, 1, 2, 3 };
    #endregion

    readonly static private Vector2 TopLeft = new Vector2(-32, 47);

    public int[,] gridArray;

    /*public Grid(int _width, int _height)
    {
        this.width = _width;
        this.height = _height;
        gridArray = new int[width, height];

        //GameObject boardCell;

        /*for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Instantiate(boardCell, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            }
        }*/
    //}

    public static Vector2 GetWorldPositionAt(Vector2 _cell)
    {
        Vector2 worldPos;

        worldPos.x = TopLeft.x + cellSize / 2 + _cell.x * cellSize;
        worldPos.y = TopLeft.y + cellSize / 2 + _cell.y * cellSize - cellSize;

        return worldPos;
    }

    public static Vector2 GetCellOfWorldPosition(Vector2 _pos)
    {
        Vector2 cell;

        if (_pos.x < TopLeft.x + cellSize || _pos.x > TopLeft.x + width * cellSize || _pos.y > TopLeft.y || _pos.y < TopLeft.y - height * cellSize)
            return -Vector2.one;

        cell.x = (int)(_pos.x - TopLeft.x) / cellSize;
        cell.y = (int)(_pos.y - TopLeft.y) / cellSize;

        return cell;
    }
}

public class Node
{
    public Vector2 worldPosition;
    public DinoMovement dino = null;

    public Node(Vector2 _worldPosition)
    {
        this.worldPosition = _worldPosition;
    }
}
