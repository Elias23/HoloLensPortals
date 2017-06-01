using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public Rigidbody projectile;
    public float speed = 20;
    private ArrayList bullets;

    private void Start()
    {
        bullets = new ArrayList();
    }
    public void OnThrow()
    {
            Rigidbody instantiatedProjectile = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
            bullets.Add(instantiatedProjectile);
            instantiatedProjectile.gameObject.SetActive(true);
            instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
    }
    public void OnReset()
    {
        foreach (Rigidbody rb in bullets) { Destroy(rb.gameObject); }
        bullets.Clear();
    }
}