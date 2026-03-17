using UnityEngine;
using UnityEngine.Networking;

public class ContadorFPS : MonoBehaviour
{
    float deltaTime = 0.0f;

    float timer = 0f;
    float intervaloActualizacion = 0.5f; // 1 segundo

    float fpsMostrado = 0f;
    string pingMostrado = "Offline";

    GUIStyle style;
    Rect rect;

    void Start()
    {
        int h = Screen.height;

        rect = new Rect(10, 10, Screen.width, h * 2 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = Color.white;
    }

    void Update()
    {
        // Suavizado FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        timer += Time.unscaledDeltaTime;

        if (timer >= intervaloActualizacion)
        {
            timer = 0f;

            // Actualizamos valores mostrados
            fpsMostrado = 1.0f / deltaTime;
            pingMostrado = ObtenerPing();
        }
    }

    string ObtenerPing()
    {
        if (NetworkManager.singleton != null)
        {
            if (NetworkClient.active && NetworkManager.singleton.client != null)
            {
                return NetworkManager.singleton.client.GetRTT() + " ms";
            }
            else if (NetworkServer.active)
            {
                return "0 ms (Host)";
            }
        }

        return "Offline";
    }

    void OnGUI()
    {
        string text = string.Format("{0:0} FPS | {1}", fpsMostrado, pingMostrado);
        GUI.Label(rect, text, style);
    }
}