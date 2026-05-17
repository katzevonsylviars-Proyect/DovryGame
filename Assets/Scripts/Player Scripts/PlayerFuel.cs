using UnityEngine;

public class PlayerFuel : MonoBehaviour
{
    public float combustibleMax = 100f;
    public float regeneracionPorSegundo = 10f;

    private float combustibleActual;

    void Start()
    {
        combustibleActual = combustibleMax;
    }

    void Update()
    {
        Regenerar();
    }

    void Regenerar()
    {
        combustibleActual += regeneracionPorSegundo * Time.deltaTime;
        combustibleActual = Mathf.Clamp(combustibleActual, 0, combustibleMax);
    }

    public bool TieneCombustible(float cantidad)
    {
        return combustibleActual >= cantidad;
    }

    public void Consumir(float cantidad)
    {
        combustibleActual -= cantidad;
        combustibleActual = Mathf.Clamp(combustibleActual, 0, combustibleMax);
    }


    public void ConsumirContinuo(float cantidadPorSegundo)
    {
        combustibleActual -= cantidadPorSegundo * Time.deltaTime;
        combustibleActual = Mathf.Clamp(combustibleActual, 0, combustibleMax);
    }

    public float ObtenerCombustible()
    {
        return combustibleActual;
    }
}