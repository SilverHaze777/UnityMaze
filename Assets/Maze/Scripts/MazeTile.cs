using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
    /// <summary>
    /// Used to track if tile has been visited
    /// </summary>
    public bool visited = false;

    /// <summary>
    /// Refrence to set floor color
    /// </summary>
    public MeshRenderer Floor;

    /// <summary>
    /// Wall block connected to tile.
    /// </summary>
    public GameObject East, South, North, West;

    /// <summary>
    /// Corner wall block connected to tile.
    /// </summary>
    public GameObject CornerWN, CornerEN, CornerWS, CornerES;
}
