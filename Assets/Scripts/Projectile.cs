using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float damage;
    private Vector3 direction;
    private Transform target;

    private void Update()
    {
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
            transform.LookAt(target);
        }
        transform.position += direction * speed * Time.deltaTime;
    }
    
    public void Fire(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }
}
