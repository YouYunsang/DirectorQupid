using CupidDirector.Utils;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneDefinition", menuName = "ScriptableObjects/Scene Definition")]
public class SceneDefinitionSO : ScriptableObject
{
    public string SceneTitle => _sceneTitle;
    public string ScriptText => _scriptText;
    public IReadOnlyList<ActorId> ActorIds => _actorIds;
    public IReadOnlyList<SceneGoalCondition> GoalConditions => _goalConditions;

    [SerializeField] private string _sceneTitle;
    [TextArea(5, 10)]
    [SerializeField] private string _scriptText;
    [SerializeField] private List<ActorId> _actorIds = new List<ActorId>();
    [SerializeField] private List<SceneGoalCondition> _goalConditions = new List<SceneGoalCondition>();

    public bool ContainsActor(ActorId actorId)
    {
        return _actorIds.Contains(actorId);
    }
}
