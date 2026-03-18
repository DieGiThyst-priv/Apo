using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public Transform player;
    public Transform[] companions;
    public float distanceBehind = 1.5f;
    public float spacing = 1.5f;

    void Update()
    {
        for (int i = 0; i < companions.Length; i++)
        {
        
            float sideOffset = (i == 0) ? -spacing : spacing; 
            
            Vector3 slotPosition = player.position 
                                 - (player.up * distanceBehind)
                                 + (player.right * sideOffset);

            var ai = companions[i].GetComponent<Pathfinding.IAstarAI>();
            if (ai != null) ai.destination = slotPosition;
        }
    }
}