using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Configuración del Bloque")]
    public float vida = 20f;

    public void RecibirDaño(float cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            Romper();
        }
    }

    void Romper()
    {
        Destroy(gameObject);
    }
}