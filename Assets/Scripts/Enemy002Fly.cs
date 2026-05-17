using UnityEngine;

public class EnemigoVolador : MonoBehaviour
{
    public float velocidad = 3f;
    public int daño = 10;

    private Transform jugador;
    private Rigidbody2D rb;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
    }

    void Update()
    {
        if (jugador == null) return;

        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.linearVelocity = direccion * velocidad;

        Girar(direccion);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemigo volador hace daño");

            // Llamar al script del jugador
        }
    }

    void Girar(Vector2 direccion)
    {
        if (direccion.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direccion.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}