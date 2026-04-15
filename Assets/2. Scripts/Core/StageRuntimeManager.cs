using UnityEngine;
using System;
using CupidDirector.Utils;

public class StageRuntimeManager : MonoBehaviour
{
    public StageRuntimeData RuntimeData => _runtimeData;

    [SerializeField] private SceneDefinitionSO _sceneDefinition;
    [SerializeField] private StageRuntimeData _runtimeData;

    private void Awake()
    {
        if(_sceneDefinition == null)
        {
            Debug.LogError("SceneDefinitionSO is not assigned.");
            return;
        }

        _runtimeData = new StageRuntimeData(_sceneDefinition);
    }

    public void SetEmotion(ActorId sourceId, ActorId targetId, EmotionType emotion)
    {
        if (_runtimeData == null)
        {
            Debug.LogError("RuntimeData is not initialized.");
            return;
        }

        _runtimeData.SetEmotion(sourceId, targetId, emotion);
    }

    public EmotionType GetEmotion(ActorId sourceId, ActorId targetId)
    {
        if (_runtimeData == null)
        {
            Debug.LogError("RuntimeData is not initialized.");
            return EmotionType.None;
        }

        return _runtimeData.GetEmotion(sourceId, targetId);
    }

    public bool EvaluateStageResult()
    {
        return StageResultEvaluator.IsSuccess(_runtimeData);
    }
}
