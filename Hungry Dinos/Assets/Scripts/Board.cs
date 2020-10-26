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
    readonly static Vector2[] directions = {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };
    #endregion

    public Cell[,] grid;
    [SerializeField]
    private GameObject[] dinos;
    [SerializeField]
    private GameObject selection;

    public static Stack<KeyValuePair<Dino, Vector2>> monstersToMove;
    
    private List<GameObject> selections;

    public static List<Cell> selectedCells;
    public static Spell selectedSpell;

    public GameObject[] spells;
    public GameObject[] tents;
    private Collider2D[] spellColliders;

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
        //ActivateTents();
        monstersToMove = new Stack<KeyValuePair<Dino, Vector2>>();
        selections = new List<GameObject>();
        selectedCells = new List<Cell>();
        selectedSpell = null;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new Cell(GetWorldPositionAt(new Vector2(x, -y)), x, y);
                //Debug.Log("Coordinates: " + x + ", " + y + " --> Center: " + grid[x, y].Center); // DEBUG
                //Instantiate(new GameObject(), grid[x, y].Center, Quaternion.identity);
            }
        }

        if (spells != null)
        {
            spellColliders = new Collider2D[spells.Length];

            for (int i = 0; i < spells.Length; i++)
            {
                if (spells[i] != null)
                {
                    spellColliders[i] = spells[i].GetComponent<Collider2D>();
                }
            }
        }
    }

    public void NextWave(int[] _wave)
    {
        if (_wave != null)
        {
            List<Vector2> newWaveCoordinates = GenerateNewWaveCoordinates(_wave.Length);
            InstantiateAndGetMonstersToMove(_wave, newWaveCoordinates);
            MoveMonsters();
        }
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

    private Dino InstantiateMonster(int _number, Vector2 _newWavePosition)
    {
        Vector2 worldPosition = GetWorldPositionAt(new Vector2(_newWavePosition.x + 2, _newWavePosition.y));

        GameObject dino;
        if (_number <= 4)
        {
            dino = dinos[0];
        }
        else if (_number > 4 && _number <= 9)
        {
            dino = dinos[1];
        }
        else if (_number > 9 && _number <= 14)
        {
            dino = dinos[2];
        }
        else
        {
            dino = dinos[3];
        }

        GameObject newMonster = Instantiate(dino, worldPosition, Quaternion.identity);
        Dino newDino = newMonster.GetComponent<Dino>();
        newDino.SetNumber(_number);

        return newDino;
    }

    private void InstantiateSelection(Vector2 _worldPosition)
    {
        GameObject selectionObj = Instantiate(selection, _worldPosition, Quaternion.identity);
        selections.Add(selectionObj);
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
            Dino dino = keyValuePair.Key;
            Vector2 coordinatesToMove = keyValuePair.Value;
            Cell cell = grid[(int)coordinatesToMove.x, Mathf.Abs((int)coordinatesToMove.y)];
            cell.SetDino(dino);
            if (coordinatesToMove.x == 0)
                dino.SetTarget(new Vector2(cell.Center.x - 24, cell.Center.y));
            else
                dino.SetTarget(cell.Center);
            //Debug.Log((int)cellCenter.x + ", " + (int)cellCenter.y); // DEBUG
            
        }

        monstersToMove.Clear();
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

    public Cell GetCellFromCoordinates(Vector2 _cellCoordinates)
    {
        return grid[(int)_cellCoordinates.x, Mathf.Abs((int)_cellCoordinates.y)];
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

    public void SelectCell(Cell _cell)
    {
        if (selectedCells.Contains(_cell))
        {
            //selectedCells.Remove(_cell);
            DeselectCells();
            return;
        }
        
        if (_cell.Dino != null)
        {
            if (selectedCells.Count <= 0)
            {
                selectedCells.Add(_cell);
                //Debug.Log("Selected Cell: [" + _cell.gridX + ", " + _cell.gridY + "]"); // DEBUG
                InstantiateSelection(_cell.Center);
                return;
            }
            else
            {
                List<Cell> neighbours = GetNeighbours(_cell);

                foreach (Cell cell in neighbours)
                {
                    if (selectedCells.Contains(cell))
                    {
                        selectedCells.Add(_cell);
                        //Debug.Log("Selected Cell: [" + _cell.gridX + ", " + _cell.gridY + "]"); // DEBUG
                        InstantiateSelection(_cell.Center);
                    }
                }
            }
        }
    }

    public void DestroySelectedDinos()
    {
        foreach (Cell cell in selectedCells)
        {
            if (cell.Dino != null)
            {
                Destroy(cell.Dino.gameObject);
                cell.SetDino(null);
            }
        }
        DeselectCells();
    }

    private List<Cell> GetNeighbours(Cell _cell)
    {
        List<Cell> neighbours = new List<Cell>();

        foreach (Vector2 direction in directions)
        {
            Vector2 gridCoordinates = new Vector2(_cell.gridX, _cell.gridY) + direction;
            
            if (gridCoordinates.x < 1
                || gridCoordinates.x >= width
                || gridCoordinates.y < 0
                || gridCoordinates.y >= height)
                continue;
            
            Cell neighbourCell = GetCellFromCoordinates(gridCoordinates);
            if (neighbourCell.Dino == null)
                continue;

            //Debug.Log("gridCoordinates: " + gridCoordinates); // DEBUG

            neighbours.Add(neighbourCell);
        }

        return neighbours;
    }

    public Spell SelectSpell(Vector2 _mousePos)
    {
        Spell spell;

        foreach (Collider2D col in spellColliders)
        {
            if (col != null && col.OverlapPoint(_mousePos))
            {
                spell = col.gameObject.GetComponent<Spell>();
                return spell;
            }
        }
        
        return null;
    }

    public int EvaluateSpell(Spell _spell)
    {
        if (_spell != null)
        {
            if (selectedCells.Count > 0)
            {
                if (_spell.Number == SelectedMonsterSum()) // Match and Destroy
                {
                    DestroySelectedDinos();
                    _spell.gameObject.SetActive(false);
                    return _spell.spellIndex;
                }
                else // Deselect all
                {
                    DeselectCells();
                    //DeselectSpells();
                }
            }
            else // Select spell
            {
                if (selectedSpell != null && selectedSpell == _spell)
                {
                    DeselectSpell();
                    DestroySelections();
                }
                else if (selectedSpell != null && selectedSpell != _spell) // Add spells
                {
                    selectedSpell.gameObject.SetActive(false);
                    DeselectCells();
                    return _spell.spellIndex + 10;
                }
                else // Select spell
                {
                    selectedSpell = _spell;
                    InstantiateSelection(selectedSpell.transform.position);
                } 
            }
        }

        return -1;
    }

    private void DeselectCells()
    {
        selectedCells.Clear();
        DestroySelections();
    }

    public static void DeselectSpell()
    {
        selectedSpell = null;
    }

    private void DestroySelections()
    {
        foreach (GameObject selection in selections)
        {
            Destroy(selection.gameObject);
        }
    }

    public int SelectedMonsterSum()
    {
        int selectedMonsterSum = 0;
        foreach (Cell cell in selectedCells)
        {
            if (cell.Dino != null)
            {
                selectedMonsterSum += cell.Dino.Number;
            }
        }
        return selectedMonsterSum;
    }

    public int GetMonsterSum()
    {
        int monsterSum = 0;
        foreach (Dino monster in GetMonstersOnBoard())
        {
            if (monster != null)
            {
                monsterSum += monster.Number;
            }
        }
        return monsterSum;
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

    public void ActivateSpells(List<int> _spells)
    {
        for (int i = 0; i < _spells.Count; i++)
        {
            if (spells[i] != null && !spells[i].activeInHierarchy)
                spells[i].SetActive(true);
        }
    }

    public void ActivateTents()
    {
        foreach (GameObject tent in tents)
        {
            tent.SetActive(true);
        }
    }
}

public class Cell
{
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    public Vector2 Center { get; private set; }
    public Dino Dino { get; private set; }

    public Cell(Vector2 _center, int _gridX, int _gridY)
    {
        this.Center = _center;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }

    public void SetDino(Dino _dino)
    {
        Dino = _dino;
    }
}
