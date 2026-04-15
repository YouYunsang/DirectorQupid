using UnityEngine;
using System;
using CupidDirector.Utils;

[Serializable]
public class SceneGoalCondition
{
    public ActorId SourceId => _sourceId;
    public ActorId TargetId => _targetId;
    public EmotionType RequiredEmotion => _requiredEmotion;

    [UnityEngine.SerializeField] private ActorId _sourceId;
    [UnityEngine.SerializeField] private ActorId _targetId;
    [UnityEngine.SerializeField] private EmotionType _requiredEmotion;

    public SceneGoalCondition(ActorId sourceId, ActorId targetId, EmotionType requiredEmotion)
    {
        _sourceId = sourceId;
        _targetId = targetId;
        _requiredEmotion = requiredEmotion;
    }
}
