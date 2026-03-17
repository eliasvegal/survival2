# Survival2 (Prototipo Multijugador)

Este es un **prototipo de juego multijugador** hecho en **Unity**.
El proyecto nunca fue terminado, pero la funcionalidad básica del multijugador funciona.

<img width="1919" height="1079" alt="Captura de pantalla 2026-03-17 070138" src="https://github.com/user-attachments/assets/5c73a3ca-1055-4144-834b-b7331a224156" />


El juego utiliza **UNet** para el sistema de red, permitiendo que varios jugadores se conecten a un host y jueguen juntos.

## Características

* Multijugador usando **UNet**
* Sistema de **Host / Cliente**
* Movimiento básico de jugador
* Multijugador en red local (LAN)
* También puede funcionar por internet compartiendo la **IP pública del host**

<img width="1919" height="1079" alt="Captura de pantalla 2026-03-17 070254" src="https://github.com/user-attachments/assets/75af79c3-fe48-46d3-ace8-04a71e07790c" />

## Cómo jugar

1. Descargar "Survival2_1.0" en Releases
2. Abrir Survival.exe
3. Un jugador inicia el juego y crea el **host**.
4. El host comparte su **IP pública** con los demás jugadores.
5. Los otros jugadores ingresan esa IP para conectarse al servidor.

⚠️ Puede ser necesario **abrir o redirigir el puerto del router (port forwarding)** para permitir conexiones externas.

## Requisitos para jugar

- SO: Windows 7 / Linux / macOS
- Procesador: Dual Core 2.0 GHz
- Memoria: 4 GB RAM
- Gráficos: Integrada (Intel HD o similar)
- Red: Conexión a internet o LAN
- Almacenamiento: ~300 MB disponibles

- ## Posibles problemas

- Si no puedes conectarte:
  - Verifica que el puerto 25565 esté abierto en el router
  - Asegúrate de que el firewall no bloquee la conexión

## Estado del proyecto

Este proyecto **no está terminado** y se comparte principalmente como un **prototipo o experimento**.

## Notas

* El sistema multijugador está basado en **UNet**, el cual actualmente está **obsoleto en Unity**.
* Dependiendo de la versión de Unity, el proyecto puede requerir algunos ajustes para funcionar correctamente.

## Autor

Proyecto creado por Elias como práctica y aprendizaje.
