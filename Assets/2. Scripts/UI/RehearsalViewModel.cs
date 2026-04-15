using CupidDirector.Utils;
using UnityEngine;

public class RehearsalViewModel
{
    private const string NONE_LABEL = "-";
    private const string EMPTY_TEXT = "";
    private const string UNKNOWN_SCENE_TITLE = "Untitled Scene";
    private const string UNKNOWN_SCRIPT_TEXT = "No Script";
    private const string WAITING_SOURCE_TEXT = "배우를 선택하세요.";
    private const string WAITING_TARGET_TEXT_FORMAT = "출발 배우: {0} / 대상 배우를 선택하세요.";
    private const string READY_TO_APPLY_TEXT_FORMAT = "출발 배우: {0} / 대상 배우: {1}";
    private const string RELATION_TEXT_FORMAT = "현재 설정: {0} → {1} = {2}";

    public RehearsalDisplayData BuildDisplayData(
        SceneDefinitionSO sceneDefinition,
        RehearsalSelectionState selectionState,
        ActorId sourceActorId,
        ActorId targetActorId,
        EmotionType currentEmotion,
        bool canStartStage)
    {
        // 장면 정의가 없으면 기본 문구를 사용한다.
        string sceneTitle = sceneDefinition != null ? sceneDefinition.SceneTitle : UNKNOWN_SCENE_TITLE;
        string scriptText = sceneDefinition != null ? sceneDefinition.ScriptText : UNKNOWN_SCRIPT_TEXT;

        // 현재 선택 단계에 맞는 안내 문구를 생성한다.
        string selectionText = BuildSelectionText(selectionState, sourceActorId, targetActorId);

        // source와 target이 모두 정해진 경우에만 관계 텍스트를 보여준다.
        string relationText = BuildRelationText(sourceActorId, targetActorId, currentEmotion);

        // 감정 적용 버튼은 source와 target이 모두 정해졌을 때만 활성화된다.
        bool canApplyEmotion = selectionState == RehearsalSelectionState.TargetSelected;

        return new RehearsalDisplayData(
            sceneTitle,
            scriptText,
            selectionText,
            relationText,
            canApplyEmotion,
            canStartStage);
    }

    private string BuildSelectionText(
        RehearsalSelectionState selectionState,
        ActorId sourceActorId,
        ActorId targetActorId)
    {
        switch (selectionState)
        {
            case RehearsalSelectionState.None:
                return WAITING_SOURCE_TEXT;

            case RehearsalSelectionState.SourceSelected:
                return string.Format(
                    WAITING_TARGET_TEXT_FORMAT,
                    GetActorLabel(sourceActorId));

            case RehearsalSelectionState.TargetSelected:
                return string.Format(
                    READY_TO_APPLY_TEXT_FORMAT,
                    GetActorLabel(sourceActorId),
                    GetActorLabel(targetActorId));

            default:
                return WAITING_SOURCE_TEXT;
        }
    }

    private string BuildRelationText(
        ActorId sourceActorId,
        ActorId targetActorId,
        EmotionType currentEmotion)
    {
        if (sourceActorId == ActorId.None || targetActorId == ActorId.None)
        {
            return EMPTY_TEXT;
        }

        return string.Format(
            RELATION_TEXT_FORMAT,
            GetActorLabel(sourceActorId),
            GetActorLabel(targetActorId),
            GetEmotionLabel(currentEmotion));
    }

    private string GetActorLabel(ActorId actorId)
    {
        return actorId == ActorId.None ? NONE_LABEL : actorId.ToString();
    }

    private string GetEmotionLabel(EmotionType emotionType)
    {
        switch (emotionType)
        {
            case EmotionType.Love:
                return "Love";

            case EmotionType.Hate:
                return "Hate";

            case EmotionType.None:
            default:
                return "None";
        }
    }
}