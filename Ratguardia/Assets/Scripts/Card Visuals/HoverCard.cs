using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCard : MonoBehaviour
{
    public float hoverOffset = 2.5f; // y offset
    Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void OnMouseEnter() // adjust y position when mouse hovers over card
    {
        Debug.Log("hovering over card");
        Vector3 pos = new Vector3(originalPosition.x, originalPosition.y + hoverOffset, originalPosition.z);
        transform.position = pos;
    }

    private void OnMouseExit() // return to normal when mouse leaves card
    {
        transform.position = originalPosition;
    }
}
