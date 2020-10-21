using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    #region Readonly Static Fields
    readonly static int width = 5;
    readonly static int height = 4;
    readonly static int cellSize = 24;
    readonly static private Vector2 TopLeft = new Vector2(-32, 47);
    #endregion

    //public static List<int> monstersOnBoard = new List<int>();
    public static Stack<KeyValuePair<Dino, Vector2>> monstersToMove;

    public Cell[,] grid;
    [SerializeField]
    private GameObject dino;

    void Awake()
    {
        #region Singleton (Unity)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

        Initialize();
    }

    public void Initialize()
    {
        grid = new Cell[width, height];

        monstersToMove = new Stack<KeyValuePair<Dino, Vector2>>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new Cell(GetWorldPositionAt(new Vector2(x, -y)));
                //Debug.Log("Coordinates: " + x + ", " + y + " --> Center: " + grid[x, y].Center); // DEBUG
                //Instantiate(new GameObject(), grid[x, y].Center, Quaternion.identity);
            }
        }
    }

    public void NextWave(int[] _wave)
    {
        monstersToMove = new Stack<KeyValuePair<Dino, Vector2>>();

        List<Vector2> newWaveCoordinates = GenerateNewWaveCoordinates(_wave.Length);
        InstantiateAndGetMonstersToMove(_wave, newWaveCoordinates);
        MoveMonsters();
    }

    private void InstantiateAndGetMonstersToMove(int[] _wave, List<Vector2> _newWaveCoordinates)
    {
        if (_wave.Length > 0 && _newWaveCoordinates.Count > 0)
        {
            for (int i = 0; i < _wave.Length; i++)
            {
                Dino newDino = InstantiateMonster(_wave[i], _newWaveCoordinates[i]);
                monstersToMove.Push(new KeyValuePair<Dino, Vector2>(newDino, _newWaveCoordinates[i]));
                //Debug.Log("_newWaveCoordinates: "+ _newWaveCoordinates[i]); // DEBUG
                GetMonstersToMove(_newWaveCoordinates[i]);
            }
        }
    }

    private Dino InstantiateMonster(int _number, Vector2 newWavePosition)
    {
        Vector2 worldPosition = GetWorldPositionAt(new Vector2(newWavePosition.x + 2, newWavePosition.y));

        GameObject newMonster = Instantiate(dino, worldPosition, Quaternion.identity);
        Dino newDino = newMonster.GetComponent<Dino>();
        newDino.SetNumber(_number);

        return newDino;
    }

    private void GetMonstersToMove(Vector2 _positionToCheck)
    {
        if (_positionToCheck.x <= 0)
            return;

        Dino dino = grid[(int)_positionToCheck.x, Mathf.Abs((int)_positionToCheck.y)].Dino; // Get Dino from node at _positionToCheck

        if (dino != null) // Check if board position has a monster
        {
            Vector2 positionToMove = new Vector2(_positionToCheck.x - 1, _positionToCheck.y); 
            monstersToMove.Push(new KeyValuePair<Dino, Vector2>(dino, positionToMove)); 
            GetMonstersToMove(positionToMove);
        }
    }

    public void MoveMonsters()
    {
        foreach (KeyValuePair<Dino, Vector2> keyValuePair in monstersToMove)
        {
            Vector2 cellCenter = keyValuePair.Value;
            keyValuePair.Key.SetTarget(grid[(int)cellCenter.x, Mathf.Abs((int)cellCenter.y)].Center);

            Debug.Log((int)cellCenter.x + ", " + (int)cellCenter.y); // DEBUG
        }

        monstersToMove.Clear();
    }

    public List<Dino> GetMonstersOnBoard()
    {
        List<Dino> monstersOnBoard = new List<Dino>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                monstersOnBoard.Add(grid[x, y].Dino);
            }
        }
        return monstersOnBoard;
    }

    public int GetRandomMonster()
    {
        List<Dino> monstersOnBoard = GetMonstersOnBoard();

        int randomIndex = UnityEngine.Random.Range(1, monstersOnBoard.Count + 1);

        return monstersOnBoard[randomIndex].Number;
    }

    public static List<Vector2> GenerateNewWaveCoordinates(int _waveSize)
    {
        List<Vector2> newWaveCoordinates = new List<Vector2>();

        List<int> yPositions = new List<int> { 0, -1, -2, -3 };

        for (int i = 0; i < _waveSize; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, yPositions.Count);
            newWaveCoordinates.Add(new Vector2(4f, yPositions[randomIndex]));
            yPositions.RemoveAt(randomIndex);
        }

        foreach (Vector2 vect in newWaveCoordinates) // DEBUG
        {
            Debug.Log(vect);
        }

        return newWaveCoordinates;
    }

    private Cell GetCellFromWorldPosition(Vector2 _worldPosition)
    {
        if (_worldPosition.x < TopLeft.x + cellSize
            || _worldPosition.x > TopLeft.x + width * cellSize
            || _worldPosition.y > TopLeft.y
            || _worldPosition.y < TopLeft.y - height * cellSize)
            return null;

        return grid[(int)(_worldPosition.x - TopLeft.x) / cellSize, (int)(_worldPosition.y - TopLeft.y) / cellSize];
    }

    public static Vector2 GetWorldPositionAt(Vector2 _cell)
    {
        Vector2 worldPos;

        worldPos.x = TopLeft.x + cellSize / 2 + _cell.x * cellSize;
        worldPos.y = TopLeft.y + cellSize / 2 + _cell.y * cellSize - cellSize;

        return worldPos;
    }

    public static Vector2 GetGridCoordinatesFromWorldPosition(Vector2 _pos)
    {
        Vector2 gridCoordinates;

        if (_pos.x < TopLeft.x + cellSize
            || _pos.x > TopLeft.x + width * cellSize
            || _pos.y > TopLeft.y
            || _pos.y < TopLeft.y - height * cellSize)
            return -Vector2.one;

        gridCoordinates.x = (int)(_pos.x - TopLeft.x) / cellSize;
        gridCoordinates.y = (int)(_pos.y - TopLeft.y) / cellSize;

        return gridCoordinates;
    }
}

public class Cell
{
    public Vector2 Center { get; private set; }
    public Dino Dino { get; private set; }

    public Cell(Vector2 _center)
    {
        this.Center = _center;
    }

    public void SetDino(Dino _dino)
    {
        Dino = _dino;
    }
}
