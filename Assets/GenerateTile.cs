using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenerateTile : MonoBehaviour
{
    public GameObject Canvas;

    public Toggle Addition;
    public Toggle Subtraction;
    public Toggle Multiplication;
    public Toggle Division;
    public Toggle Negative;
    public Toggle Variable;
    public Text Comment;

    // Variables
    public static bool addition = true;
    public static bool subtraction = true;
    public static bool multiplication;
    public static bool division;
    public static bool negative;
    public static bool variable;
    public static string difficulty;

    protected int difficultyValue = 0;
    protected int hasNegative;
    protected int tileCountRequired;

    public static int tileCount;
    public static int boardSize = 10;

    public static List<string> operatorList;
    public static Tile[,] spaceMatrix;
    // Methods

    public void Start()
    {
        ChangeDifficulty changeDifficultyScript = Canvas.GetComponent<ChangeDifficulty>();

        // Part 1
        if (addition == true) { Addition.isOn = true; }
        if (subtraction == true) { Subtraction.isOn = true; }
        if (multiplication == true) { Multiplication.isOn = true; }
        if (division == true) { Division.isOn = true; }
        if (negative == true) { Negative.isOn = true; }
        if (variable == true) { Variable.isOn = true; }
        // Difficulty
        if (difficulty == "Hard") { changeDifficultyScript.SetDifficulty("Hard"); }
        else if (difficulty == "Medium") { changeDifficultyScript.SetDifficulty("Medium"); }
        else { changeDifficultyScript.SetDifficulty("Easy"); }
    }

    // ==================== Start of Initializing Functions ====================

    public void check()
    {
        operatorList = new List<string>();
        tileCountRequired = (boardSize * boardSize) / 3;
        GetConfigurations();

        if (operatorList.Count == 0)
        {
            Comment.GetComponent<Text>().text = "Must include an operator in Operators Setting!";
            Comment.GetComponent<Text>().color = new Color32(255, 0, 0, 255);
            return;
        }
        Comment.GetComponent<Text>().text = "Generating a board, please wait...";
        Comment.GetComponent<Text>().color = new Color32(0, 127, 0, 255);

        Generate();
        SceneManager.LoadScene(2);
    }

    public void Generate()
    {
        for (int n = 0; n < 100; n++)
        {
            InitializeSpaceMatrix();
            tileCount = 0;
            InitializeFirstTile();
            while (CreateALine()) {}
            if (tileCount > tileCountRequired) { break; }
        }
    }

    public void GetConfigurations()
    {
        // Part 1
        if (Addition.isOn == true) { operatorList.Add("+"); addition = true; } else { addition = false; }
        if (Subtraction.isOn == true) { operatorList.Add("-"); subtraction = true; } else { subtraction = false; }
        if (Multiplication.isOn == true) { operatorList.Add("×"); multiplication = true; } else { multiplication = false; }
        if (Division.isOn == true) { operatorList.Add("÷"); division = true; }else { division = false; } 
        if (Negative.isOn == true) { hasNegative = 1; negative = true; } else { hasNegative = 0; negative = false; }
        if (Variable.isOn == true) { variable = true; } else { variable = false; }
        // Difficulty
        if (difficulty == "Hard") { difficultyValue = 2; }
        else if (difficulty == "Medium") { difficultyValue = 1; }
        else { difficultyValue = 0; }
    }

    public void InitializeSpaceMatrix()
    {
        spaceMatrix = new Tile[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Tile newTile = new Tile();
                spaceMatrix[x, y] = newTile;
                ModifyTile(newTile, "Void", x, y);
            }
        }
    }

    public void InitializeFirstTile()
    {
        int firstX = Random.Range(0, boardSize);
        int firstY = Random.Range(0, boardSize);
        while (true)
        {
            firstX = Random.Range(0, boardSize);
            firstY = Random.Range(0, boardSize);
            if ((firstX + firstY) % 2 == 0) { break; }
        }
        string firstValue = Random.Range(0, 10).ToString();

        ModifyTile(spaceMatrix[firstX, firstY], firstValue, firstX, firstY); // Create the first tile with random number or operator. initialized for line sketching
        spaceMatrix[firstX, firstY].isIntersected = false;
    }

    // ==================== End of Initializing Functions ====================



    // ==================== Start of Create Line Functions ====================

    public bool CreateALine()
    {

        // Add every non-void tile to list

        List<Tile> availableTileList = new List<Tile>();
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (spaceMatrix[i, j].value != "Void" && spaceMatrix[i, j].isIntersected == false)
                {
                    availableTileList.Add(spaceMatrix[i, j]);
                }
            }
        }

        // Get a random tile in list

        while (true)
        {
            Line line = new Line();

            if (availableTileList.Count == 0)
            {
                return false; // No more available tile to create a line ---> Stop all
            }

            int randomTileIndex = Random.Range(0, availableTileList.Count);
            line.pivot = availableTileList[randomTileIndex];
            availableTileList[randomTileIndex].isPivot = true;
            bool pass = TestLinePlacement(line);
            availableTileList[randomTileIndex].isPivot = false;
            if (pass == true)
            {
                Eliminate(line);
                LineToTiles(line);
                break;
            }
            availableTileList.RemoveAt(randomTileIndex);
        }

        return true;
    }

    public bool TestLinePlacement(Line line)
    {
        // Get type of line (Vertical or Horizontal)

        int neighborX = GetNeighbors(line.pivot, "X");
        int neighborY = GetNeighbors(line.pivot, "Y");
        if (neighborX > neighborY) { line.type = "V"; }
        else { line.type = "H"; }

        // Get start point and end point of the line

        int l; // Length of the line (0 = 5 tiles, 1 = 7 tiles, 2 = 9 tiles)
        int s; // Shift from pivot to the end of the line (0 = pivot, 1 = +2)
        int retries = 0;

        while(true) // Test line's length and placement
        {
            l = Random.Range(0, difficultyValue + 1);

            if (line.pivot.type == "Number") { s = Random.Range(0, l + 3) * 2; }
            else { s = (Random.Range(0, l + 2) * 2) + 1; }

            if (line.type == "H") { line.p2 = line.pivot.x + s; }
            else { line.p2 = line.pivot.y + s; }
            line.p1 = line.p2 - (l * 2) - 4;

            if (line.p1 >= 0 && line.p1 <= boardSize - 1 && line.p2 >= 0 && line.p2 <= boardSize - 1) { break; }
            else { retries++; }

            if (retries == 100) { return false; }
        }

        // Check line's surrounding tile neighbors
        for (int i = line.p1; i <= line.p2; i++)
        {
            if (line.type == "H")
            {
                if (CheckTile(line, spaceMatrix[i, line.pivot.y], "X", "Y") == false) { return false; };
            }
            else
            {
                if (CheckTile(line, spaceMatrix[line.pivot.x, i], "Y", "X") == false) { return false; };
            }
        }
        
        // Check for equal sign
        int nEqual = 0;
        int nVoid = 0;
        for (int i = 0; i < line.stringList.Count; i++)
        {
            if (i % 2 == 1)
            {
                if (line.stringList[i] == "=") { nEqual++; }
                if (line.stringList[i] == "Void") { nVoid++; }
            }
        }

        if (nEqual > 1) { return false; }
        if (nEqual == 0) 
        { 
            if (nVoid == 0)
            {
                return false;
            }
            else
            {
                List<int> tempIndexList = new List<int>();
                for (int i = 0; i < line.stringList.Count; i++)
                {
                    if (i % 2 == 1 && line.stringList[i] == "Void")
                    {
                        tempIndexList.Add(i);
                    }
                }
                if (tempIndexList.Count == 0) { return false; }
                line.stringList[tempIndexList[Random.Range(0, tempIndexList.Count)]] = "=";
            }
        }

        for (int n = 1; n < 10; n++)
        {
            if (CreateAnEquation(line) == true) { return true; }
        }

        return false;
    }

    public bool CheckTile(Line line, Tile tile, string t1, string t2)
    {
        line.stringList.Add(tile.value);
        if (tile.value == "Void")
        {
            if (GetNeighbors(tile, t2) > 0) { return false; }
            if (GetNeighbors(tile, t1) >= 1) { return false; }
        }
        else if (tile.isIntersected == true)
        {
            return false;
        }

        return true;
    }

    public int GetNeighbors(Tile t, string direction)
    {
        int n = 0;
        if (direction == "X")
        {
            if (t.x + 1 <= boardSize - 1) { if (spaceMatrix[t.x + 1, t.y].value != "Void" && spaceMatrix[t.x + 1, t.y].isPivot == false) { n++; } }
            if (t.x - 1 >= 0) { if (spaceMatrix[t.x - 1, t.y].value != "Void" && spaceMatrix[t.x - 1, t.y].isPivot == false) { n++; } }
        }
        else
        {
            if (t.y + 1 <= boardSize - 1) { if (spaceMatrix[t.x, t.y + 1].value != "Void" && spaceMatrix[t.x, t.y + 1].isPivot == false) { n++; } }
            if (t.y - 1 >= 0) { if (spaceMatrix[t.x, t.y - 1].value != "Void" && spaceMatrix[t.x, t.y - 1].isPivot == false) { n++; } }
        }
        return n;
    }

    // ==================== End of Create Line Functions ====================



    // ==================== Start of Create Equation Functions ====================

    public bool CreateAnEquation(Line line)
    {
        List<string> e1 = new List<string>();
        List<string> e2 = new List<string>();

        bool equalTriggered = false;
        for (int i = 0; i < line.stringList.Count; i++)
        {
            if (line.stringList[i] == "=")
            {
                equalTriggered = true;
            }
            else
            {
                if (equalTriggered == false)
                {
                    e1.Add(line.stringList[i]);
                }
                else
                {
                    e2.Add(line.stringList[i]);
                }
            }
        }

        List<string> _e1 = Calculate(e1);
        List<string> _e2 = Calculate(e2);
        if (_e1[0] == "Break" || _e2[0] == "Break") {return false;}

        // Prevents solving for X in a >5 lines equation (Too Hard)
        if (_e1.Count >= 5) {if (_e2.Count == 1) {return false;} } 
        if (_e2.Count >= 5) {if (_e1.Count == 1) {return false;} } 

        if (_e1.Count == 3) { _e1 = Assign(_e1, false); e2 = Assign(e2, true); _e2 = Calculate(e2); if (_e2[0] == "Break") {return false;} else {e1 = SolveForX(_e1, _e2[0]);}}
        else if (_e2.Count == 3) { e1 = Assign(e1, true); _e2 = Assign(_e2, false); _e1 = Calculate(e1); if (_e1[0] == "Break") {return false;} else {e2 = SolveForX(_e2, _e1[0]);}}
        else {return false;}

        if (e1[0] == "Break" || e2[0] == "Break") {return false;}

        for (int i = 0; i < e1.Count; i++)
        {
            line.stringList[i] = e1[i];
        }
        for (int i = 0; i < e2.Count; i++)
        {
            line.stringList[i + e1.Count + 1] = e2[i];
        }

        return true;
    }

    
    public List<string> Assign(List<string> e, bool fillsAllNumber)
    {
        if (e.Count == 1 && e[0] == "Void") { e[0] = Random.Range(-9 * hasNegative, 10).ToString(); }

        List<int> variableIndexList = new List<int>();
        for (int i = 0; i < e.Count; i++)
        {
            if (e[i] == "Void")
            {
                if (i % 2 == 1)
                {
                    e[i] = operatorList[Random.Range(0, operatorList.Count)];
                }
                else
                {
                    variableIndexList.Add(i);
                }
            }
        }
        if (fillsAllNumber == false)
        {
            int randomVariableIndex = variableIndexList[Random.Range(0, variableIndexList.Count)];
            e[randomVariableIndex] = "X";
        }
        
        for (int i = 0; i < e.Count; i++)
        {
            if (i % 2 == 1)
            {
                if (e[i - 1] == "Void")
                {
                    e[i - 1] = Random.Range(-9 * hasNegative, 10).ToString();
                }
                if (e[i + 1] == "Void")
                {
                    e[i + 1] = Random.Range(-9 * hasNegative, 10).ToString();
                }
            }
        }

        return e;
    }

    public List<string> Calculate(List<string> e)
    {
        bool everyTileAssigned = true;
        for (int i = 0; i < e.Count; i++)
        {
            if (e[i] == "Void")
            {
                everyTileAssigned = false;
            }
        }

        if (everyTileAssigned == true)
        {
            if (e.Count == 1) {return e;}

            List<string> _e = new List<string>();
            for (int i = 0; i < e.Count; i++) { _e.Add(e[i]); }

            _e = CalculateEachGroup(_e, "×");
            _e = CalculateEachGroup(_e, "÷");
            if (_e[0] == "Break") { return _e; }
            _e = CalculateEachGroup(_e, "+");
            _e = CalculateEachGroup(_e, "-");

            return (_e);
        }

        return e;
    }

    public string CalculateEquation(List<string> e)
    {
        if (e.Count == 1) {return e[0];}

        List<string> _e = new List<string>();
        for (int i = 0; i < e.Count; i++)
        {
            _e.Add(e[i]);
        }

        _e = CalculateEachGroup(_e, "×");
        _e = CalculateEachGroup(_e, "÷");
        if (_e[0] == "Break") {return _e[0];}
        _e = CalculateEachGroup(_e, "+");
        _e = CalculateEachGroup(_e, "-");

        return _e[0];
    }

    public List<string> CalculateEachGroup(List<string> e, string operation)
    {
        while (true)
        {
            bool isAvailable = false;
            for (int i = 0; i < e.Count; i++)
            {
                float f = 0;
                string result = "Null";
                if (e[i] == operation)
                {
                    float a = float.Parse(e[i - 1]);
                    float b = float.Parse(e[i + 1]);
                    if (operation == "×") { f = a * b; }
                    if (operation == "÷") 
                    { 
                        if (b != 0) 
                        {
                            f = a / b;
                            if (f != (int)f) { e[0] = "Break"; break;}
                        } 
                        else 
                        {
                            e[0] = "Break"; 
                            break;
                        }
                    }
                    if (operation == "+") { f = a + b; }
                    if (operation == "-") { f = a - b; }

                    result = ((int)f).ToString();
                }
                if (result != "Null")
                {
                    e[i - 1] = result;
                    e.RemoveAt(i + 1);
                    e.RemoveAt(i);
                    isAvailable = true;
                    break;
                }
            }
            if (isAvailable == false)
            {
                break;
            }
        }
        return e;
    }

    public List<string> SolveForX(List<string> e, string r)
    {
        float f = (float)(int.Parse(r));
        float x = (float)f;
        int a = 0;
        int b = 0;
        if (e.Count != 1)
        {
            if (e[0] != "X") {a = int.Parse(e[0]);}
            if (e[2] != "X") {b = int.Parse(e[2]);}
            if (e[1] == "+") { if (e[0] == "X") { x = f - b; } else { x = f - a; } } 
            if (e[1] == "-") { if (e[0] == "X") { x = f + b; } else { x = a - f; } } 
            if (e[1] == "×") { if (e[0] == "X") { if (b != 0) {x = f / b; } else {e[0] = "Break";}} else { if (a != 0) {x = f / a;} else {e[0] = "Break";}} } 
            if (e[1] == "÷") { 
                if (e[0] == "X") { 
                    x = f * b;
                } 
                else 
                { 
                    x = a / f;
                } 
                if (x == 0 && b == 0) {e[0] = "Break";}
            } 
        }
        int int_x = (int)x;
        if (int_x != x) {e[0] = "Break";}
        if (negative == false) {if (x < 0) {e[0] = "Break";}}
        if (x > 100) {e[0] = "Break";}
        if (e[0] == "X") { e[0] = x.ToString(); }
        else { e[2] = x.ToString(); }
        return e;
    }

    // ==================== End of Create Equation Functions ====================



    // ==================== Start of Eliminating Functions ====================

    public void Eliminate(Line line)
    {
        List<int> stringIndexList = new List<int>();
        for (int i = 0; i < line.stringList.Count; i++)
        {
            if (i % 2 == 0)
            {
                stringIndexList.Add(i);
            }
        }
        int randomStringIndex = stringIndexList[Random.Range(0, stringIndexList.Count)];
        line.blankedIndex = randomStringIndex;
    }

    // ==================== End of Eliminating Functions ====================



    // ==================== Start of Modifier Functions ====================
    
    public void ModifyTile(Tile tile, string newValue, int x, int y) // Change tile's property
    {
        if ((x + y) % 2 == 0) { tile.type = "Number"; }
        else { tile.type = "Operator"; }
        tile.x = x;
        tile.y = y;
        tile.value = newValue;
        tileCount++;
    }

    public void LineToTiles(Line line) // Change line's property
    {
        int i = 0;
        for (int p = line.p1; p <= line.p2; p++)
        {
            if (line.type == "H")
            {
                if (spaceMatrix[p, line.pivot.y].value == "Void")
                {
                    ModifyTile(spaceMatrix[p, line.pivot.y], line.stringList[i], p, line.pivot.y);
                }
                else
                {
                    spaceMatrix[p, line.pivot.y].isIntersected = true;
                }
                if (i == line.blankedIndex) { spaceMatrix[p, line.pivot.y].isBlank = true; }
            }
            else
            {
                if (spaceMatrix[line.pivot.x, p].value == "Void")
                {
                    ModifyTile(spaceMatrix[line.pivot.x, p], line.stringList[i], line.pivot.x, p);
                }
                else
                {
                    spaceMatrix[line.pivot.x, p].isIntersected = true;
                }
                if (i == line.blankedIndex) { spaceMatrix[line.pivot.x, p].isBlank = true; }
            }
            i++;
        }
    }

    // ==================== End of Modifier Functions ====================

}

public class Tile
{
    public int x;
    public int y;
    public string value = "Void";
    public string type = "Void";
    public bool isIntersected = false;
    public bool isPivot = false;
    public bool isBlank = false;
}

public class Line
{
    public Tile pivot;
    public List<string> stringList = new List<string>();
    public string type;
    public int p1;
    public int p2;
    public int blankedIndex;
}
