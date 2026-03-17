using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterEnviar : MonoBehaviour
{
    public Button botonAPresionar;

    void Update()
    {
        // Si se presiona Enter o el Enter del teclado numérico
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (botonAPresionar != null)
            {
                botonAPresionar.onClick.Invoke();
            }
        }
    }
}
