using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Spell : MonoBehaviour
{
    [SerializeField]
    private GameObject numberHolderText;
    
    public int spellIndex;

    private static Canvas canvas;
    private GameObject myNumberHolderText;
    private RectTransform myRectTransform;

    [HideInInspector]
    public int Number { get; private set; }
    private Text text;

    void Start()
    {
        InstantiateNumberHolderText();
        Enable();
    }

    void OnEnable()
    {
        Enable();
    }

    void OnDisable()
    {
        Disable();
    }

    void Update()
    {
        myRectTransform.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void InstantiateNumberHolderText()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        myNumberHolderText = Instantiate(numberHolderText, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        myNumberHolderText.transform.SetParent(canvas.transform);
        myRectTransform = myNumberHolderText.GetComponent<RectTransform>();
        myRectTransform.localScale = new Vector3(1f, 1f, 1f);

        TMP_Text tmpObj = myNumberHolderText.GetComponent<TMP_Text>();
        tmpObj.color = new Color32(32, 121, 234, 255);
        tmpObj.text = " " + Number.ToString(); //Number < 10 ? " " + Number.ToString() : Number.ToString();
    }

    public void SetNumber(int _number)
    {
        Number = _number;
    }

    private void SetNumberHolderText()
    {
        if (myNumberHolderText != null)
            myNumberHolderText.GetComponent<TMP_Text>().text = Number < 10 ? " " + Number.ToString() : Number.ToString();
    }

    private void Enable()
    {
        Game game = GameManager.Instance.game;
        if (game != null)
        {
            if (game.spells != null && game.spells.Count >= spellIndex + 1)
            {
                SetNumber(game.spells[spellIndex]);
                //Debug.Log("Spell Number: " + Number); // DEBUG
            }
        }

        if (myNumberHolderText != null)
        {
            SetNumberHolderText();
            myNumberHolderText.SetActive(true);
        }
        //Debug.Log("Enabled"); // DEBUG
    }

    private void Disable()
    {
        SetNumber(0);

        if (myNumberHolderText != null)
        {
            myNumberHolderText.SetActive(false);
        }

        Debug.Log("Disabled"); // DEBUG
    }
}
