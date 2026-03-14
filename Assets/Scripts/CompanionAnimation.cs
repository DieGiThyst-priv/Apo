using UnityEngine;
using Pathfinding;

public class CompanionAnimation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Animator animator;
    private float distance;
    private float stopBuffer = 0.01f;

    [SerializeField] private float stopDistance = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

     void Update()
    {
        if (player == null) return;
        distance = Vector2.Distance(this.transform.position, player.transform.position);
        Vector2 direction = (player.transform.position - this.transform.position).normalized;
        UpdateAnimationDirection(direction);
        
        if (distance > stopDistance + stopBuffer)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        
    }


    void UpdateAnimationDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;

        float x = 0;
        float y = 0;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            x = Mathf.Sign(dir.x);
        }
        else
        {
            y = Mathf.Sign(dir.y);
        }

        animator.SetFloat("DirectionX", x);
        animator.SetFloat("DirectionY", y);
    }
}
