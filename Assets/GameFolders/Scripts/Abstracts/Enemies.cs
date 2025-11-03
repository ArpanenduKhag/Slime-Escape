using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat; 


public class Enemies : MonoBehaviour
{
    [SerializeField] float _hitJumpForce;  
    protected Damage _hitDamage;        
    Rigidbody2D _rb;                       
    Health _targetHealth;                 

    // Called when the enemy collides with another object and deals damage if possible.
    protected void HitTarget(Collision2D collision)
    {
        // Try to get the Health component from the object that the enemy collided with.
        _targetHealth = collision.gameObject.GetComponent<Health>();

        // If the object has a Health component, deal damage to it using the Damage script.
        if (_targetHealth != null)
            _hitDamage.HitTarget(_targetHealth);
    }

    // Makes the collided target jump upwards — usually the player when they land on the enemy.
    protected void MakeTargetJump(Collision2D collision)
    {
        // Only execute if the object has the tag "Player".
        if (!collision.gameObject.CompareTag("Player")) return;

        // Get the Rigidbody2D of the player.
        _rb = collision.rigidbody;

        // Reset current movement (so the jump starts cleanly).
        _rb.linearVelocity = Vector2.zero;

        // Apply an upward force to make the player jump or bounce.
        _rb.AddForce(Vector2.up * _hitJumpForce);
    }
}


// HitTarget() — checks if the object the enemy hit has a Health component. If yes, it applies damage using a Damage class.

// MakeTargetJump() — when the player collides with the enemy, it applies an upward force to the player’s Rigidbody to make them bounce or jump off.