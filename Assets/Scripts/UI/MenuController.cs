using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    [SerializeField] public GameObject playerMovement;
    private PlayerMovement playerMovementScript;

    void Start()
    {
      menuCanvas.SetActive(false);
        playerMovementScript = playerMovement.GetComponent<PlayerMovement>();   
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerMovementScript.setFrozen(!playerMovementScript.isFrozen());
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }   
    }
}
