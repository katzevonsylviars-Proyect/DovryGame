using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float vida = 40f;

    public void RecibirDaño(float cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Destroy(gameObject);
    }
}