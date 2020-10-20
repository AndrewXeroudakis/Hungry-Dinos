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

    public static List<int> monstersOnBoard = new List<int>();

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

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new Cell(GetWorldPositionAt(new Vector2(x, y)));
            }
        }
    }

    public void SpawnWave(int[] _wave)
    {
        List<Vector2> newWavePositions = GenerateNewWavePositions(_wave.Length);
        InstantiateMonsters(_wave, newWavePositions);
        // Move Dinos, Set target
    }

    public void UpdateGrid()
    {

    }

    private void InstantiateMonsters(int[] _wave, List<Vector2> _newWavePositions)
    {
        for (int i = 0; i < _wave.Length; i++)
        {
            Vector2 worldPosition = GetWorldPositionAt(new Vector2(_newWavePositions[i].x + 2, _newWavePositions[i].y));
            GameObject newMonster = Instantiate(dino, worldPosition, Quaternion.identity);
            newMonster.GetComponent<Dino>().SetNumber(_wave[i]);
        }
    }

    public static int GetRandomMonster()
    {
        int randomIndex = UnityEngine.Random.Range(1, monstersOnBoard.Count + 1);

        return monstersOnBoard[randomIndex];
    }

    public static List<Vector2> GenerateNewWavePositions(int _waveSize)
    {
        List<Vector2> newWavePositions = new List<Vector2>();

        List<int> yPositions = new List<int> { 0, -1, -2, -3 };

        for (int i = 0; i < _waveSize; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, yPositions.Count);
            newWavePositions.Add(new Vector2(4f, yPositions[randomIndex]));
            yPositions.RemoveAt(randomIndex);
        }

        foreach (Vector2 vect in newWavePositions) // DEBUG
        {
            Debug.Log(vect);
        }

        return newWavePositions;
    }

    public Cell GetCellFromWorldPosition(Vector2 _worldPosition)
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
    public Vector2 center;
    public Dino dino = null;

    public Cell(Vector2 _center)
    {
        this.center = _center;
    }

    public void SetDino(Dino _dinoMovement)
    {
        dino = _dinoMovement;
    }
}
