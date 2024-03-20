using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButtonTrigger : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject Tile;

    public void Triggered() 
    {
        GameSystem gameSystemScript = Canvas.GetComponent<GameSystem>();
        gameSystemScript.SetTileInput(Tile);
    }
}
