using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Configuración del End Game")]
    [Tooltip("Tiempo en segundos que esperará el juego antes de reiniciar la escena")]
    public float tiempoEsperaReiniciar = 2f;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void JugadorMuerto()
    {
        Debug.Log("GameManager: El jugador ha muerto. Iniciando cuenta regresiva para reiniciar...");
        
        StartCoroutine(ReiniciarEscenaCoroutine());
    }

    private IEnumerator ReiniciarEscenaCoroutine()
    {
        yield return new WaitForSeconds(tiempoEsperaReiniciar);

        string nombreEscenaActual = SceneManager.GetActiveScene().name;

        Debug.Log("GameManager: Reiniciando escena: " + nombreEscenaActual);

        SceneManager.LoadScene(nombreEscenaActual);
    }
}