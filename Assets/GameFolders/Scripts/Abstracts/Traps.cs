using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Abstracts
{
    public abstract class Traps : MonoBehaviour
    {
        [SerializeField] float _hitJumpForce;
        protected Damage _hitDamage;
        Rigidbody2D _rb;
        Health _targetHealth;
         protected void HitTarget(Collider2D collision)
        {
            // Try to find a Health component on the object that entered the trap’s collider.
            _targetHealth = collision.gameObject.GetComponent<Health>();

            // If found, apply damage using the Damage system.
            if (_targetHealth != null)
                _hitDamage.HitTarget(_targetHealth);
        }
        protected void MakeTargetJump(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            // Get the Rigidbody2D of the player to control their physics.
            _rb = collision.attachedRigidbody;

            // Stop any current movement before applying the jump force.
            _rb.linearVelocity = Vector2.zero;

            // Apply an upward force to make the player jump or bounce off the trap.
            _rb.AddForce(Vector2.up * _hitJumpForce);
        }
    }
}