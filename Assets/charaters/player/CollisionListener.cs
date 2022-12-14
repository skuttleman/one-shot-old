using Game.System.Events;
using Game.System.Events.Player;
using Game.Utils.Mono;
using UnityEngine;

public class CollisionListener : Subscriber<Event<PlayerStance>> {
    [SerializeField] PlayerStance stance;
    Collider collide;

    new void Start() {
        base.Start();
        collide = GetComponent<Collider>();
    }

    public override void OnEvent(Event<PlayerStance> e) =>
        collide.enabled = e.data == stance;
}
