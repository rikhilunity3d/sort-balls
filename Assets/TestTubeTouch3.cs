using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestTubeTouch3 : MonoBehaviour
{
    public static event Action ButtonPressed = delegate { };

    private void OnMouseDown()
    {
        ButtonPressed();
    }

}
