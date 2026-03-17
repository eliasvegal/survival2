using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class player : NetworkBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 6f;
    public float acceleration = 12f;
    public float deceleration = 10f;

    [Header("Salto")]
    public float jumpForce = 9f;
    public float gravityUp = -25f;
    public float gravityDown = -35f;
    public float groundStickForce = -5f;

    [Header("Cámara")]
    public float mouseSensitivity = 250f;
    public float cameraSmoothTime = 0.02f;
    public Transform playerCamera;
    [SyncVar] public string playerName = "Jugador";


    [Header("Interacción")]
    public float rayDistance = 5f;
    public KeyCode interactKey = KeyCode.E; // <--- Ahora puedes cambiarla en el Inspector


    [Header("UI Interacción (LOCAL)")]
    public List<RawImage> mostrarAlDetectar = new List<RawImage>();
    public List<Text> textosAlDetectar = new List<Text>();

    [Space]
    public List<RawImage> ocultarAlDetectar = new List<RawImage>();
    public List<Text> textosAOcultar = new List<Text>();


    [Header("Menu")]
    private GameObject canvasMenu;

    private bool menuActivo = false;

    private int resolucionSeleccionadaIndex = 0;

    private Toggle toggleFPS;
    private Text textoFPS;
    private ContadorFPS contadorFPS;

    private Toggle toggleVSync;
    private Text textoVSync;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 currentMoveVelocity;
    private Vector3 moveVelocitySmoothRef;
    private Outline objetoConOutline; // <--- Referencia para saber qué objeto brilla

    // resolucion
    private Dropdown dropdownResolucion;
    private Resolution[] resolucionesDisponibles;

    // sensibilidad
    private Slider sliderSensibilidad;


    private Dropdown dropdownCalidad;
    private int calidadSeleccionadaIndex;
    private float targetYaw;
    private float targetPitch;
    private float currentYaw;
    private float currentPitch;
    private float smoothYawVelocity;
    private float smoothPitchVelocity;

    // -------- CHAT --------
    private InputField chatInputUI;
    private Button botonUI;
    private GameObject canvasChat;
    private bool chatActivo = false;


    public override void OnStartLocalPlayer()
    {
        string nombreGuardado = PlayerPrefs.GetString("PlayerName", "Jugador");
        CmdSetPlayerName(nombreGuardado);
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

        player[] jugadores = FindObjectsOfType<player>();

        bool nombreExiste = true;

        while (nombreExiste)
        {
            nombreExiste = false;

            foreach (player p in jugadores)
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
    }


    void Start()
    {
        if (!isLocalPlayer)
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
            

            Canvas canvasHijo = GetComponentInChildren<Canvas>();
            if (canvasHijo != null)
                canvasHijo.gameObject.SetActive(false);
            return;
        }

        controller = GetComponent<CharacterController>();

        contadorFPS = FindObjectOfType<ContadorFPS>();

        mouseSensitivity = PlayerPrefs.GetFloat("SensibilidadMouse", mouseSensitivity);

        // 🔎 Buscar Canvas aunque esté desactivado
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "CanvasChat")
            {
                canvasChat = obj;
                
            }

            if (obj.name == "CanvasMenu")
            {
                canvasMenu = obj;
                
            }
        }

        Button botonCerrar = null;

        Button[] botones = canvasMenu.GetComponentsInChildren<Button>(true);

        foreach (Button b in botones)
        {
            if (b.gameObject.name == "CerrarMenu")
            {
                botonCerrar = b;
                break;
            }
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(CerrarMenu);
        }



        if (canvasChat != null)
        {
            // Activarlo temporalmente para poder buscar hijos
            canvasChat.SetActive(true);

            chatInputUI = canvasChat.GetComponentInChildren<InputField>(true);
            botonUI = canvasChat.GetComponentInChildren<Button>(true);

            if (botonUI != null)
            {
                botonUI.onClick.RemoveAllListeners();
                botonUI.onClick.AddListener(EnviarMensajeChat);
            }

            // Volver a ocultarlo
            canvasChat.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 e = transform.eulerAngles;
        currentYaw = targetYaw = e.y;
        currentPitch = targetPitch = playerCamera.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;


        // ------------------ CONFIGURAR SLIDER SENSIBILIDAD ------------------

        if (canvasMenu != null)
        {
            Slider[] sliders = canvasMenu.GetComponentsInChildren<Slider>(true);

            foreach (Slider s in sliders)
            {
                if (s.gameObject.name == "SliderSensibilidad")
                {
                    sliderSensibilidad = s;
                    break;
                }
            }

            if (sliderSensibilidad != null)
            {
                // Valor inicial
                sliderSensibilidad.value = mouseSensitivity;

                sliderSensibilidad.onValueChanged.RemoveAllListeners();
                sliderSensibilidad.onValueChanged.AddListener(CambiarSensibilidad);
            }
        }


        // ------------------ CONFIGURAR DROPDOWN RESOLUCION ------------------

        if (canvasMenu != null)
        {
            Dropdown[] dropdowns = canvasMenu.GetComponentsInChildren<Dropdown>(true);

            foreach (Dropdown d in dropdowns)
            {
                if (d.gameObject.name == "DropdownResolucion")
                {
                    dropdownResolucion = d;
                    break;
                }
            }

            if (dropdownResolucion != null)
            {
                resolucionesDisponibles = Screen.resolutions;
                dropdownResolucion.ClearOptions();

                List<string> opciones = new List<string>();
                List<Resolution> resolucionesFiltradas = new List<Resolution>();
                HashSet<string> resolucionesUnicas = new HashSet<string>();

                int indiceActual = 0;

                for (int i = 0; i < resolucionesDisponibles.Length; i++)
                {
                    string clave = resolucionesDisponibles[i].width + "x" + resolucionesDisponibles[i].height;

                    if (!resolucionesUnicas.Contains(clave))
                    {
                        resolucionesUnicas.Add(clave);
                        resolucionesFiltradas.Add(resolucionesDisponibles[i]);

                        string opcion = resolucionesDisponibles[i].width + " x " + resolucionesDisponibles[i].height;
                        opciones.Add(opcion);

                        if (resolucionesDisponibles[i].width == Screen.currentResolution.width &&
                            resolucionesDisponibles[i].height == Screen.currentResolution.height)
                        {
                            indiceActual = resolucionesFiltradas.Count - 1;
                        }
                    }
                }

                // Reemplazamos el array original por el filtrado
                resolucionesDisponibles = resolucionesFiltradas.ToArray();

                dropdownResolucion.AddOptions(opciones);
                dropdownResolucion.value = indiceActual;
                dropdownResolucion.RefreshShownValue();

                dropdownResolucion.onValueChanged.RemoveAllListeners();
                dropdownResolucion.onValueChanged.AddListener(SeleccionarResolucion);
            }
        }

        int indiceGuardado = PlayerPrefs.GetInt("ResolucionIndex", -1);

        if (indiceGuardado >= 0 && indiceGuardado < resolucionesDisponibles.Length)
        {
            dropdownResolucion.value = indiceGuardado;
            CambiarResolucion(indiceGuardado);
        }



        // ------------------ CONFIGURAR BOTÓN APLICAR ------------------

        Button[] botonesMenu = canvasMenu.GetComponentsInChildren<Button>(true);

        foreach (Button b in botonesMenu)
        {
            if (b.gameObject.name == "BotonAplicar")
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(AplicarResolucion);
                break;
            }
        }


        // ------------------ CONFIGURAR TOGGLE VSYNC ------------------

        Toggle[] toggles = canvasMenu.GetComponentsInChildren<Toggle>(true);

        foreach (Toggle t in toggles)
        {
            if (t.gameObject.name == "ToggleVSync")
            {
                toggleVSync = t;
                break;
            }
        }

        if (toggleVSync != null)
        {
            textoVSync = toggleVSync.GetComponentInChildren<Text>(true);

            int vsyncGuardado = PlayerPrefs.GetInt("VSync", 0);

            QualitySettings.vSyncCount = vsyncGuardado;
            toggleVSync.isOn = vsyncGuardado == 1;

            ActualizarTextoVSync(toggleVSync.isOn);

            toggleVSync.onValueChanged.RemoveAllListeners();
            toggleVSync.onValueChanged.AddListener(CambiarVSync);
        }


        // ------------------ CONFIGURAR TOGGLE FPS ------------------

        Toggle[] togglesFPS = canvasMenu.GetComponentsInChildren<Toggle>(true);

        foreach (Toggle t in togglesFPS)
        {
            if (t.gameObject.name == "ToggleFPS")
            {
                toggleFPS = t;
                break;
            }
        }

        if (toggleFPS != null)
        {
            textoFPS = toggleFPS.GetComponentInChildren<Text>(true);

            int fpsGuardado = PlayerPrefs.GetInt("MostrarFPS", 0);

            bool activo = fpsGuardado == 1;

            if (contadorFPS != null)
                contadorFPS.enabled = activo;

            toggleFPS.isOn = activo;

            ActualizarTextoFPS(activo);

            toggleFPS.onValueChanged.RemoveAllListeners();
            toggleFPS.onValueChanged.AddListener(CambiarFPS);
        }


        // ------------------ CONFIGURAR DROPDOWN CALIDAD ------------------

        Dropdown[] dropdownsCalidad = canvasMenu.GetComponentsInChildren<Dropdown>(true);

        foreach (Dropdown d in dropdownsCalidad)
        {
            if (d.gameObject.name == "DropdownCalidad")
            {
                dropdownCalidad = d;
                break;
            }
        }

        if (dropdownCalidad != null)
        {
            dropdownCalidad.ClearOptions();

            List<string> opciones = new List<string>(QualitySettings.names);

            dropdownCalidad.AddOptions(opciones);

            int calidadGuardada = PlayerPrefs.GetInt("CalidadGrafica", QualitySettings.GetQualityLevel());

            dropdownCalidad.value = calidadGuardada;
            dropdownCalidad.RefreshShownValue();

            calidadSeleccionadaIndex = calidadGuardada;

            dropdownCalidad.onValueChanged.RemoveAllListeners();
            dropdownCalidad.onValueChanged.AddListener(SeleccionarCalidad);
        }

    }


    void SeleccionarCalidad(int indice)
    {
        calidadSeleccionadaIndex = indice;
    }

    void CambiarFPS(bool activado)
    {
        if (contadorFPS != null)
            contadorFPS.enabled = activado;

        PlayerPrefs.SetInt("MostrarFPS", activado ? 1 : 0);

        ActualizarTextoFPS(activado);
    }

    void ActualizarTextoFPS(bool activado)
    {
        if (textoFPS == null) return;

        if (activado)
        {
            textoFPS.text = " Activado";
            textoFPS.color = Color.green;
        }
        else
        {
            textoFPS.text = " Desactivado";
            textoFPS.color = Color.red;
        }
    }


    void CambiarVSync(bool activado)
    {
        QualitySettings.vSyncCount = activado ? 1 : 0;
        PlayerPrefs.SetInt("VSync", activado ? 1 : 0);

        ActualizarTextoVSync(activado);
    }

    void ActualizarTextoVSync(bool activado)
    {
        if (textoVSync == null) return;

        if (activado)
        {
            textoVSync.text = " Activado";
            textoVSync.color = Color.green;
        }
        else
        {
            textoVSync.text = " Desactivado";
            textoVSync.color = Color.red;
        }
    }


    void SeleccionarResolucion(int indice)
    {
        resolucionSeleccionadaIndex = indice;
    }


    public void AplicarResolucion()
    {
        // --- RESOLUCION ---
        if (resolucionesDisponibles != null && resolucionesDisponibles.Length > 0)
        {
            Resolution res = resolucionesDisponibles[resolucionSeleccionadaIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolucionIndex", resolucionSeleccionadaIndex);
        }

        // --- CALIDAD GRAFICA ---
        QualitySettings.SetQualityLevel(calidadSeleccionadaIndex);
        PlayerPrefs.SetInt("CalidadGrafica", calidadSeleccionadaIndex);
    }


    void CambiarResolucion(int indice)
    {
        Resolution res = resolucionesDisponibles[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolucionIndex", indice);
    }


    void CambiarSensibilidad(float nuevaSensibilidad)
    {
        mouseSensitivity = nuevaSensibilidad;
        PlayerPrefs.SetFloat("SensibilidadMouse", nuevaSensibilidad);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.T) && !chatActivo && !menuActivo)
            ActivarChat();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (chatActivo)
            {
                DesactivarChat();
            }
            else if (menuActivo)
            {
                CerrarMenu();
            }
            else
            {
                AbrirMenu();
            }
        }

        CheckGround();
        HandleJumpAndGravity();

        if (!chatActivo && !menuActivo)
        {
            ReadMoveInput();
            ReadMouseInput();
            ProcesarRaycast();
        }
        else
        {
            ReadMoveInputBloqueado();
        }
    }

    void ProcesarRaycast()
    {
        Vector3 rayOrigin = playerCamera.position;
        Vector3 rayDirection = playerCamera.forward;

        Color colorRayo = Color.red;
        bool hayInteractuable = false;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance))
        {
            Interactuable interact = hit.collider.GetComponent<Interactuable>();

            if (interact != null)
            {
                colorRayo = Color.green;
                hayInteractuable = true;

                // --- LÓGICA DEL OUTLINE ---
                Outline outlineEncontrado = hit.collider.GetComponent<Outline>();

                // Si el objeto tiene un Outline...
                if (outlineEncontrado != null)
                {
                    // Si es un objeto DISTINTO al que ya brillaba
                    if (outlineEncontrado != objetoConOutline)
                    {
                        if (objetoConOutline != null) objetoConOutline.enabled = false; // Apaga el anterior
                        objetoConOutline = outlineEncontrado;
                        objetoConOutline.enabled = true; // Enciende el nuevo
                    }
                }
                else
                {
                    // Si miramos algo interactuable PERO no tiene outline, limpiamos el anterior
                    LimpiarOutline();
                }

                // --- INTERACCIÓN ---
                if (Input.GetKeyDown(interactKey))
                {
                    interact.Interactuar();
                }
            }
            else
            {
                // Tocamos algo (pared, suelo) que no es interactuable
                LimpiarOutline();
            }
        }
        else
        {
            // No tocamos absolutamente nada (el vacío)
            LimpiarOutline();
        }

        // Actualizar UI
        SetUIVisible(hayInteractuable);

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, colorRayo);
    }

    // Función para resetear el estado
    void LimpiarOutline()
    {
        if (objetoConOutline != null)
        {
            objetoConOutline.enabled = false;
            objetoConOutline = null;
        }
    }


    void SetUIVisible(bool detectoAlgo)
    {
        // ¡IMPORTANTE! Solo el jugador local puede tocar su propia UI
        if (!isLocalPlayer) return;

        foreach (RawImage img in mostrarAlDetectar) if (img != null) img.gameObject.SetActive(detectoAlgo);
        foreach (Text txt in textosAlDetectar) if (txt != null) txt.gameObject.SetActive(detectoAlgo);

        foreach (RawImage img in ocultarAlDetectar) if (img != null) img.gameObject.SetActive(!detectoAlgo);
        foreach (Text txt in textosAOcultar) if (txt != null) txt.gameObject.SetActive(!detectoAlgo);
    }



    void LateUpdate()
    {
        if (!isLocalPlayer) return;

        ApplyMovement();

        if (!chatActivo && !menuActivo)
            ApplyCamera();
    }

    void ActivarChat()
    {
        chatActivo = true;
        if (canvasChat != null) canvasChat.SetActive(true);

        // --- NUEVO: Ocultar el chat flotante ---
        FloatingChatManager floating = FindObjectOfType<FloatingChatManager>();
        if (floating != null)
        {
            CanvasGroup cg = floating.contenedor.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0; // Se vuelve invisible
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (chatInputUI != null) chatInputUI.ActivateInputField();
    }

    void DesactivarChat()
    {
        chatActivo = false;
        if (canvasChat != null) canvasChat.SetActive(false);

        // --- NUEVO: Mostrar el chat flotante ---
        FloatingChatManager floating = FindObjectOfType<FloatingChatManager>();
        if (floating != null)
        {
            CanvasGroup cg = floating.contenedor.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1; // Vuelve a ser visible
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void AbrirMenu()
    {
        menuActivo = true;

        if (canvasMenu != null)
            canvasMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarMenu()
    {
        menuActivo = false;

        if (canvasMenu != null)
            canvasMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void EnviarMensajeChat()
    {
        if (chatInputUI != null && !string.IsNullOrEmpty(chatInputUI.text))
        {
            // Usamos directamente 'playerName' que ya existe en este script (línea 26)
            // Si por alguna razón está vacío, usamos el netId
            string nombreParaChat = !string.IsNullOrEmpty(playerName) ? playerName : "Jugador " + netId;

            string mensajeFinal = "<" + nombreParaChat + "> " + chatInputUI.text;

            CmdDifundirAlServidor(mensajeFinal);

            chatInputUI.text = "";
            chatInputUI.ActivateInputField();
        }
    }



    [Command]
    void CmdDifundirAlServidor(string mensaje)
    {
        ChatManager chat = FindObjectOfType<ChatManager>();
        if (chat != null)
            chat.RpcRecibirMensaje(mensaje);
    }

    void CheckGround()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = groundStickForce;
    }

    void ReadMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = transform.right * x + transform.forward * z;
        Vector3 targetDir = inputDir.sqrMagnitude > 0.001f ? inputDir.normalized : Vector3.zero;

        Vector3 desiredVelocity = targetDir * walkSpeed;

        float smoothTime = targetDir.sqrMagnitude > 0.001f
            ? 1f / acceleration
            : 1f / deceleration;

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            desiredVelocity,
            ref moveVelocitySmoothRef,
            smoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );
    }

    void ReadMoveInputBloqueado()
    {
        Vector3 desiredVelocity = Vector3.zero;

        float smoothTime = 1f / deceleration;

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            desiredVelocity,
            ref moveVelocitySmoothRef,
            smoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );
    }

    void HandleJumpAndGravity()
    {
        // --- SALTO TIPO MINECRAFT ---
        if (!chatActivo && !menuActivo)
        {
            if (isGrounded && Input.GetButton("Jump"))
            {
                velocity.y = jumpForce;
            }
        }

        // --- GRAVEDAD ---
        if (velocity.y > 0f)
            velocity.y += gravityUp * Time.deltaTime;
        else
            velocity.y += gravityDown * Time.deltaTime;
    }

    void ApplyMovement()
    {
        Vector3 finalMove = currentMoveVelocity + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    void ReadMouseInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        float mouseFactor = mouseSensitivity * 0.01f;

        targetYaw += mouseX * mouseFactor;
        targetPitch -= mouseY * mouseFactor;
        targetPitch = Mathf.Clamp(targetPitch, -90f, 90f);
    }

    void ApplyCamera()
    {
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref smoothYawVelocity, cameraSmoothTime);
        currentPitch = Mathf.SmoothDampAngle(currentPitch, targetPitch, ref smoothPitchVelocity, cameraSmoothTime);

        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        playerCamera.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}