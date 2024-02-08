using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficulty : MonoBehaviour
{
    public Button Easy;
    public Button Medium;
    public Button Hard;
    public Color DefaultColor, EasyColor, MediumColor, HardColor;

    public void SetDifficulty(string difficulty)
    {
        Easy.GetComponent<Image>().color = DefaultColor;
        Medium.GetComponent<Image>().color = DefaultColor;
        Hard.GetComponent<Image>().color = DefaultColor;

        if (difficulty == "Easy")
        {
            Easy.GetComponent<Image>().color = EasyColor;
            GenerateTile.difficulty = "Easy";
        }
        else if (difficulty == "Medium")
        {
            Medium.GetComponent<Image>().color = MediumColor;
            GenerateTile.difficulty = "Medium";
        }
        else
        {
            Hard.GetComponent<Image>().color = HardColor;
            GenerateTile.difficulty = "Hard";
        }
    }
}