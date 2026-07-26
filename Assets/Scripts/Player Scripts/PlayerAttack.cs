using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInputs controls;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerFuel fuel;

    [Header("Ataque Básico")]
    public Transform puntoAtaque;
    public float rangoAtaque = 1f;
    public int daño = 20;
    public LayerMask capaEnemigos;
    public float cooldownAtaque = 0.5f;
    private float tiempoProximoAtaque;

    [Header("Rebote Aéreo")]
    public float fuerzaRebote = 8f;

    [Header("Ground Pound")]
    public float fuerzaGroundPound = 15f;
    public float radioImpactoTerreno = 1.5f;
    private bool haciendoGroundPound = false;

    [Header("Dash Especial")]
    public float velocidadEspecial = 16f;
    public float duracionEspecial = 0.6f;
    public float fuerzaReboteEspecial = 10f;
    public float cooldownEspecial = 1.2f;
    public float costoCombustibleDash = 25f;

    private bool haciendoEspecial = false;
    private float tiempoEspecial;
    private float proximoEspecial;

    void Awake()
    {
        controls = new PlayerInputs();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Attack.performed += ctx => Atacar();
        controls.Player.Special.performed += ctx => EjecutarEspecial();
    }

    void OnDisable()
    {
        controls.Disable();
        controls.Player.Attack.performed -= ctx => Atacar();
        controls.Player.Special.performed -= ctx => EjecutarEspecial();
    }

    void Start()
    {
        fuel = GetComponent<PlayerFuel>();
    }

    void Update()
    {
        if (haciendoGroundPound)
        {
            if (playerMovement != null && playerMovement.EnElSuelo)
            {
                haciendoGroundPound = false;
                HacerDañoTerreno();
            }
        }

        if (haciendoEspecial)
        {
            tiempoEspecial -= Time.deltaTime;
            if (tiempoEspecial <= 0)
            {
                haciendoEspecial = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (haciendoGroundPound)
        {
            Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(transform.position, 0.8f, capaEnemigos);
            if (enemigosGolpeados.Length > 0)
            {
                haciendoGroundPound = false;
                
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaRebote, ForceMode2D.Impulse);

                foreach (var enemigo in enemigosGolpeados)
                {
                    Debug.Log("Golpeaste al enemigo con el Ground Pound: " + enemigo.name);
                    EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
                    if (vidaEnemigo != null)
                    {
                        vidaEnemigo.RecibirDaño(daño);
                    }

                    DestructibleBlock bloque = enemigo.GetComponent<DestructibleBlock>();
                    if (bloque != null) bloque.RecibirDaño(daño);
                }
            }
        }

        if (haciendoEspecial)
        {
            float direccion = Mathf.Sign(transform.localScale.x);
            rb.linearVelocity = new Vector2(direccion * velocidadEspecial, 0);

            Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(transform.position, 0.8f, capaEnemigos);
            if (enemigosGolpeados.Length > 0)
            {
                haciendoEspecial = false;

                rb.linearVelocity = new Vector2(-direccion * fuerzaReboteEspecial, rb.linearVelocity.y);

                foreach (var enemigo in enemigosGolpeados)
                {
                    Debug.Log("Golpeaste al enemigo con el Especial (Dash): " + enemigo.name);
                    EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
                    if (vidaEnemigo != null)
                    {
                        vidaEnemigo.RecibirDaño(daño);
                    }
                    DestructibleBlock bloque = enemigo.GetComponent<DestructibleBlock>();
                    if (bloque != null) bloque.RecibirDaño(daño);
                }
            }
        }
    }

    public void EjecutarEspecial()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (playerMovement != null && !playerMovement.EnElSuelo && moveInput.y < -0.5f)
        {
            IniciarGroundPound();
        }
        else 
        {
            if (Time.time >= proximoEspecial)
            {
                if (fuel != null && fuel.TieneCombustible(costoCombustibleDash))
                {
                    fuel.Consumir(costoCombustibleDash);

                    haciendoEspecial = true;
                    tiempoEspecial = duracionEspecial;
                    proximoEspecial = Time.time + cooldownEspecial;

                    if (playerMovement != null)
                    {
                        playerMovement.estaHaciendoHover = false;
                    }
                }
                else
                {
                    Debug.Log("No tienes suficiente combustible para hacer un Dash.");
                }
            }
        }
    }

    void IniciarGroundPound()
    {
        haciendoGroundPound = true;
        rb.linearVelocity = new Vector2(0, -fuerzaGroundPound);
    }

    void HacerDañoTerreno()
    {
        Debug.Log("Impacto contra el suelo: Daño a ambos lados.");

        Vector3 posIzquierda = transform.position + Vector3.left * radioImpactoTerreno;
        Vector3 posDerecha = transform.position + Vector3.right * radioImpactoTerreno;

        Collider2D[] enemigosIzquierda = Physics2D.OverlapCircleAll(posIzquierda, radioImpactoTerreno, capaEnemigos);
        Collider2D[] enemigosDerecha = Physics2D.OverlapCircleAll(posDerecha, radioImpactoTerreno, capaEnemigos);

        foreach (var enemigo in enemigosIzquierda)
        {
            Debug.Log("Daño al enemigo por impacto (izquierda): " + enemigo.name);
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(daño);
            }
        }

        foreach (var enemigo in enemigosDerecha)
        {
            Debug.Log("Daño al enemigo por impacto (derecha): " + enemigo.name);
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(daño);
            }
        }
    }

    public void Atacar()
    {
        if (haciendoEspecial) return;

        if (Time.time < tiempoProximoAtaque) return;

        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        bool presionandoAbajo = moveInput.y < -0.5f;
        bool enElSuelo = playerMovement != null && playerMovement.EnElSuelo;

        if (!enElSuelo && presionandoAbajo)
        {
            AtacarAereoAbajo();
        }
        else
        {
            AtacarNormal();
        }
    }

    void AtacarNormal()
    {
        tiempoProximoAtaque = Time.time + cooldownAtaque;

        Vector3 posicionAtaque;
        if (puntoAtaque != null)
        {
            posicionAtaque = puntoAtaque.position;
        }
        else
        {
            float direccion = Mathf.Sign(transform.localScale.x);
            posicionAtaque = transform.position + new Vector3(direccion * 0.8f, 0, 0);
        }

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(posicionAtaque, rangoAtaque, capaEnemigos);

        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Golpeaste a: " + enemigo.name);
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(daño);
            }

            DestructibleBlock bloque = enemigo.GetComponent<DestructibleBlock>();
            if (bloque != null) bloque.RecibirDaño(daño);
        }
    }

    void AtacarAereoAbajo()
    {
        tiempoProximoAtaque = Time.time + cooldownAtaque;

        Vector3 posicionAtaque = transform.position + Vector3.down * 0.6f;
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(posicionAtaque, rangoAtaque, capaEnemigos);

        bool enemigoGolpeado = false;
        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Golpeaste al enemigo hacia abajo: " + enemigo.name);
            enemigoGolpeado = true;
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(daño);
            }

            DestructibleBlock bloque = enemigo.GetComponent<DestructibleBlock>();
            if (bloque != null) bloque.RecibirDaño(daño);
        }

        if (enemigoGolpeado)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaRebote, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
        }
        else
        {
            float direccion = Mathf.Sign(transform.localScale.x);
            Vector3 posAtaqueNormal = transform.position + new Vector3(direccion * 0.8f, 0, 0);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(posAtaqueNormal, rangoAtaque);

            Vector3 posAtaqueAbajo = transform.position + Vector3.down * 0.6f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(posAtaqueAbajo, rangoAtaque);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * radioImpactoTerreno, radioImpactoTerreno);
        Gizmos.DrawWireSphere(transform.position + Vector3.right * radioImpactoTerreno, radioImpactoTerreno);
    }
}