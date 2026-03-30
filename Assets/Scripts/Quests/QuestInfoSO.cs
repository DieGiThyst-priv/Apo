using UnityEngine;
[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "Scriptable Objects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {  get; private set; }


    [Header("General")]
    public string displayName;

    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    public void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }






}
