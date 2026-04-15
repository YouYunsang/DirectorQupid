using System.Collections.Generic;
using UnityEngine;
using CupidDirector.Utils;

public class RehearsalController : MonoBehaviour
{
    private const bool CAN_START_STAGE_BY_DEFAULT = true;

    [SerializeField] private StageRuntimeManager _stageRuntimeManager;
    [SerializeField] private RehearsalUIView _rehearsalUIView;
    [SerializeField] private RehearsalInputController _rehearsalInputController;
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
        BindInputController();
        BindUIView();
        RefreshView();
        RefreshSelectionViews();
    }

    private void OnDisable()
    {
        UnbindInputController();
        UnbindUIView();
    }

    private void BindInputController()
    {
        if (_rehearsalInputController == null)
        {
            return;
        }

        _rehearsalInputController.ActorClicked += OnActorPresenterClicked;
    }

    private void UnbindInputController()
    {
        if (_rehearsalInputController == null)
        {
            return;
        }

        _rehearsalInputController.ActorClicked -= OnActorPresenterClicked;
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

    private void OnActorPresenterClicked(ActorPresenter actorPresenter)
    {
        if (actorPresenter == null)
        {
            return;
        }

        OnActorClicked(actorPresenter.ActorId);
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
        if (_selectedSourceActorId == clickedActorId)
        {
            ClearAllSelection();
            return;
        }

        SelectTarget(clickedActorId);
    }

    private void HandleActorClickWhenTargetSelected(ActorId clickedActorId)
    {
        if (_selectedSourceActorId == clickedActorId)
        {
            ClearAllSelection();
            return;
        }

        if (_selectedTargetActorId == clickedActorId)
        {
            ClearTargetSelection();
            _selectionState = RehearsalSelectionState.SourceSelected;
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

        ClearTargetSelection();
        _selectionState = RehearsalSelectionState.SourceSelected;

        RefreshView();
        RefreshSelectionViews();
    }

    private void OnStartStageButtonClicked()
    {
        Debug.Log("Start Stage button clicked.");
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

    private void ClearAllSelection()
    {
        _selectedSourceActorId = ActorId.None;
        _selectedTargetActorId = ActorId.None;
        _selectionState = RehearsalSelectionState.None;
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