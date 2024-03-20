using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public GameObject TileBlueprint, Blocker, Numpad, Results, CheckButton;
    public Text TileText, TimerText, CheckText;
    public Color Color1, Color2, Color3, Color4, CorrectColor, IncorrectColor;

    private Tile[,] spaceMatrix;
    private int boardSize;
    private string operations;
    private string options;

    private Color tileColor;
    private float tileSize;
    private float tileShift;

    private float time;
    private int sec;
    private int min;
    private string timeFormat;

    public GameObject currentTile;
    public Color currentTileColor;

    public GameObject r1, r2, r3, r4, r5, r6, r7, r8;

    private bool hasFinished = true;
    private int nInput;
    private int nCorrect;
    private double score;

    void Start()
    {
        Initialize();
        GetDataFromTileGenerator();
        GetTileProperties();
        AddTiles();
    }

    private void Initialize() {
        time = 0;
        sec = 0;
        min = 0;
        nInput = 0;
        nCorrect = 0;

        Numpad.transform.localPosition = new Vector3(0, -600, 0);
        Blocker.transform.localPosition = new Vector3(0, -600, 0);
        Results.transform.localPosition = new Vector3(0, 5000, 0);
        CheckButton.transform.localPosition = new Vector3(-450, 875, 0);
        TimerText.transform.localPosition = new Vector3(0, 675, 0);
        CheckText.transform.localPosition = new Vector3(-225, 750, 0);

        hasFinished = false;
    }

    private void GetDataFromTileGenerator()
    {
        spaceMatrix = GenerateTile.spaceMatrix;
        boardSize = GenerateTile.boardSize;
        operations = "";
        options = "";
        if (GenerateTile.addition == true) {operations = operations + " +";}
        if (GenerateTile.subtraction == true) {operations = operations + " -";}
        if (GenerateTile.multiplication == true) {operations = operations + " ×";}
        if (GenerateTile.division == true) {operations = operations + " ÷";}
        if (GenerateTile.negative == true) {options = options + " Negative";}
        if (GenerateTile.variable == true) {options = options + " Variable";}
    }

    private void GetTileProperties()
    {
        tileSize = 1 / (float)boardSize;
    }

    private void AddTiles()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (spaceMatrix[x, y].value != "Void")
                {
                    GameObject t = GameObject.Instantiate(TileBlueprint);
                    t.transform.SetParent(transform);
                    t.transform.localScale = new Vector3(tileSize, tileSize, 0);
                    t.transform.localPosition = new Vector3(-500 + (tileSize * 500) + (tileSize * x * 1000), 500 - (tileSize * 500) - (tileSize * y * 1000), 0);
                    TileText = t.GetComponentInChildren<Text>();
                    if ((x + y) % 2 == 0)
                    {
                        t.GetComponent<Image>().color = Color1;
                    }
                    else
                    {
                        t.GetComponent<Image>().color = Color2;
                    }
                    if (spaceMatrix[x, y].isBlank == false) { 
                        t.transform.GetChild(0).GetComponent<Text>().text = spaceMatrix[x, y].value;
                        t.name = "Given";
                    } 
                    else 
                    {
                        t.transform.GetChild(0).GetComponent<Text>().text = "";
                        t.transform.GetChild(1).GetComponent<Text>().text = spaceMatrix[x, y].value;
                        t.transform.GetChild(2).GetComponent<Text>().text = spaceMatrix[x, y].group;
                        t.GetComponent<Image>().color = Color4;
                        t.name = "Input";
                    }
                }
            }
        }
    }

    public void SetTileInput(GameObject t) 
    {
        if (hasFinished == false) {
        if (currentTile != null)
        {
            currentTile.GetComponent<Image>().color = currentTileColor;
        }
        currentTileColor = t.GetComponent<Image>().color;
        t.GetComponent<Image>().color = Color3;
        currentTile = t;

        if (t.name == "Input")
        {
            Blocker.transform.localPosition = new Vector3(0, -6000, 0);
        }
        else
        {
            Blocker.transform.localPosition = new Vector3(0, -600, 0);
        }
        }
    }

    public void Input(string input)
    {
        string s = currentTile.transform.GetChild(0).GetComponent<Text>().text;
            if (input == "Erase")
            {
                if (s.Length > 0)
                {
                    s = s.Remove(s.Length - 1);
                }
            }
            else if (input == "Negative")
            {
                if (s.Contains("-"))
                {
                    s = s.Remove(0, 1);
                }
                else if (s.Length <= 2)
                {
                    s = "-" + s;
                }
            }
            else
            {
                int nV = 0;
                if (s.Contains("-"))
                {
                    nV = 1;
                }
                if (s.Length < 2 + nV)
                {
                    s = s + input;
                }
            }
            currentTile.transform.GetChild(0).GetComponent<Text>().text = s;
    }

    public void Finish() 
    {
        if (hasFinished == false) {
        hasFinished = true;
        Numpad.transform.localPosition = new Vector3(0, 5000, 0);
        Blocker.transform.localPosition = new Vector3(0, 5000, 0);
        CheckButton.transform.localPosition = new Vector3(0, 5000, 0);
        TimerText.transform.localPosition = new Vector3(0, 5000, 0);
        CheckText.transform.localPosition = new Vector3(0, 5000, 0);
        if (currentTile != null)
        {
            currentTile.GetComponent<Image>().color = currentTileColor;
        }
        foreach (Transform t in transform)
        {
            if (t.name == "Input")
            {
                nInput++;
                if (t.transform.GetChild(0).GetComponent<Text>().text == t.transform.GetChild(1).GetComponent<Text>().text) 
                {
                    t.transform.GetChild(0).GetComponent<Text>().color = CorrectColor;
                    nCorrect++;
                }
                else
                {
                    t.transform.GetChild(0).GetComponent<Text>().color = IncorrectColor;
                    if (t.transform.GetChild(0).GetComponent<Text>().text == "") {
                        t.transform.GetChild(0).GetComponent<Text>().text = "X";
                    }
                }
            }
        }
        }

        Results.transform.localPosition = new Vector3(0, -675, 0);
        r1.GetComponent<Text>().text = operations;
        r2.GetComponent<Text>().text = options;
        r3.GetComponent<Text>().text = GenerateTile.difficulty;
        r4.GetComponent<Text>().text = boardSize + "x" + boardSize;
        r5.GetComponent<Text>().text = timeFormat;
        r6.GetComponent<Text>().text = nCorrect.ToString();
        r7.GetComponent<Text>().text = (nInput - nCorrect).ToString();
        r8.GetComponent<Text>().text = Mathf.Round(((float)nCorrect / (float)nInput) * 100).ToString() + "%";



        CalculateResults();
    }

    private void CalculateResults()
    {

    }

    void Update()
    {
        if (hasFinished == false) 
        {
        time += Time.deltaTime;
        sec = ((int)time % 60);
        min = ((int)time / 60);
        timeFormat = string.Format("{0:00}:{1:00}",min,sec);
        TimerText.GetComponent<Text>().text = timeFormat;
        }
    }
}
