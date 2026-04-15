using System.Collections;
using TMPro;
using UnityEngine;
using CupidDirector.Utils;

public class StageActorView : MonoBehaviour
{
    public ActorId ActorId => _actorId;
    public Vector3 StartPosition => _startPosition;

    [SerializeField] private ActorId _actorId = ActorId.None;
    [SerializeField] private Transform _anchorTransform;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private float _moveSpeed = 2.5f;

    private Vector3 _startPosition;
    private Coroutine _dialogueCoroutine;

    private void Awake()
    {
        _startPosition = _anchorTransform != null ? _anchorTransform.position : transform.position;
        HideDialogueImmediate();
    }

    public void ResetToStartPosition()
    {
        if (_anchorTransform != null)
        {
            _anchorTransform.position = _startPosition;
            return;
        }

        transform.position = _startPosition;
    }

    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        Transform targetTransform = _anchorTransform != null ? _anchorTransform : transform;

        while (Vector3.Distance(targetTransform.position, targetPosition) > 0.05f)
        {
            targetTransform.position = Vector3.MoveTowards(
                targetTransform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime);

            yield return null;
        }

        targetTransform.position = targetPosition;
    }

    public void ShowDialogue(string dialogue, float duration)
    {
        if (_dialogueText == null)
        {
            return;
        }

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
        }

        _dialogueCoroutine = StartCoroutine(ShowDialogueRoutine(dialogue, duration));
    }

    public void HideDialogueImmediate()
    {
        if (_dialogueText == null)
        {
            return;
        }

        _dialogueText.text = string.Empty;
        _dialogueText.gameObject.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return _anchorTransform != null ? _anchorTransform.position : transform.position;
    }

    private IEnumerator ShowDialogueRoutine(string dialogue, float duration)
    {
        _dialogueText.gameObject.SetActive(true);
        _dialogueText.text = dialogue;

        yield return new WaitForSeconds(duration);

        _dialogueText.text = string.Empty;
        _dialogueText.gameObject.SetActive(false);
        _dialogueCoroutine = null;
    }
}