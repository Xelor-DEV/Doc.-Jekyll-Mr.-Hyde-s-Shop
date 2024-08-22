using UnityEngine;
[CreateAssetMenu(fileName = "PatrolPoints", menuName = "ScriptableObjects/Data/PatrolPoints", order = 1)]
public class PatrolPoints : ScriptableObject
{
    [SerializeField] private Transform[] patrolPoints;
}
