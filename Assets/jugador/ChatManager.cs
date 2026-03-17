using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class ChatManager : NetworkBehaviour
{
    private List<string> mensajes = new List<string>();
    private const int LIMITE_MENSAJES = 50;

    [SerializeField] private Text textoChat; // ← asignar en inspector

    [ClientRpc]
    public void RpcRecibirMensaje(string mensaje)
    {
        if (textoChat == null) return;

        // -------- CHAT ORIGINAL (NO MODIFICADO) --------
        mensajes.Add(mensaje);

        if (mensajes.Count > LIMITE_MENSAJES)
            mensajes.RemoveAt(0);

        textoChat.text = string.Join("\n", mensajes.ToArray());

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            textoChat.GetComponent<RectTransform>()
        );

        // -------- NUEVO: CHAT FLOTANTE --------
        FloatingChatManager floating = FindObjectOfType<FloatingChatManager>();
        if (floating != null)
        {
            floating.AgregarMensaje(mensaje);
        }
    }

    [Server]
    public void EnviarMensajeSistema(string mensaje)
    {
        string mensajeFormateado = "" + mensaje;
        RpcRecibirMensaje(mensajeFormateado);
    }
}