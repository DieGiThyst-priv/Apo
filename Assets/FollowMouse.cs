using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        float direction = Mathf.Sign(mousePos.x - transform.position.x);

        animator.SetFloat("mouseX", direction);
    }
}