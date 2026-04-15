using CupidDirector.Utils;

public static class StageResultEvaluator
{
    public static bool IsSuccess(StageRuntimeData runtimeData)
    {
        if(runtimeData == null || runtimeData.SceneDefinition == null)
        {
            return false;
        }

        var goalConditions = runtimeData.SceneDefinition.GoalConditions;

        for(int i = 0; i < goalConditions.Count; i++)
        {
            SceneGoalCondition condition = goalConditions[i];
            EmotionType currentEmotion = runtimeData.GetEmotion(condition.SourceId, condition.TargetId);

            if (currentEmotion != condition.RequiredEmotion) return false;
        }

        return true;
    }
}
