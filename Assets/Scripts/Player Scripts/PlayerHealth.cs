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
        // tiempo de invulnerabilidad
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

        // Desactivamos los componentes de control
        if (TryGetComponent<PlayerMovement>(out var movement)) movement.enabled = false;
        if (TryGetComponent<PlayerAttack>(out var attack)) attack.enabled = false;

        // Avisamos al GameManager 
        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.JugadorMuerto();
        }
        else
        {
            Debug.LogWarning("No se encontró una instancia de GameManager en la escena. Reiniciando de forma directa.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
    }
    public float ObtenerVidaPorcentaje()
    {
        return (float)vidaActual / vidaMaxima;
    }
}