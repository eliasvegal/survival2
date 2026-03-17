using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public InputField ipInput;
    public InputField portInput;
    public InputField inputNombre;

    private NetworkManager manager;

    void Start()
    {
        manager = NetworkManager.singleton;

        // Carga el último nombre usado para que el jugador no tenga que escribirlo siempre
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            inputNombre.text = PlayerPrefs.GetString("PlayerName");
        }

        ipInput.text = "localhost";
        portInput.text = "25565";
    }

    // Esta función guarda el nombre en la PC antes de entrar al juego
    public void GuardarNombreEnPrefs()
    {
        string nombreFinal = inputNombre.text;

        // Si el campo está vacío, le asignamos uno por defecto
        if (string.IsNullOrEmpty(nombreFinal))
        {
            nombreFinal = "Jugador_" + Random.Range(100, 999);
        }

        PlayerPrefs.SetString("PlayerName", nombreFinal);
        PlayerPrefs.Save(); // Asegura que se guarde en el disco
        Debug.Log("Nombre guardado: " + nombreFinal);
    }

    // MODO OFFLINE
    public void PlayOffline()
    {
        GuardarNombreEnPrefs();
        manager.StopHost();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    // CREAR SERVIDOR
    public void CreateServer()
    {
        GuardarNombreEnPrefs(); // Guardamos el nombre antes de iniciar

        int port;
        if (int.TryParse(portInput.text, out port))
        {
            manager.networkPort = port;
        }

        manager.networkAddress = "0.0.0.0";
        manager.StartHost();
    }

    // UNIRSE A SERVIDOR
    public void JoinServer()
    {
        GuardarNombreEnPrefs(); // Guardamos el nombre antes de iniciar

        int port;
        if (int.TryParse(portInput.text, out port))
        {
            manager.networkPort = port;
        }

        manager.networkAddress = ipInput.text;
        manager.StartClient();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
