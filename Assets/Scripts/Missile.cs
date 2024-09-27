using UnityEngine;

public class Missile : MonoBehaviour
{
    private int damage;
    private float angle;
    private const float speed = 10f;
    private const float turnSpeed = 10f;
    private Transform target;
    private Transform textRotateTarget;
    private Quaternion targetRotation;
    private Quaternion newRotation;
    private readonly Color orange = new (1f, 0.55f, 0.25f);

    public void Setter(int newDamage, Transform newTarget, Transform newRotateTarget)
    {
        target = newTarget;
        damage = newDamage;
        textRotateTarget = newRotateTarget;
    }
    
    private void Update()
    {
        transform.up = Vector3.Slerp(transform.up, target.position-transform.position, turnSpeed * Time.deltaTime);
        transform.position += Time.deltaTime * speed * transform.up;
    }
    
    private void OnCollisionEnter(Collision collision) {
        ContactPoint contactPoint = collision.GetContact(0);
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Interfaces.IDamage>().TakeDamage(damage, contactPoint.point, textRotateTarget, orange);
        }
        Destroy(gameObject);
    }

}
