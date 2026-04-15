using UnityEngine;
using System;
using CupidDirector.Utils;

[Serializable]
public class EmotionRelation
{
    public ActorId SourceId => _sourceId;
    public ActorId TargetId => _targetId;
    public EmotionType Emotion => _emotion;

    [SerializeField] private ActorId _sourceId;
    [SerializeField] private ActorId _targetId;
    [SerializeField] private EmotionType _emotion;

    public EmotionRelation(ActorId sourceId, ActorId targetId, EmotionType emotion)
    {
        _sourceId = sourceId;
        _targetId = targetId;
        _emotion = emotion;
    }

    public void SetEmotion(EmotionType emotion)
    {
        _emotion = emotion;
    }

    public bool IsMatch(ActorId sourceId, ActorId targetId)
    {
        return _sourceId == sourceId && _targetId == targetId;
    }
}
