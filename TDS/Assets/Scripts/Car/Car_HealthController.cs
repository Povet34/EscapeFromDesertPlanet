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
    [SerializeField] Transform explosionPoint;

    [Space]
    [SerializeField] float explosionRadius = 3; 
    [SerializeField] float explosionDelay = 3;
    [SerializeField] float explosionForce = 7;
    [SerializeField] float explosionUpwardsModifier = 2;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(fireFx.gameObject.activeSelf)
        {
            fireFx.transform.rotation = Quaternion.identity;
        }
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

        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsModifier, ForceMode.Impulse);

        Explode();
    }

    private void Explode()
    {
        HashSet<GameObject> unieqEntites = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null && !unieqEntites.Contains(collider.gameObject))
            {
                damagable.TakeDamage(explosionDamage);
                unieqEntites.Add(collider.gameObject);

                collider.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsModifier, ForceMode.VelocityChange);
            }
        }
    }
}
