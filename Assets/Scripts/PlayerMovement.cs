using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs controls;
    private Vector2 moveInput;

    public float velocidad = 5f;
    public float FuerzaDeSalto = 5f;
    public float LongitudRaycast = 0.1f;
    public LayerMask CapaDeSuelo;

    public float combustible = 1000;

    public int saltosMaximos = 1;

    public float velocidadDash = 15f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;

    public bool EnElSuelo;

    private int saltosRestantes;
    private bool estaDasheando;
    private float tiempoDash;
    private float proximoDash;

    private Rigidbody2D rb;

    void Awake()
    {
        controls = new PlayerInputs();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Dash.performed += ctx => Dash();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        saltosRestantes = saltosMaximos;
    }

    void Update()
    {
        // Comprobación de suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, LongitudRaycast, CapaDeSuelo);
        EnElSuelo = hit.collider != null;

        if (EnElSuelo)
        {
            saltosRestantes = saltosMaximos;
        }

        // DASH
        if (estaDasheando)
        {
            float direccion = transform.localScale.x;
            rb.linearVelocity = new Vector2(direccion * velocidadDash, 0);
            tiempoDash -= Time.deltaTime;

            if (tiempoDash <= 0)
            {
                estaDasheando = false;
            }
            return;
        }

        // Movimiento horizontal con nuevo input system
        float VelocidadX = moveInput.x;

        if (VelocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (VelocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        rb.linearVelocity = new Vector2(VelocidadX * velocidad, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (saltosRestantes > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * FuerzaDeSalto, ForceMode2D.Impulse);
            saltosRestantes--;
        }
    }

    void Dash()
    {
        if (Time.time >= proximoDash)
        {
            estaDasheando = true;
            tiempoDash = duracionDash;
            proximoDash = Time.time + cooldownDash;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * LongitudRaycast);
    }
}