using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHandLayout : MonoBehaviour
{
    // adjusts transform of the opponent's hand based on which they are
    public void SetLayout(int playerNum)
    {
        if (playerNum < 1 || playerNum > 3)
        {
            Debug.Log("incorrect player index: " + playerNum);
            return;
        }

        // position and rotation
        Vector3 pos = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch(playerNum)
        {
            case 1: // left player
                pos.x = -8.8f;
                pos.y = 1.0f;
                rot.z = -90.0f;
                break;
            case 2: // top player
                pos.x = 0f;
                pos.y = 5.9f;
                rot.z = 180.0f;
                break;
            case 3: // right player
                pos.x = 8.8f;
                pos.y = 1.0f;
                rot.z = 90.0f;
                break;
            default:
                break;
        }
    }
}
