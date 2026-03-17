using UnityEngine;
using System.Collections;

public class PoliceLights : MonoBehaviour
{
    public Light lightRed;
    public Light lightBlue;
    public float flashSpeed = 0.15f; // Velocidad del parpadeo

    void Start()
    {
        StartCoroutine(PoliceRoutine());
    }

    IEnumerator PoliceRoutine()
    {
        while (true)
        {
            // Parpadeo Rojo (doble destello)
            lightBlue.enabled = false;
            lightRed.enabled = true; yield return new WaitForSeconds(flashSpeed);
            lightRed.enabled = false; yield return new WaitForSeconds(flashSpeed);
            lightRed.enabled = true; yield return new WaitForSeconds(flashSpeed);
            lightRed.enabled = false;

            yield return new WaitForSeconds(flashSpeed);

            // Parpadeo Azul (doble destello)
            lightRed.enabled = false;
            lightBlue.enabled = true; yield return new WaitForSeconds(flashSpeed);
            lightBlue.enabled = false; yield return new WaitForSeconds(flashSpeed);
            lightBlue.enabled = true; yield return new WaitForSeconds(flashSpeed);
            lightBlue.enabled = false;

            yield return new WaitForSeconds(flashSpeed);
        }
    }
}
