using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CupidDirector.Utils;

public class StagePerformanceDirector : MonoBehaviour
{
    private const float PERFORMANCE_START_DELAY = 0.75f;
    private const float TURN_DELAY = 0.5f;
    private const float DIALOGUE_DURATION = 1.5f;
    private const float REACTION_DELAY = 0.4f;
    private const float STOP_DISTANCE = 0.8f;

    public IReadOnlyList<StageActorView> ActorViews => _actorViews;
    public StageRuntimeData RuntimeData => _runtimeData;

    [SerializeField] private List<StageActorView> _actorViews = new List<StageActorView>();
    [SerializeField] private StageResultUIView _stageResultUIView;
    [SerializeField] private string _rehearsalSceneName = "RehearsalScene";

    private StageRuntimeData _runtimeData;
    private Coroutine _performanceCoroutine;

    public void Initialize(StageRuntimeData runtimeData)
    {
        if (runtimeData == null)
        {
            Debug.LogError("StageRuntimeData is null.");
            return;
        }

        _runtimeData = runtimeData;
        ResetStageViews();
        HideResultView();
        BindResultUIView();

        StartPerformance();
    }

    public StageActorView FindActorView(ActorId actorId)
    {
        for (int i = 0; i < _actorViews.Count; i++)
        {
            StageActorView actorView = _actorViews[i];

            if (actorView == null)
            {
                continue;
            }

            if (actorView.ActorId == actorId)
            {
                return actorView;
            }
        }

        return null;
    }

    public void ShowStageResult(bool isSuccess, string detailText)
    {
        if (_stageResultUIView == null)
        {
            return;
        }

        _stageResultUIView.ShowResult(isSuccess, detailText);
    }

    private void StartPerformance()
    {
        if (_performanceCoroutine != null)
        {
            StopCoroutine(_performanceCoroutine);
        }

        _performanceCoroutine = StartCoroutine(PlayPerformanceRoutine());
    }

    private IEnumerator PlayPerformanceRoutine()
    {
        yield return new WaitForSeconds(PERFORMANCE_START_DELAY);

        IReadOnlyList<ActorId> actorIds = _runtimeData.SceneDefinition.ActorIds;

        for (int i = 0; i < actorIds.Count; i++)
        {
            yield return PlayActorTurn(actorIds[i]);
            yield return new WaitForSeconds(TURN_DELAY);
        }

        bool isSuccess = StageResultEvaluator.IsSuccess(_runtimeData);
        string resultDetailText = BuildResultDetailText(isSuccess);

        ShowStageResult(isSuccess, resultDetailText);

        _performanceCoroutine = null;
    }

    private IEnumerator PlayActorTurn(ActorId actorId)
    {
        StageActorView actorView = FindActorView(actorId);

        if (actorView == null)
        {
            yield break;
        }

        ActorActionData actionData = BuildActorActionData(actorId);

        if (actionData.TargetActorId != ActorId.None)
        {
            StageActorView targetView = FindActorView(actionData.TargetActorId);

            if (targetView != null)
            {
                Vector3 targetPosition = CalculateStopPosition(actorView, targetView);
                yield return actorView.MoveTo(targetPosition);
            }
        }

        actorView.ShowDialogue(actionData.ActorDialogue, DIALOGUE_DURATION);
        yield return new WaitForSeconds(DIALOGUE_DURATION);

        if (actionData.TargetActorId != ActorId.None && !string.IsNullOrEmpty(actionData.TargetReactionDialogue))
        {
            StageActorView targetView = FindActorView(actionData.TargetActorId);

            if (targetView != null)
            {
                yield return new WaitForSeconds(REACTION_DELAY);
                targetView.ShowDialogue(actionData.TargetReactionDialogue, DIALOGUE_DURATION);
                yield return new WaitForSeconds(DIALOGUE_DURATION);
            }
        }
    }

    private ActorActionData BuildActorActionData(ActorId actorId)
    {
        ActorId loveTargetId = FindTargetWithEmotion(actorId, EmotionType.Love);
        ActorId hateTargetId = FindTargetWithEmotion(actorId, EmotionType.Hate);

        if (loveTargetId != ActorId.None)
        {
            return new ActorActionData(
                EmotionType.Love,
                loveTargetId,
                BuildLoveDialogue(actorId, loveTargetId),
                BuildLoveReactionDialogue(loveTargetId, actorId));
        }

        if (hateTargetId != ActorId.None)
        {
            return new ActorActionData(
                EmotionType.Hate,
                hateTargetId,
                BuildHateDialogue(actorId, hateTargetId),
                BuildHateReactionDialogue(hateTargetId, actorId));
        }

        return new ActorActionData(
            EmotionType.None,
            ActorId.None,
            BuildNeutralDialogue(actorId),
            string.Empty);
    }

    private ActorId FindTargetWithEmotion(ActorId sourceActorId, EmotionType emotionType)
    {
        IReadOnlyList<ActorId> actorIds = _runtimeData.SceneDefinition.ActorIds;

        for (int i = 0; i < actorIds.Count; i++)
        {
            ActorId targetActorId = actorIds[i];

            if (targetActorId == sourceActorId)
            {
                continue;
            }

            EmotionType currentEmotion = _runtimeData.GetEmotion(sourceActorId, targetActorId);

            if (currentEmotion == emotionType)
            {
                return targetActorId;
            }
        }

        return ActorId.None;
    }

    private Vector3 CalculateStopPosition(StageActorView actorView, StageActorView targetView)
    {
        Vector3 actorPosition = actorView.GetPosition();
        Vector3 targetPosition = targetView.GetPosition();

        Vector3 direction = (targetPosition - actorPosition).normalized;

        if (direction == Vector3.zero)
        {
            direction = Vector3.right;
        }

        return targetPosition - direction * STOP_DISTANCE;
    }

    private string BuildLoveDialogue(ActorId sourceActorId, ActorId targetActorId)
    {
        return $"{sourceActorId}: \"{targetActorId}, 너를 사랑해!\"";
    }

    private string BuildLoveReactionDialogue(ActorId targetActorId, ActorId sourceActorId)
    {
        EmotionType reactionEmotion = _runtimeData.GetEmotion(targetActorId, sourceActorId);

        if (reactionEmotion == EmotionType.Love)
        {
            return $"{targetActorId}: \"나도 널 사랑해!\"";
        }

        if (reactionEmotion == EmotionType.Hate)
        {
            return $"{targetActorId}: \"다가오지 마!\"";
        }

        return $"{targetActorId}: \"...무슨 말이지?\"";
    }

    private string BuildHateDialogue(ActorId sourceActorId, ActorId targetActorId)
    {
        return $"{sourceActorId}: \"{targetActorId}, 네가 마음에 들지 않아!\"";
    }

    private string BuildHateReactionDialogue(ActorId targetActorId, ActorId sourceActorId)
    {
        EmotionType reactionEmotion = _runtimeData.GetEmotion(targetActorId, sourceActorId);

        if (reactionEmotion == EmotionType.Love)
        {
            return $"{targetActorId}: \"왜 그런 말을 해...?\"";
        }

        if (reactionEmotion == EmotionType.Hate)
        {
            return $"{targetActorId}: \"나도 널 싫어해!\"";
        }

        return $"{targetActorId}: \"무슨 문제라도 있어?\"";
    }

    private string BuildNeutralDialogue(ActorId actorId)
    {
        return $"{actorId}: \"...\"";
    }

    private string BuildResultDetailText(bool isSuccess)
    {
        if (_runtimeData == null || _runtimeData.SceneDefinition == null)
        {
            return "공연 데이터를 확인할 수 없습니다.";
        }

        return isSuccess
            ? "배우들의 감정이 대본의 핵심 의도와 맞아떨어졌습니다."
            : "배우들의 감정이 대본과 어긋나 다른 드라마가 만들어졌습니다.";
    }

    private void ResetStageViews()
    {
        for (int i = 0; i < _actorViews.Count; i++)
        {
            StageActorView actorView = _actorViews[i];

            if (actorView == null)
            {
                continue;
            }

            actorView.ResetToStartPosition();
            actorView.HideDialogueImmediate();
        }
    }

    private void HideResultView()
    {
        if (_stageResultUIView == null)
        {
            return;
        }

        _stageResultUIView.Hide();
    }

    private void BindResultUIView()
    {
        if (_stageResultUIView == null)
        {
            return;
        }

        _stageResultUIView.RetryButtonClicked -= OnRetryButtonClicked;
        _stageResultUIView.BackToRehearsalButtonClicked -= OnBackToRehearsalButtonClicked;

        _stageResultUIView.RetryButtonClicked += OnRetryButtonClicked;
        _stageResultUIView.BackToRehearsalButtonClicked += OnBackToRehearsalButtonClicked;
    }

    private void OnRetryButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnBackToRehearsalButtonClicked()
    {
        SceneManager.LoadScene(_rehearsalSceneName);
    }

    private readonly struct ActorActionData
    {
        public EmotionType EmotionType { get; }
        public ActorId TargetActorId { get; }
        public string ActorDialogue { get; }
        public string TargetReactionDialogue { get; }

        public ActorActionData(
            EmotionType emotionType,
            ActorId targetActorId,
            string actorDialogue,
            string targetReactionDialogue)
        {
            EmotionType = emotionType;
            TargetActorId = targetActorId;
            ActorDialogue = actorDialogue;
            TargetReactionDialogue = targetReactionDialogue;
        }
    }
}