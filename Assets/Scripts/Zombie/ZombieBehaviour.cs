using UnityEngine;
using System.Collections;

public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] Collider2D visionCollider;
    [SerializeField] private float moveSpeed = 1f;
    private bool isWandering = false;

    private GameObject currentTarget;
    void Start()
    {
        if (!isWandering)
        {
            StartCoroutine(Wander());
        }
    }

    void Update()
    {
        if (currentTarget != null)
        {
            isWandering = false;
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered: " + collision.name + " | Tag: " + collision.tag);
        if (collision.CompareTag("Player") || collision.CompareTag("Companion"))
        {
            currentTarget = collision.gameObject;
        }
    }
    private IEnumerator Wander() {
        float counter = 10f;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        while (currentTarget == null)
        {
            if (counter > 1f)
            {
                counter = 0;
                randomDirection = Random.insideUnitCircle.normalized;
            }
            transform.position += (Vector3)randomDirection * moveSpeed * Time.deltaTime;
            isWandering = true;
            counter += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
