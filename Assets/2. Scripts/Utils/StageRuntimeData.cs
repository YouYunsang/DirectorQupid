using UnityEngine;
using System.Collections.Generic;
using System;
using CupidDirector.Utils;

[Serializable]
public class StageRuntimeData
{
    public SceneDefinitionSO SceneDefinition => _sceneDefinition;
    public IReadOnlyList<EmotionRelation> Relations => _relations;

    [SerializeField] private SceneDefinitionSO _sceneDefinition;
    [SerializeField] private List<EmotionRelation> _relations = new List<EmotionRelation>();

    public StageRuntimeData(SceneDefinitionSO sceneDefinition)
    {
        _sceneDefinition = sceneDefinition; ;
        InitializeDefaultRelations();
    }

    public void Initialize(SceneDefinitionSO sceneDefinition)
    {
        _sceneDefinition = sceneDefinition;
        _relations.Clear();
        InitializeDefaultRelations();
    }

    public EmotionType GetEmotion(ActorId sourceId, ActorId targetId)
    {
        for (int i = 0; i < _relations.Count; i++)
        {
            if (_relations[i].IsMatch(sourceId, targetId))
            {
                return _relations[i].Emotion;
            }
        }

        return EmotionType.None;
    }

    public void SetEmotion(ActorId sourceId, ActorId targetId, EmotionType emotion)
    {
        if (sourceId == ActorId.None || targetId == ActorId.None)
        {
            Debug.LogWarning("Invalid actor id.");
            return;
        }

        if (sourceId == targetId)
        {
            Debug.LogWarning("Source and target cannot be the same actor.");
            return;
        }

        for (int i = 0; i < _relations.Count; i++)
        {
            if (_relations[i].IsMatch(sourceId, targetId))
            {
                _relations[i].SetEmotion(emotion);
                return;
            }
        }

        _relations.Add(new EmotionRelation(sourceId, targetId, emotion));
    }

    private void InitializeDefaultRelations()
    {
        if (_sceneDefinition == null)
        {
            Debug.LogWarning("SceneDefinition is null.");
            return;
        }

        IReadOnlyList<ActorId> actorIds = _sceneDefinition.ActorIds;

        for (int i = 0; i < actorIds.Count; i++)
        {
            for (int j = 0; j < actorIds.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                _relations.Add(new EmotionRelation(actorIds[i], actorIds[j], EmotionType.None));
            }
        }
    }
}
