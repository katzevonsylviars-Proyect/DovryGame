using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 5f;

    public float FuerzaDeSalto = 5f;
    public float LongitudRaycast = 0.1f;
    public LayerMask CapaDeSuelo;

    public bool EnElSuelo;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float VelocidadX = Input.GetAxis("Horizontal");

        if (VelocidadX < 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        Vector3 posicion = transform.position;

        //transform.position = new Vector3(VelocidadX + posicion.x, posicion.y, posicion.z);
    
        Vector2 NuevaVelocidad = new Vector2(VelocidadX * velocidad, rb.linearVelocity.y);
        rb.linearVelocity = NuevaVelocidad;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, LongitudRaycast, CapaDeSuelo);
        EnElSuelo = hit.collider != null;
            


        if(EnElSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f,FuerzaDeSalto), ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * LongitudRaycast);
    }
}