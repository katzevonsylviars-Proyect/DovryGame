using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int vidaMaxima = 100;
    public float tiempoInvulnerable = 1f;

    private int vidaActual;
    private bool esInvulnerable;
    private float tiempoInvulnerableRestante;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    void Update()
    {
        // Manejo del tiempo de invulnerabilidad
        if (esInvulnerable)
        {
            tiempoInvulnerableRestante -= Time.deltaTime;

            if (tiempoInvulnerableRestante <= 0)
            {
                esInvulnerable = false;
            }
        }
    }

    public void RecibirDaño(int daño)
    {
        if (esInvulnerable) return;

        vidaActual -= daño;
        Debug.Log("Jugador recibe daño. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        // Activar invulnerabilidad temporal
        esInvulnerable = true;
        tiempoInvulnerableRestante = tiempoInvulnerable;
    }

    void Morir()
    {
        Debug.Log("Jugador murió");

        // Opciones 
    }
    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
    }
}