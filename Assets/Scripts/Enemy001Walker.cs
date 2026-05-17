using UnityEngine;

public class EnemigoBasico : MonoBehaviour
{
    public float velocidad = 2f;
    public int daño = 10;

    private Rigidbody2D rb;
    private int direccion = -1; // -1 = izquierda, 1 = derecha

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            direccion *= -1;
            Girar();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Daña al jugador");

            // Llamar al script del jugador
        }
    }

    void Girar()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}