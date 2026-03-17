# Survival2 (Prototipo Multijugador)

Este es un **prototipo de juego multijugador** hecho en **Unity**.
El proyecto nunca fue terminado, pero la funcionalidad básica del multijugador funciona.

El juego utiliza **UNet** para el sistema de red, permitiendo que varios jugadores se conecten a un host y jueguen juntos.

## Características

* Multijugador usando **UNet**
* Sistema de **Host / Cliente**
* Movimiento básico de jugador
* Multijugador en red local (LAN)
* También puede funcionar por internet compartiendo la **IP pública del host**

## Cómo jugar

1. Un jugador inicia el juego y crea el **host**.
2. El host comparte su **IP pública** con los demás jugadores.
3. Los otros jugadores ingresan esa IP para conectarse al servidor.

⚠️ Puede ser necesario **abrir o redirigir el puerto del router (port forwarding)** para permitir conexiones externas.

## Estado del proyecto

Este proyecto **no está terminado** y se comparte principalmente como un **prototipo o experimento**.

## Notas

* El sistema multijugador está basado en **UNet**, el cual actualmente está **obsoleto en Unity**.
* Dependiendo de la versión de Unity, el proyecto puede requerir algunos ajustes para funcionar correctamente.

## Autor

Proyecto creado como práctica y aprendizaje.
