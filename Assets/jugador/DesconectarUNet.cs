using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // Necesario para UNet
using UnityEngine.SceneManagement;

public class DesconectarUNet : MonoBehaviour
{
    public void SalirAlMenu()
    {
        // 1. Obtenemos la referencia al NetworkManager actual
        NetworkManager manager = NetworkManager.singleton;

        if (manager != null)
        {
            // 2. Detenemos según el rol del jugador
            if (NetworkServer.active && NetworkClient.active)
            {
                // Si eres el Host (Servidor + Cliente)
                manager.StopHost();
            }
            else if (NetworkClient.active)
            {
                // Si eres solo un Cliente
                manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                // Si eres solo Servidor (Dedicated)
                manager.StopServer();
            }
        }

        // 3. Cargamos la escena del menú manualmente si no está configurada en el Manager
        SceneManager.LoadScene("menu");
    }
}
