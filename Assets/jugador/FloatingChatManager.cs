using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloatingChatManager : MonoBehaviour
{
    public GameObject mensajePrefab;   // Prefab MensajeFlotante
    public Transform contenedor;       // ChatFloatingContainer

    private List<GameObject> mensajesActivos = new List<GameObject>();
    private const int MAX_MENSAJES = 10;

    public void AgregarMensaje(string texto)
    {
        // 1. Crear el nuevo mensaje primero para medirlo
        GameObject nuevo = Instantiate(mensajePrefab, contenedor);
        nuevo.transform.SetAsLastSibling();

        Text txt = nuevo.GetComponentInChildren<Text>();
        if (txt != null) txt.text = texto;

        mensajesActivos.Add(nuevo);

        // 2. FORZAR RECONSTRUCCIÓN: Necesario para saber la altura real al instante
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contenedor.GetComponent<RectTransform>());

        // 3. LIMPIEZA POR ESPACIO Y LÍMITE:
        // Mientras la altura del contenido sea mayor que el contenedor O haya más de 10
        RectTransform rectContenedor = contenedor.GetComponent<RectTransform>();

        // Usamos un bucle while por si hay que borrar varios mensajes de golpe
        while (mensajesActivos.Count > 1 &&
              (CalculosDeAltura() > rectContenedor.rect.height || mensajesActivos.Count > MAX_MENSAJES))
        {
            GameObject viejo = mensajesActivos[0];
            mensajesActivos.RemoveAt(0);
            DestroyImmediate(viejo); // DestroyImmediate para que la altura se actualice ya mismo

            // Volvemos a recalcular después de borrar
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectContenedor);
        }

        // 4. Temporizador normal
        StartCoroutine(DestruirDespues(nuevo, 10f));
    }

    // Función auxiliar para saber cuánto miden todos los mensajes juntos
    float CalculosDeAltura()
    {
        float alturaTotal = 0;
        foreach (GameObject m in mensajesActivos)
        {
            if (m != null)
                alturaTotal += m.GetComponent<RectTransform>().rect.height;
        }
        // Sumamos el spacing del Layout Group
        VerticalLayoutGroup vlg = contenedor.GetComponent<VerticalLayoutGroup>();
        if (vlg != null && mensajesActivos.Count > 1)
            alturaTotal += vlg.spacing * (mensajesActivos.Count - 1);

        return alturaTotal;
    }





    IEnumerator DestruirDespues(GameObject obj, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        // Verificamos que el objeto aún exista antes de intentar quitarlo
        if (obj != null)
        {
            if (mensajesActivos.Contains(obj))
                mensajesActivos.Remove(obj);

            Destroy(obj);

            // Esto obliga a la UI a reacomodar los mensajes restantes suavemente
            LayoutRebuilder.ForceRebuildLayoutImmediate(contenedor.GetComponent<RectTransform>());
        }
    }

}