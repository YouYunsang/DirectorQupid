using System;

[Serializable]
public class RehearsalDisplayData
{
    public string SceneTitle => _sceneTitle;
    public string ScriptText => _scriptText;
    public string SelectionText => _selectionText;
    public string RelationText => _relationText;

    public bool CanApplyEmotion => _canApplyEmotion;
    public bool CanStartStage => _canStartStage;

    private readonly string _sceneTitle;
    private readonly string _scriptText;
    private readonly string _selectionText;
    private readonly string _relationText;
    private readonly bool _canApplyEmotion;
    private readonly bool _canStartStage;

    public RehearsalDisplayData(
        string sceneTitle,
        string scriptText,
        string selectionText,
        string relationText,
        bool canApplyEmotion,
        bool canStartStage)
    {
        _sceneTitle = sceneTitle;
        _scriptText = scriptText;
        _selectionText = selectionText;
        _relationText = relationText;
        _canApplyEmotion = canApplyEmotion;
        _canStartStage = canStartStage;
    }
}