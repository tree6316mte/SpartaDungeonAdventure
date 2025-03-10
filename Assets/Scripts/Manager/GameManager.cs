using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public PlayerController Player;

    public void FixedUpdate()
    {
        if (Player.transform.position.y <= -100f)
        {
            Player.transform.position = Vector3.zero;
        }
    }
}
