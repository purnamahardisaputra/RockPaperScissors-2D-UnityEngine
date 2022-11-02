using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] Rigidbody2D rb;
    ShooterPlayer owner;
    public void Set(ShooterPlayer owner, Vector3 position, Vector3 direction, float lag)
    {
        this.owner = owner;
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), owner.GetComponent<Collider2D>());

        rb.position = position;
        rb.velocity = direction * speed;
        rb.position = rb.position + rb.velocity * lag;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<ShooterPlayer>() != owner)
            {
                owner.RestoreHealth();
            }
        }
        Destroy(this.gameObject);
    }
}
