using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactuable : MonoBehaviour
{
    [HideInInspector] // Lo ocultamos porque se llena solo
    public string nombreObjeto;

    void Start()
    {
        // Toma el nombre que tiene el objeto en la jerarquía
        nombreObjeto = gameObject.name;
    }

    // Método para cuando quieras que el objeto haga algo
    public void Interactuar()
    {
        Debug.Log("Interactuando con: " + nombreObjeto);
    }
}
