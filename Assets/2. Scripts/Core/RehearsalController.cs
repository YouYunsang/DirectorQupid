using CupidDirector.Utils;
using System.Collections.Generic;
using UnityEngine;

public class RehearsalController : MonoBehaviour
{
    private const bool CAN_START_STAGE_BY_DEFAULT = true;

    [SerializeField] private StageRuntimeManager _stageRuntimeManager;
    [SerializeField] private RehearsalUIView _rehearsalUIView;
    [SerializeField] private List<ActorPresenter> _actorPresenters = new List<ActorPresenter>();
    [SerializeField] private List<ActorSelectionView> _actorSelectionViews = new List<ActorSelectionView>();

    private RehearsalViewModel _viewModel;
    private RehearsalSelectionState _selectionState = RehearsalSelectionState.None;
    private ActorId _selectedSourceActorId = ActorId.None;
    private ActorId _selectedTargetActorId = ActorId.None;

    private void Awake()
    {
        _viewModel = new RehearsalViewModel();
    }

    private void OnEnable()
    {
        BindActorPresenters();
        BindUIView();
        RefreshView();
        RefreshSelectionViews();
    }

    private void OnDisable()
    {
        UnbindActorPresenters();
        UnbindUIView();
    }

    private void BindActorPresenters()
    {
        for (int i = 0; i < _actorPresenters.Count; i++)
        {
            if (_actorPresenters[i] == null)
            {
                continue;
            }

            _actorPresenters[i].Clicked += OnActorClicked;
        }
    }

    private void UnbindActorPresenters()
    {
        for (int i = 0; i < _actorPresenters.Count; i++)
        {
            if (_actorPresenters[i] == null)
            {
                continue;
            }

            _actorPresenters[i].Clicked -= OnActorClicked;
        }
    }

    private void BindUIView()
    {
        if (_rehearsalUIView == null)
        {
            return;
        }

        _rehearsalUIView.EmotionButtonClicked += OnEmotionButtonClicked;
        _rehearsalUIView.ResetButtonClicked += OnResetButtonClicked;
        _rehearsalUIView.StartStageButtonClicked += OnStartStageButtonClicked;
    }

    private void UnbindUIView()
    {
        if (_rehearsalUIView == null)
        {
            return;
        }

        _rehearsalUIView.EmotionButtonClicked -= OnEmotionButtonClicked;
        _rehearsalUIView.ResetButtonClicked -= OnResetButtonClicked;
        _rehearsalUIView.StartStageButtonClicked -= OnStartStageButtonClicked;
    }

    private void OnActorClicked(ActorId clickedActorId)
    {
        if (clickedActorId == ActorId.None)
        {
            return;
        }

        switch (_selectionState)
        {
            case RehearsalSelectionState.None:
                SelectSource(clickedActorId);
                break;

            case RehearsalSelectionState.SourceSelected:
                HandleActorClickWhenSourceSelected(clickedActorId);
                break;

            case RehearsalSelectionState.TargetSelected:
                HandleActorClickWhenTargetSelected(clickedActorId);
                break;
        }

        RefreshView();
        RefreshSelectionViews();
    }

    private void HandleActorClickWhenSourceSelected(ActorId clickedActorId)
    {
        // 같은 배우를 다시 클릭하면 source 유지.
        if (_selectedSourceActorId == clickedActorId)
        {
            return;
        }

        SelectTarget(clickedActorId);
    }

    private void HandleActorClickWhenTargetSelected(ActorId clickedActorId)
    {
        // target까지 고른 상태에서 source를 바꾸고 싶으면 새 source로 재시작.
        if (_selectedSourceActorId == clickedActorId)
        {
            ClearTargetSelection();
            _selectionState = RehearsalSelectionState.SourceSelected;
            return;
        }

        // 현재 target을 다시 클릭하면 그대로 유지.
        if (_selectedTargetActorId == clickedActorId)
        {
            return;
        }

        SelectSource(clickedActorId);
    }

    private void OnEmotionButtonClicked(EmotionType emotionType)
    {
        if (_selectionState != RehearsalSelectionState.TargetSelected)
        {
            return;
        }

        if (_stageRuntimeManager == null)
        {
            Debug.LogError("StageRuntimeManager is not assigned.");
            return;
        }

        _stageRuntimeManager.SetEmotion(_selectedSourceActorId, _selectedTargetActorId, emotionType);

        // 적용 후 source는 유지하고 target만 초기화한다.
        ClearTargetSelection();
        _selectionState = RehearsalSelectionState.SourceSelected;

        RefreshView();
        RefreshSelectionViews();
    }

    private void OnResetButtonClicked()
    {
        if (_selectionState != RehearsalSelectionState.TargetSelected)
        {
            return;
        }

        if (_stageRuntimeManager == null)
        {
            Debug.LogError("StageRuntimeManager is not assigned.");
            return;
        }

        _stageRuntimeManager.SetEmotion(_selectedSourceActorId, _selectedTargetActorId, EmotionType.None);

        // 적용 후 source는 유지하고 target만 초기화한다.
        ClearTargetSelection();
        _selectionState = RehearsalSelectionState.SourceSelected;

        RefreshView();
        RefreshSelectionViews();
    }

    private void OnStartStageButtonClicked()
    {
        Debug.Log("Start Stage button clicked.");
        // 다음 단계에서 StageScene 전환 로직을 연결한다.
    }

    private void SelectSource(ActorId sourceActorId)
    {
        _selectedSourceActorId = sourceActorId;
        _selectedTargetActorId = ActorId.None;
        _selectionState = RehearsalSelectionState.SourceSelected;
    }

    private void SelectTarget(ActorId targetActorId)
    {
        if (_selectedSourceActorId == ActorId.None || _selectedSourceActorId == targetActorId)
        {
            return;
        }

        _selectedTargetActorId = targetActorId;
        _selectionState = RehearsalSelectionState.TargetSelected;
    }

    private void ClearTargetSelection()
    {
        _selectedTargetActorId = ActorId.None;
    }

    private void RefreshView()
    {
        if (_rehearsalUIView == null)
        {
            return;
        }

        SceneDefinitionSO sceneDefinition = _stageRuntimeManager != null && _stageRuntimeManager.RuntimeData != null
            ? _stageRuntimeManager.RuntimeData.SceneDefinition
            : null;

        EmotionType currentEmotion = GetCurrentSelectedRelationEmotion();

        RehearsalDisplayData displayData = _viewModel.BuildDisplayData(
            sceneDefinition,
            _selectionState,
            _selectedSourceActorId,
            _selectedTargetActorId,
            currentEmotion,
            CAN_START_STAGE_BY_DEFAULT);

        _rehearsalUIView.ApplyDisplay(displayData);
    }

    private EmotionType GetCurrentSelectedRelationEmotion()
    {
        if (_selectionState != RehearsalSelectionState.TargetSelected)
        {
            return EmotionType.None;
        }

        if (_stageRuntimeManager == null)
        {
            return EmotionType.None;
        }

        return _stageRuntimeManager.GetEmotion(_selectedSourceActorId, _selectedTargetActorId);
    }

    private void RefreshSelectionViews()
    {
        for (int i = 0; i < _actorSelectionViews.Count; i++)
        {
            ActorSelectionView selectionView = _actorSelectionViews[i];

            if (selectionView == null)
            {
                continue;
            }

            selectionView.ClearSelection();
        }

        ApplySelectionStateToActor(_selectedSourceActorId, true);
        ApplySelectionStateToActor(_selectedTargetActorId, false);
    }

    private void ApplySelectionStateToActor(ActorId actorId, bool isSource)
    {
        if (actorId == ActorId.None)
        {
            return;
        }

        ActorSelectionView selectionView = FindSelectionView(actorId);

        if (selectionView == null)
        {
            return;
        }

        if (isSource)
        {
            selectionView.ShowAsSource();
            return;
        }

        selectionView.ShowAsTarget();
    }

    private ActorSelectionView FindSelectionView(ActorId actorId)
    {
        for (int i = 0; i < _actorPresenters.Count; i++)
        {
            ActorPresenter actorPresenter = _actorPresenters[i];

            if (actorPresenter == null || actorPresenter.ActorId != actorId)
            {
                continue;
            }

            if (i >= _actorSelectionViews.Count)
            {
                return null;
            }

            return _actorSelectionViews[i];
        }

        return null;
    }
}