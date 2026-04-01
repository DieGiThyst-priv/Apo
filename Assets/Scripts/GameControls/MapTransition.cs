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

    [SerializeField] private bool requireQuest = false;
    [SerializeField] private QuestInfoSO requiredQuest;
    [SerializeField] private QuestState requiredState = QuestState.CAN_FINISH;

    enum Direction {up, down, left, right, teleport};

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        virtualCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    async void FadeTransition(GameObject player)
    {
        await ScreenFader.Instance.FadeOut();
        UpdatePlayerPosition(player);
        await ScreenFader.Instance.FadeIn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!CanTransition())
        {
            Debug.Log("I dont want to leave yet..");
            return;
        }

        FadeTransition(collision.gameObject);
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

    public void TriggerTransition(GameObject player)
    {
        FadeTransition(player);
    }

    private bool CanTransition()
    {
        if (!requireQuest) return true;

        QuestManager questManager = FindFirstObjectByType<QuestManager>();
        Quest quest = questManager.GetQuestById(requiredQuest.id);

        return quest.state == requiredState || quest.state == QuestState.FINISHED;
    }

}
