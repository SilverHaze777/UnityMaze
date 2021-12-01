using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetText : MonoBehaviour
{
    public void SetText(float value)
    {
        GetComponent<Text>().text = value.ToString();
    } 
}
