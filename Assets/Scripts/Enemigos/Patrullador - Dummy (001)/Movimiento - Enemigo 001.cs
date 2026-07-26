using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;
    public float distanciaDeteccion = 0.5f;
    public LayerMask capaObstaculos;

    [Header("Combate")]
    public int dañoAlJugador = 20;

    private Rigidbody2D rb;
    private bool moviendoDerecha = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 direccion = moviendoDerecha ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, distanciaDeteccion, capaObstaculos);

        if (hit.collider != null)
        {
            Girar();
        }
    }

    void FixedUpdate()
    {
        float dir = moviendoDerecha ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * velocidad, rb.linearVelocity.y);
    }

    void Girar()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RecibirDaño(dañoAlJugador);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 dir = moviendoDerecha ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * distanciaDeteccion);
    }
}