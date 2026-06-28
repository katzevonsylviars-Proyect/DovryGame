using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs controls;
    private Vector2 moveInput;

    public float velocidad = 8f;
    public float FuerzaDeSalto = 10f;
    public float LongitudRaycast = 0.6f;
    public LayerMask CapaDeSuelo;

    [Header("Coste de Habilidades")]
    public float costoCombustibleDash = 25f;

    [Header("Combustible y Hover")]
    public PlayerFuel fuel;
    public float consumoHover = 20f;
    public bool estaHaciendoHover = false;
    private float tiempoUltimoSalto = -1f;
    public int saltosMaximos = 1;
    public bool EnElSuelo;

    [Header("Escalada y Salto en Pared")]
    public LayerMask CapaDePared;
    public float velocidadDeslizamiento = 2f;
    public Vector2 fuerzaSaltoPared = new Vector2(7f, 7f);
    private bool deslizandoEnPared;
    private bool tocandoParedIzquierda;
    private bool tocandoParedDerecha;

    private int saltosRestantes;
    private Rigidbody2D rb;

    void Awake()
    {
        controls = new PlayerInputs();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
    
        controls.Player.Attack.performed += ctx => AtacarInput();
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

        if (fuel == null)
        {
            fuel = GetComponent<PlayerFuel>();
        }
    }

    void Update()
    {
        RaycastHit2D hitSuelo = Physics2D.Raycast(transform.position, Vector2.down, LongitudRaycast, CapaDeSuelo);
        EnElSuelo = hitSuelo.collider != null;

        if (EnElSuelo)
        {
            saltosRestantes = saltosMaximos;
            deslizandoEnPared = false;
            estaHaciendoHover = false;
        }

        tocandoParedIzquierda = Physics2D.Raycast(transform.position, Vector2.left, LongitudRaycast, CapaDePared);
        tocandoParedDerecha = Physics2D.Raycast(transform.position, Vector2.right, LongitudRaycast, CapaDePared);

        bool tocandoPared = tocandoParedIzquierda || tocandoParedDerecha;

        if (tocandoPared && !EnElSuelo && moveInput.x != 0 && rb.linearVelocity.y < 0)
        {
            deslizandoEnPared = true;
            estaHaciendoHover = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocidadDeslizamiento);
        }
        else
        {
            deslizandoEnPared = false;
        }

        if (estaHaciendoHover)
        {
            if (fuel != null && fuel.TieneCombustible(consumoHover * Time.deltaTime))
            {
                fuel.ConsumirContinuo(consumoHover);
            }
            else
            {
                estaHaciendoHover = false;
            }
        }

        // MOVIMIENTO
        float VelocidadX = moveInput.x;

        if (VelocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (VelocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (deslizandoEnPared)
        {
        }
        else if (estaHaciendoHover)
        {
            rb.linearVelocity = new Vector2(VelocidadX * velocidad, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(VelocidadX * velocidad, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        if (deslizandoEnPared)
        {
            estaHaciendoHover = false;
            SaltoPared();
        }
        else if (EnElSuelo)
        {
            estaHaciendoHover = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * FuerzaDeSalto, ForceMode2D.Impulse);
            saltosRestantes = saltosMaximos;
            saltosRestantes--;
            tiempoUltimoSalto = Time.time;
        }
        else if (estaHaciendoHover)
        {
            estaHaciendoHover = false;
        }
        else if (saltosRestantes > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * FuerzaDeSalto, ForceMode2D.Impulse);
            saltosRestantes--;
            tiempoUltimoSalto = Time.time;
        }
        else if (Time.time - tiempoUltimoSalto > 0.3f)
        {
            estaHaciendoHover = true;
            tiempoUltimoSalto = Time.time;
        }
    }

    void SaltoPared()
    {
        estaHaciendoHover = false;

        float direccionSalto = tocandoParedIzquierda ? 1f : -1f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(fuerzaSaltoPared.x * direccionSalto, fuerzaSaltoPared.y), ForceMode2D.Impulse);
        
        saltosRestantes = saltosMaximos;
        tiempoUltimoSalto = Time.time;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * LongitudRaycast);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * LongitudRaycast);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * LongitudRaycast);
    }

    void AtacarInput()
    {
        PlayerAttack attackScript = GetComponent<PlayerAttack>();
        if (attackScript != null)
        {
            attackScript.Atacar();
        }
    }
}