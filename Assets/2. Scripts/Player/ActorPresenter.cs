using UnityEngine;
using System;
using CupidDirector.Utils;

[RequireComponent(typeof(Collider2D))]
public class ActorPresenter : MonoBehaviour
{
    public event Action<ActorId> Clicked;
    public ActorId ActorId => _actorId;

    [SerializeField] private ActorId _actorId = ActorId.None;

    private void OnMouseDown()
    {
        if(_actorId == ActorId.None)
        {
            Debug.LogWarning($"{name} has invalid ActorId.");
            return;
        }

        Clicked?.Invoke(_actorId);
    }
}
