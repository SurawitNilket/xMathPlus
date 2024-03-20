using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckButtonTrigger : MonoBehaviour
{
    public GameObject Canvas;

    public void Triggered() 
    {
        GameSystem gameSystemScript = Canvas.GetComponent<GameSystem>();
        gameSystemScript.Finish();
    }
}
