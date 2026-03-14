using Unity.Cinemachine;
using UnityEngine;
using System.Threading.Tasks;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;

    [SerializeField] CinemachineCamera virtualCamera;

    enum Direction {up, down, left, right, teleport};

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        virtualCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {
        await ScreenFader.Instance.FadeOut();
        UpdatePlayerPosition(player);
        await ScreenFader.Instance.FadeIn();
    }

private void UpdatePlayerPosition(GameObject player)
{
    if(direction == Direction.teleport)
    {
        player.transform.position = teleportTargetPosition.position;

        confiner.BoundingShape2D = mapBoundary;
        confiner.InvalidateBoundingShapeCache();

        virtualCamera.PreviousStateIsValid = false; // forces instant camera snap
    }
}
    
}
