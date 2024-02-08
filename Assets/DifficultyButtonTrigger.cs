using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyButtonTrigger : MonoBehaviour
{
    public GameObject Canvas;

    public void Triggered(string difficulty) 
    {
        ChangeDifficulty changeDifficultyScript = Canvas.GetComponent<ChangeDifficulty>();
        changeDifficultyScript.SetDifficulty(difficulty);
    }
}
