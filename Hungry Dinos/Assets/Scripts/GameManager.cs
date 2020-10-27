using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game Fields
    enum Turns { PC, Player}
    int currentTurn;
    [HideInInspector]
    public Game game;

    // Mouse Controls
    private MouseControls mouseControls;
    Vector2 input_mousePosition;

    //Vector3 or; // DEBUG
    //Vector3 dir; // DEBUG

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
        
        //Debug.Log("I am a " + Instance.GetType());
        mouseControls = new MouseControls();
    }

    void OnEnable()
    {
        mouseControls.Enable();
    }

    void OnDisable()
    {
        mouseControls.Disable();
    }

    void Start()
    {
        //game = new Game("Easy"); //((Game.Difficulty).0).ToString()
        //game.StartNextRound();
        //Board.GenerateNewWavePositions(2);

        //game.DisplayWaves();
        /*for (int i = 0; i < game.TotalNumberOfWaves; i++)
        {
            game.NextWave();
        }*/

        //game.GenerateSpell(12);
        //game.SpellAddition(2, 0);
        //game.SpellAddition(1, 0);
        //game.GenerateSpell(12);
        StartNewGame("Hard");

        mouseControls.PlayerMouse.Position.performed += mP => input_mousePosition = mP.ReadValue<Vector2>();
        mouseControls.PlayerMouse.Select.performed += Select;
        mouseControls.PlayerMouse.NextWave.performed += NextWave;
    }

    void Update()
    {
        //Raycast(or, dir);
    }

    private void StartNewGame(string _difficulty)
    {
        Board.Instance.ResetBoard();
        game = new Game(_difficulty);
        game.StartNextRound();
    }

    private void NextWave(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(input_mousePosition);
        Spell spell = Board.Instance.SelectSpell(mousePos);
        int spellIndex = -1;
        if (spell != null)
            spellIndex = spell.spellIndex;
        if (spellIndex != -1)
        {
            //Debug.Log("Selected Spell: " + spellIndex);
            game.ReplaceSpellAtIndex(spellIndex);
        }


        game.StartNextRound();
    }

    private void Select(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(input_mousePosition);
        //mousePos.z = 0f;

        //Debug.Log("input_mousePosition: " + input_mousePosition); // DEBUG
        //Debug.Log("mousePos: " + mousePos); // DEBUG

        // Select Cell
        Vector2 cellCoordinates = Board.GetGridCoordinatesFromWorldPosition(mousePos);
        if (cellCoordinates != -Vector2.one)
        {
            Cell cellAtMousePosition = Board.Instance.GetCellFromCoordinates(cellCoordinates);

            Board.Instance.SelectCell(cellAtMousePosition);
            //Vector2 position = Board.GetWorldPositionAt(cell);

            /*Debug.Log("cellCoordinates: " + cellCoordinates); // DEBUG
            Debug.Log("cellAtMousePosition: " + cellAtMousePosition);*/
        }

        // Select Spell
        int spellIndexToRemove = Board.Instance.EvaluateSpell(Board.Instance.SelectSpell(mousePos));
        if (spellIndexToRemove > -1 && spellIndexToRemove < 3)
        {
            game.RemoveSpellAtIndex(spellIndexToRemove);
            game.StartNextRound();
        }
        else if (spellIndexToRemove >= 3) // Spell Addition
        {
            int decodedSpellIndex = spellIndexToRemove - 10;
            Debug.Log("decodedSpellIndex: " + decodedSpellIndex);
            game.SpellAddition(Board.selectedSpell.spellIndex, decodedSpellIndex);
            Board.DeselectSpell();
        }
    }

    private void Raycast(Vector3 _origin, Vector3 _direction)
    {
        Debug.DrawRay(_origin, _direction * 200, Color.red); //(_direction - _origin).normalized
    }
}

public class Game
{
    #region Readonly Static Fields
    readonly static int maxMonstersPerWave = 4;
    readonly static int minMonstersPerWave = 1;

    // Easy
    readonly static int maxMonsterNumberEasy = 5;
    readonly static int minMonsterNumberEasy = 1;
    readonly static int maxSumEasy = 50;
    readonly static int minSumEasy = 25;

    // Medium
    readonly static int maxMonsterNumberMedium = 10;
    readonly static int minMonsterNumberMedium = 1;
    readonly static int maxSumMedium = 100;
    readonly static int minSumMedium = 50;

    // Hard
    readonly static int maxMonsterNumberHard = 15;
    readonly static int minMonsterNumberHard = 1;
    readonly static int maxSumHard = 150;
    readonly static int minSumHard = 100;

    // Spells
    readonly static int maxSpells = 3;
    #endregion
    
    // Difficulty
    public enum Difficulty { Easy, Medium, Hard }
    private string difficulty;

    // Waves
    private Queue<int[]> waves;
    public int TotalNumberOfWaves { get; private set; }
    public int CurrentWaveCount { get; private set; }
    private int sumOfMonsters = 0;

    // Spells
    public List<int> spells = new List<int>();

    public Game(string _difficulty)
    {
        difficulty = _difficulty;
        this.Initialize();
    }

    void Initialize()
    {
        switch (difficulty)
        {
            case "Easy":
                {
                    waves = GenerateWaves(minMonsterNumberEasy, maxMonsterNumberEasy, minSumEasy, maxSumEasy);
                }
                break;
            case "Medium":
                {
                    waves = GenerateWaves(minMonsterNumberMedium, maxMonsterNumberMedium, minSumMedium, maxSumMedium);
                }
                break;
            case "Hard":
                {
                    waves = GenerateWaves(minMonsterNumberHard, maxMonsterNumberHard, minSumHard, maxSumHard);
                }
                break;
        }
    }

    public void StartNextRound()
    {
        if (!Board.Instance.NextWave(NextWave()))
        {
            Debug.Log("VICTORY!");
            if (Victory())
            {
                Board.Instance.victoryText.SetActive(true);
            }
        }
        RemoveZeroSpells();
        int monsterSum = Board.Instance.GetMonsterSum();
        int spellSum = GetSpellSum();
        Debug.Log("monsterSum: " + monsterSum); // DEBUG
        Debug.Log("spellSum: " + spellSum); // DEBUG
        GenerateSpell(monsterSum - spellSum);
        ResetSpellsOnBoard();
        //Board.Instance.ActivateSpells(spells);
    }

    Queue<int[]> GenerateWaves(int _minMonsterNumber, int _maxMonsterNumber, int _minSum, int _maxSum)
    {
        Queue<int[]> wavesToGenerate = new Queue<int[]>();

        int maxSum = UnityEngine.Random.Range(_minSum, _maxSum + 1);
        int sum = 0;

        while (sum < maxSum)
        {
            int maxMonstersInWave = UnityEngine.Random.Range(minMonstersPerWave, maxMonstersPerWave + 1);

            int[] wave = new int[maxMonstersInWave];
            wavesToGenerate.Enqueue(wave);
            TotalNumberOfWaves += 1;

            for (int i = 0; i < maxMonstersInWave; i++)
            {
                int monsterNumber = UnityEngine.Random.Range(_minMonsterNumber, _maxMonsterNumber + 1);
                wave[i] = monsterNumber;
                sum += monsterNumber;
            }
        }

        sumOfMonsters = sum;
        
        Debug.Log("The sum of monsters is: " + sumOfMonsters.ToString()); // DEBUG
        Debug.Log("The total waves are: " + TotalNumberOfWaves.ToString()); // DEBUG

        return wavesToGenerate;
    }

    public int[] NextWave()
    {
        int[] wave = null;

        if (waves.Count > 0)
        {
            wave = waves.Peek();
            waves.Dequeue();
            CurrentWaveCount++;

            Debug.Log("Current wave count: " + CurrentWaveCount); // DEBUG
            DisplayWave(wave); // DEBUG
        }

        return wave;
    }

    private void ResetSpellsOnBoard()
    {
        foreach (GameObject gameObjSpell in Board.Instance.spells)
        {
            gameObjSpell.SetActive(false);
        }

        for (int i = 0; i < spells.Count; i++)
        {
            Board.Instance.spells[i].SetActive(true);
        }
    }

    private int GenerateSpell(int _remainder)
    {
        if (_remainder <= 0 || spells.Count >= maxSpells)
            return 0;

        int spell = (spells.Count < (maxSpells - 1)) ? UnityEngine.Random.Range(1, _remainder + 1) : _remainder;

        AddSpell(spell);
        Debug.Log("Spell: " + spell); // DEBUG

        return GenerateSpell(_remainder - spell);
    }

    public void SpellAddition(int _spellIndexA, int _spellIndexB) // A-->B
    {
        spells[_spellIndexB] = spells[_spellIndexA] + spells[_spellIndexB];
        spells[_spellIndexA] = 0;
        //spells.RemoveAt(_spellIndexA);
        Spell spellToReset = Board.Instance.spells[_spellIndexB].gameObject.GetComponent<Spell>();
        spellToReset.SetNumber(spells[_spellIndexB]);
        spellToReset.SetNumberHolderText();

        Debug.Log("After addition"); // DEBUG
        foreach (int spell in spells) // DEBUG
        {
            Debug.Log(spell);
        }
    }

    private void RemoveZeroSpells()
    {
        /*List<int> indexesToRemove = new List<int>();

        for (int i = 0; i < spells.Count; i++)
        {
            if (spells[i].Equals(0))
            {
                indexesToRemove.Add(i);
            }
        }

        foreach (int index in indexesToRemove)
        {
            spells.RemoveAt(index);
            Debug.Log("Removed at index: " + index); // DEBUG
        }*/

        for (int i = 0; i < spells.Count; i++)
        {
            if (spells[i] == 0)
            {
                spells.RemoveAt(i);
                i--;
            }
        }
    }

    public void AddSpell(int _spell)
    {
        if (spells.Count < maxSpells)
            spells.Add(_spell);

        /*for (int i = 0; i < spellNumbers.Length; i++)
        {
            if (spellNumbers[i] == 0)
            {
                spellNumbers[i] = _spell;
            }
        }*/
    }

    public void RemoveSpellAtIndex(int _spellIndex)
    {
        spells.RemoveAt(_spellIndex);
    }

    public void ReplaceSpellAtIndex(int _spellIndex)
    {
        spells[_spellIndex] = Board.Instance.GetRandomMonster();
        ResetSpellsOnBoard();
        // Get a new spell equal to a monster from the board, needs to be decoupled
        //AddSpell(Board.Instance.GetRandomMonster());
    }

    private int GetSpellSum()
    {
        int spellSum = 0;
        foreach (int spell in spells)
        {
            spellSum += spell;
        }

        return spellSum;
    }

    public bool Victory()
    {
        if (Board.Instance.GetMonstersOnBoard().Count <= 0) //waves.Count <= 0 && 
            return true;
        return false;
    }

    public bool Defeat()
    {
        return false;
    }

    #region Debug
    public void DisplayWaves()
    {
        foreach (int[] wave in waves)
        {
            DisplayWave(wave);
        }
    }

    void DisplayWave(int[] _wave)
    {
        string waveString = "[" + String.Join(", ", _wave) + "]";
        Debug.Log(waveString);
    }
    #endregion
}

/*public sealed class Board
{
    #region Readonly Static Fields
    readonly static int width = 5;
    readonly static int height = 4;
    readonly static int cellSize = 24;
    //readonly static int[] entryXPositions = new int[4] { 0, 1, 2, 3 };
    #endregion

    #region Singleton
    private static Board instance = null;
    public static Board Instance
    {
        get
        {
            if (instance == null)
                instance = new Board();

            return instance;
        }
    }

    private Board() { }
    #endregion

    public static List<int> monstersOnBoard = new List<int>();

    //public static Dictionary<GameObject, Vector2> monsters = new Dictionary<GameObject, Vector2>();

    public static Stack<KeyValuePair<Dino, Vector2>> monstersToMove;

    //private static List<Vector2> newWavePositions;

    public static void Init()
    {

    }

    
    
    

    public static void UpdateBoard(List<Vector2> _newWavePositions)
    {
        if (_newWavePositions != null)
        {
            monstersToMove = new Stack<KeyValuePair<Dino, Vector2>>(); // GameObject?

            foreach (Vector2 newWavePosition in _newWavePositions)
            {
                // Instansiate new dino (at position outside of the board relative to newWavePosition)
                // Set dino number from wave index ? How to access?
                //monstersToMove.Push(new KeyValuePair<DinoMovement, Vector2>(new dino, newWavePosition));

                SetMonstersToMove(newWavePosition);
            }
        }
    }

    private static void SetMonstersToMove(Vector2 _positionToCheck)
    {
        if (_positionToCheck.y <= 0)
            return;

        Dino dinoMovement = new Dino(); // Get DinoMovement from node at _positionToCheck

        if (dinoMovement != null) // Check if board position has a monster
        {
            Vector2 positionToMove = new Vector2(_positionToCheck.x, _positionToCheck.y - 1);
            monstersToMove.Push(new KeyValuePair<Dino, Vector2>(dinoMovement, positionToMove));
            SetMonstersToMove(positionToMove);
        }
    }

    
}*/


