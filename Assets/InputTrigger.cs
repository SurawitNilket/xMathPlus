using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTrigger : MonoBehaviour
{
    public GameObject Input;

    public void Triggered() 
    {
        GameSystem gameSystemScript = GameObject.Find("Board").GetComponent<GameSystem>();
        gameSystemScript.Input(Input.name);
    }
}
