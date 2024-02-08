using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSize : MonoBehaviour
{
    public Text Size;

    public void Start()
    {
        changeSize(0);
    }

    public void changeSize(int x)
    {
        if (GenerateTile.boardSize + x >= 10 && GenerateTile.boardSize + x <= 20)
        {
            GenerateTile.boardSize += x;
            Size.GetComponent<Text>().text = GenerateTile.boardSize + "x" + GenerateTile.boardSize;
        }
    }
}
