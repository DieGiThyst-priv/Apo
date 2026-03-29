using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Shootable") || !collision.gameObject.CompareTag("Default"))
        {
            gameObject.SetActive(false); return;
        }
    }

    public GameObject getShooter()
    {
        return shooter;
    }   
}
