using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
    [Header("Objetos que se ACTIVAN al desconectarse")]
    public GameObject[] objectsToEnable;

    [Header("Objetos que se DESACTIVAN al desconectarse")]
    public GameObject[] objectsToDisable;

    [Header("Objetos que se ACTIVAN al conectarse")]
    public GameObject[] objectsToEnableOnConnect;

    [Header("Objetos que se DESACTIVAN al conectarse")]
    public GameObject[] objectsToDisableOnConnect;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // Activar al conectarse
        foreach (GameObject obj in objectsToEnableOnConnect)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Desactivar al conectarse
        foreach (GameObject obj in objectsToDisableOnConnect)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        // Activar al desconectarse
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Desactivar al desconectarse
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }


    // ===== MENSAJES DE SISTEMA =====

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        string nombre = "Jugador";

        if (conn.playerControllers.Count > 0 &&
            conn.playerControllers[0].gameObject != null)
        {
            PlayerIdentity identity =
                conn.playerControllers[0].gameObject.GetComponent<PlayerIdentity>();

            if (identity != null)
                nombre = identity.playerName;
        }

        ChatManager chat = FindObjectOfType<ChatManager>();
        if (chat != null)
        {
            string nombreColor = "<color=yellow>" + nombre + "</color>";
            chat.EnviarMensajeSistema(nombreColor + " <color=yellow>abandonó la partida</color>");
        }

        base.OnServerDisconnect(conn);
    }


}
