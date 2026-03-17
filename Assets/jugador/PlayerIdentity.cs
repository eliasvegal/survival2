using UnityEngine;
using UnityEngine.Networking;

public class PlayerIdentity : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChanged")]
    public string playerName = "";

    public TextMesh textoNombreArriba;

    void Start()
    {
        ActualizarTextoVisual(playerName);
    }

    public override void OnStartLocalPlayer()
    {
        string nombreDelMenu = PlayerPrefs.GetString("PlayerName", "Jugador_" + Random.Range(1, 999));
        CmdSetPlayerName(nombreDelMenu);

        // Ocultar nuestro propio nombre para que no nos estorbe en la pantalla
        if (textoNombreArriba != null)
            textoNombreArriba.gameObject.SetActive(false);
    }

    [Command]
    void CmdSetPlayerName(string nombre)
    {
        nombre = nombre.Trim();

        if (string.IsNullOrEmpty(nombre))
            nombre = "Jugador";

        string nombreBase = nombre;
        string nombreFinal = nombreBase;
        int contador = 0;

        PlayerIdentity[] jugadores = FindObjectsOfType<PlayerIdentity>();

        bool nombreExiste = true;

        while (nombreExiste)
        {
            nombreExiste = false;

            foreach (PlayerIdentity p in jugadores)
            {
                if (p != this && p.playerName == nombreFinal)
                {
                    contador++;
                    nombreFinal = nombreBase + "(" + contador + ")";
                    nombreExiste = true;
                    break;
                }
            }
        }

        playerName = nombreFinal;

        // Mensaje sistema usando el nombre final
        ChatManager chat = FindObjectOfType<ChatManager>();
        if (chat != null)
        {
            string nombreColor = "<color=yellow>" + nombreFinal + "</color>";
            chat.EnviarMensajeSistema(nombreColor + " <color=yellow>se unió a la partida</color>");
        }
    }


    void OnNameChanged(string nuevoNombre)
    {
        playerName = nuevoNombre;
        ActualizarTextoVisual(nuevoNombre);
    }

    void ActualizarTextoVisual(string nombreMostrar)
    {
        if (textoNombreArriba != null)
        {
            textoNombreArriba.text = string.IsNullOrEmpty(nombreMostrar) ? "..." : nombreMostrar;
        }
    }

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = FindObjectOfType<Camera>();

        if (cam != null && textoNombreArriba != null)
        {
            // 1. Calculamos dirección hacia la cámara (incluyendo altura/eje Y)
            Vector3 direccionACamara = cam.transform.position - textoNombreArriba.transform.position;

            // 2. Ya NO bloqueamos la Y. Al dejar la dirección completa, 
            // el texto rotará en X e Y para apuntar a la cámara.

            if (direccionACamara != Vector3.zero)
            {
                // 3. Aplicamos la rotación apuntando directamente al punto de la cámara
                textoNombreArriba.transform.rotation = Quaternion.LookRotation(-direccionACamara);
            }
        }
    }


}
