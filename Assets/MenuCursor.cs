using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    void Awake()
    {
        MostrarCursor();
    }

    void OnEnable()
    {
        MostrarCursor();
    }

    void Start()
    {
        MostrarCursor();
    }

    void MostrarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
