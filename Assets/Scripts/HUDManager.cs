using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public PlayerHealth playerHealth;
    public PlayerFuel playerFuel; // NUEVO: Referencia al script de combustible

    [Header("Elementos de la Interfaz")]
    public Image barraVidaUI;
    public Image barraCombustibleUI; // NUEVO: Referencia a la imagen de la barra de combustible

    void Update()
    {
        // Actualizar la barra de vida
        if (playerHealth != null && barraVidaUI != null)
        {
            barraVidaUI.fillAmount = playerHealth.ObtenerVidaPorcentaje();
        }

        // NUEVO: Actualizar la barra de combustible
        if (playerFuel != null && barraCombustibleUI != null)
        {
            // Calculamos el porcentaje dividiendo el combustible actual por el máximo
            float porcentajeCombustible = playerFuel.ObtenerCombustible() / playerFuel.combustibleMax;
            
            // Asignamos el porcentaje al Relleno (Fill Amount) de la UI
            barraCombustibleUI.fillAmount = porcentajeCombustible;
        }
    }
}