using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RehearsalInputController : MonoBehaviour
{
    public event Action<ActorPresenter> ActorClicked;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private InputActionReference _pointActionReference;
    [SerializeField] private InputActionReference _clickActionReference;

    private InputAction _pointAction;
    private InputAction _clickAction;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _pointAction = _pointActionReference != null ? _pointActionReference.action : null;
        _clickAction = _clickActionReference != null ? _clickActionReference.action : null;
    }

    private void OnEnable()
    {
        if (_pointAction != null)
        {
            _pointAction.Enable();
        }

        if (_clickAction != null)
        {
            _clickAction.Enable();
            _clickAction.performed += OnClickPerformed;
        }
    }

    private void OnDisable()
    {
        if (_clickAction != null)
        {
            _clickAction.performed -= OnClickPerformed;
            _clickAction.Disable();
        }

        if (_pointAction != null)
        {
            _pointAction.Disable();
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (_mainCamera == null || _pointAction == null)
        {
            return;
        }

        Vector2 screenPosition = _pointAction.ReadValue<Vector2>();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPoint = new Vector2(worldPosition.x, worldPosition.y);

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider == null)
        {
            return;
        }

        ActorPresenter actorPresenter = hitCollider.GetComponent<ActorPresenter>();

        if (actorPresenter == null)
        {
            return;
        }

        ActorClicked?.Invoke(actorPresenter);
    }
}