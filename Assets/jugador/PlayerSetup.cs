using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject modeloVisual; // arrastras el mesh aquí

    void Start()
    {
        if (isLocalPlayer)
        {
            modeloVisual.SetActive(false);
        }
    }
}