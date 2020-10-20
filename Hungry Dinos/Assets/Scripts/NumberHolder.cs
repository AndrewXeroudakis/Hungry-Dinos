using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberHolder : MonoBehaviour
{
    [SerializeField]
    private Sprite numberHolder;
    [SerializeField]
    private Sprite numberHolderx2;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject numberHolderText;

    private static Canvas canvas;
    private GameObject myNumberHolderText;
    private RectTransform myRectTransform;

    private Transform parentDino;
    private int number = 0;
    private Text text;

    void Awake()
    {
        
    }

    void Start()
    {
        parentDino = this.transform.parent;
        number = parentDino.gameObject.GetComponent<Dino>().Number;

        InstantiateNumberHolderText();
    }

    void Update()
    {
        myRectTransform.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    void OnDisable()
    {
        Destroy(myNumberHolderText.gameObject);
    }

    private void InstantiateNumberHolderText()
    {
        if (number < 10)
            spriteRenderer.sprite = numberHolder;
        else
            spriteRenderer.sprite = numberHolderx2;
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        myNumberHolderText = Instantiate(numberHolderText, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        myNumberHolderText.transform.SetParent(canvas.transform); //myNumberHolderText.transform.parent = canvas.transform;
        myRectTransform = myNumberHolderText.GetComponent<RectTransform>();
        myRectTransform.localScale = new Vector3(1f, 1f, 1f);

        myNumberHolderText.GetComponent<TMP_Text>().text = number < 10 ? " " + number.ToString() : number.ToString();
    }
}
