using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    void Start()
    {
        
    }

    void Update()
    {
        if (health <= 0)
        {
            this.die();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 25f;
            collision.gameObject.SetActive(false);
        }
    }
    void die()
    {
        Destroy(gameObject);
    }
}
