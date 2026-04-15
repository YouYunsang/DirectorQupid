using UnityEngine;
using CupidDirector.Utils;

[RequireComponent(typeof(Collider2D))]
public class ActorPresenter : MonoBehaviour
{
    public ActorId ActorId => _actorId;

    [SerializeField] private ActorId _actorId = ActorId.None;
}