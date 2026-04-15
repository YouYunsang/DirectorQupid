using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using CupidDirector.Utils;

public class RehearsalUIView : MonoBehaviour
{
    public event Action<EmotionType> EmotionButtonClicked;
    public event Action ResetButtonClicked;
    public event Action StartStageButtonClicked;

    [SerializeField] private TMP_Text _sceneTitleText;
    [SerializeField] private TMP_Text _scriptText;
    [SerializeField] private TMP_Text _selectionText;
    [SerializeField] private TMP_Text _relationText;

    [SerializeField] private Button _loveButton;
    [SerializeField] private Button _hateButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _startStageButton;

    private void Awake()
    {
        BindButtons();
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    public void ApplyDisplay(RehearsalDisplayData displayData)
    {
        if (displayData == null)
        {
            Debug.LogWarning("DisplayData is null.");
            return;
        }

        SetText(_sceneTitleText, displayData.SceneTitle);
        SetText(_scriptText, displayData.ScriptText);
        SetText(_selectionText, displayData.SelectionText);
        SetText(_relationText, displayData.RelationText);
    }

    private void BindButtons()
    {
        if (_loveButton != null)
        {
            _loveButton.onClick.AddListener(OnLoveButtonClicked);
        }

        if (_hateButton != null)
        {
            _hateButton.onClick.AddListener(OnHateButtonClicked);
        }

        if (_resetButton != null)
        {
            _resetButton.onClick.AddListener(OnResetButtonClicked);
        }

        if (_startStageButton != null)
        {
            _startStageButton.onClick.AddListener(OnStartStageButtonClicked);
        }
    }

    private void UnbindButtons()
    {
        if (_loveButton != null)
        {
            _loveButton.onClick.RemoveListener(OnLoveButtonClicked);
        }

        if (_hateButton != null)
        {
            _hateButton.onClick.RemoveListener(OnHateButtonClicked);
        }

        if (_resetButton != null)
        {
            _resetButton.onClick.RemoveListener(OnResetButtonClicked);
        }

        if (_startStageButton != null)
        {
            _startStageButton.onClick.RemoveListener(OnStartStageButtonClicked);
        }
    }

    private void OnLoveButtonClicked()
    {
        EmotionButtonClicked?.Invoke(EmotionType.Love);
    }

    private void OnHateButtonClicked()
    {
        EmotionButtonClicked?.Invoke(EmotionType.Hate);
    }

    private void OnResetButtonClicked()
    {
        ResetButtonClicked?.Invoke();
    }

    private void OnStartStageButtonClicked()
    {
        StartStageButtonClicked?.Invoke();
    }

    private void SetText(TMP_Text targetText, string value)
    {
        if (targetText == null)
        {
            return;
        }

        targetText.text = value;
    }

    private void SetEmotionButtonsInteractable(bool canApplyEmotion)
    {
        if (_loveButton != null)
        {
            _loveButton.interactable = canApplyEmotion;
        }

        if (_hateButton != null)
        {
            _hateButton.interactable = canApplyEmotion;
        }

        if (_resetButton != null)
        {
            _resetButton.interactable = canApplyEmotion;
        }
    }

    private void SetStartStageButtonInteractable(bool canStartStage)
    {
        if (_startStageButton != null)
        {
            _startStageButton.interactable = canStartStage;
        }
    }
}
