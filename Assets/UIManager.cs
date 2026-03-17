using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Paneles que se OCULTAN")]
    public GameObject[] panelsToHide;

    [Header("Paneles que se MUESTRAN")]
    public GameObject[] panelsToShow;

    public void SwitchPanels()
    {
        // Ocultar
        foreach (GameObject panel in panelsToHide)
        {
            panel.SetActive(false);
        }

        // Mostrar
        foreach (GameObject panel in panelsToShow)
        {
            panel.SetActive(true);
        }
    }
}
