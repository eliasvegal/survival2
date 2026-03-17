using UnityEngine;
using System.Collections.Generic;

public class FireFlicker : MonoBehaviour
{
    public Light fireLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 4.0f;
    [Range(1, 50)]
    public int smoothing = 5; // Cuanto más bajo, más "nervioso" e inestable

    Queue<float> smoothQueue = new Queue<float>();
    float lastSum = 0;

    void Start()
    {
        if (fireLight == null) fireLight = GetComponent<Light>();
    }

    void Update()
    {
        // Eliminar el valor más antiguo
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generar un nuevo valor aleatorio "crudo"
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calcular el promedio para que no sea un parpadeo digital molesto
        fireLight.intensity = lastSum / smoothQueue.Count;
    }
}
