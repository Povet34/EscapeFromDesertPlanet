using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamagable
{
    private Car_Controller carController;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;

    [Header("Explosion info")]
    [SerializeField] int explosionDamage = 350;
    [SerializeField] ParticleSystem fireFx;
    [SerializeField] ParticleSystem explosionFx;

    [Space]
    [SerializeField] float explosionDelay = 3;
    [SerializeField] float explosionForce = 7;
    [SerializeField] float explosionUpwardsModifier = 2;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth,maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            BrakeTheCar();
    }

    private void BrakeTheCar()
    {
        carBroken = true;
        carController.BrakeTheCar();

        fireFx.gameObject.SetActive(true);
        StartCoroutine(ExplosionCo(explosionDelay));
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionFx.gameObject.SetActive(true);

        float explosionRadius = 5f;
        carController.rb.AddExplosionForce(explosionDamage, transform.position - Vector3.down + (Vector3.forward * 1.5f), explosionRadius, explosionUpwardsModifier, ForceMode.Impulse);

        Explode();
    }

    private void Explode()
    {
        HashSet<GameObject> unieqEntites = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null && !unieqEntites.Contains(collider.gameObject))
            {
                damagable.TakeDamage(explosionDamage);
                unieqEntites.Add(collider.gameObject);

                var explosionPoint = transform.position + Vector3.forward * 1.5f;

                collider.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(explosionForce, explosionPoint, 5f, explosionUpwardsModifier, ForceMode.VelocityChange);
            }
        }
    }
}
