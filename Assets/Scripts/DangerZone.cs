using UnityEngine;

public class DangerZone : MonoBehaviour
{
    public int dañoQueInflige = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Esto imprimirá en la consola CUALQUIER cosa que toque el obstáculo
        Debug.Log("Algo entró en la zona de peligro: " + collision.gameObject.name);

        if (collision.TryGetComponent<PlayerHealth>(out var saludJugador))
        {
            saludJugador.RecibirDaño(dañoQueInflige);
        }
    }
}