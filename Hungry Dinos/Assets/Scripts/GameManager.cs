using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game Fields
    enum Turns { PC, Player}
    int currentTurn;

    Game game;

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
    }

    void Start()
    {
        game = new Game("Easy"); //((Game.Difficulty).0).ToString()

        game.Start();
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
    }

    void Update()
    {

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
    
    // Board
    //public static Board Board { get; private set; }

    // Difficulty
    public enum Difficulty { Easy, Medium, Hard }
    private string difficulty;

    // Waves
    private Queue<int[]> waves;
    public int TotalNumberOfWaves { get; private set; }
    public int CurrentWaveCount { get; private set; }
    private int sumOfMonsters = 0;

    // Spells
    //private int[] spells = new int[3];
    List<int> spells = new List<int>();

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

    public void Start()
    {
        Board.Instance.NextWave(NextWave());
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
        int[] wave = waves.Peek();
        waves.Dequeue();
        CurrentWaveCount++;

        Debug.Log("Current wave count: " + CurrentWaveCount); // DEBUG
        DisplayWave(wave); // DEBUG

        return wave;
    }

    public int GenerateSpell(int _remainder)
    {
        if (_remainder <= 0 || spells.Count >= maxSpells)
            return 0;

        int spell = (spells.Count < (maxSpells - 1)) ? UnityEngine.Random.Range(1, _remainder + 1) : _remainder;

        AddSpell(spell);
        Debug.Log(spell); // DEBUG

        return GenerateSpell(_remainder - spell);
    }

    public void SpellAddition(int _spellIndexA, int _spellIndexB) // A-->B
    {
        spells[_spellIndexB] = spells[_spellIndexA] + spells[_spellIndexB];
        spells.RemoveAt(_spellIndexA);

        Debug.Log("After addition"); // DEBUG
        foreach (int spell in spells) // DEBUG
        {
            Debug.Log(spell);
        }
    }

    public void AddSpell(int _spell)
    {
        //if (spells.Count < maxSpells)
            spells.Add(_spell);
    }

    public void RemoveSpellAtIndex(int _spellIndex)
    {
        spells.RemoveAt(_spellIndex);

        // Get a new spell equal to a monster from the board, needs to be decoupled
        AddSpell(Board.Instance.GetRandomMonster());
    }

    public bool Victory()
    {
        if (waves.Count == 0 && Board.Instance.GetMonstersOnBoard().Count == 0)
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

    
    
    public int MonsterSum(List<int> _monstersOnBoard)
    {
        int monsterSum = 0;
        foreach (int monster in _monstersOnBoard)
        {
            monsterSum += monster;
        }
        return monsterSum;
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


