using UnityEngine;

public class MissileTargeting : MonoBehaviour
{
    private int damage;
    private const float speed = 10f;
    private const float turnSpeed = 7.5f;
    private Transform target;
    private Transform textRotateTarget;
    private Color color;
    
    
    public void Setter(int newDamage, Transform newTarget, Transform newRotateTarget, Color newColor)
    {
        target = newTarget;
        damage = newDamage;
        textRotateTarget = newRotateTarget;
        color = newColor;
    }
    
    private void Update()
    {
        transform.up = Vector3.Slerp(transform.up, target.position-transform.position, turnSpeed * Time.deltaTime);
        transform.position += Time.deltaTime * speed * transform.up;
    }
    
    private void OnCollisionEnter(Collision collision) {
        ContactPoint contP = collision.GetContact(0);
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<InterfacesMNG.IDmg>().TakeDmg(damage, contP.point, textRotateTarget, color);
        }
        Destroy(gameObject);
    }

}
