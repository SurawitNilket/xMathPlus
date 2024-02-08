using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBoard : MonoBehaviour
{
    public GameObject TileBlueprint;
    public Text TileText;
    public Color Color1, Color2;

    private Tile[,] spaceMatrix;
    private int boardSize;

    private Color tileColor;
    private int n;
    private float tileSize;
    private float tileShift;

    void Start()
    {

        GetDataFromTileGenerator();
        GetTileProperties();
        AddTiles();
    }

    private void GetDataFromTileGenerator()
    {
        spaceMatrix = GenerateTile.spaceMatrix;
        boardSize = GenerateTile.boardSize;
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
                    t.name = n.ToString();
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
                        TileText.text = spaceMatrix[x, y].value;
                    } 
                    else 
                    {
                        TileText.text = "";
                    }
                }
            }
        }
    }
}
