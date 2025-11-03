using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetDetection : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField][Range(3, 10)] float _detectRange;
    [SerializeField][Range(3, 20)] float _actionRange;
    float _maxDistance;


    public Vector2 TargetPos => _target.position;
    public bool IsTargetInActionRange => _maxDistance < _actionRange;
    public bool IsTargetInDetectionRange => _maxDistance < _detectRange;
    public bool IsTargetOnLeft => _target.position.x < transform.position.x;
    public bool IsTargetOnRight => _target.position.x > transform.position.x;


    private void Update()
    {
        // Continuously calculate the distance between the object and its target every frame.
        _maxDistance = Vector2.Distance(transform.position, _target.position);

    }
    private void OnDrawGizmos()
    {
        // Red circle = detection range (how far the object can see the target)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        // Yellow circle = action range (when the object will start to act, e.g., attack)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _actionRange);
    }
}



// A Detection Range (red circle) — how far the enemy can “see” the player.
// An Action Range (yellow circle) — the range at which it starts performing actions like attacking or chasing.

//OnDrawGizmos() method helps visualize these ranges directly in the Unity Scene view
