using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abstracts;
using Combat;

namespace Controllers
{

    public class FiretrapController : Traps
    {
        Animator _anim;           
        BoxCollider2D _collider;   
        float _currentTime;        

        [SerializeField] float _maxTime;     
        [SerializeField] float _startDelay;  
        private void Awake()
        {
            // Get required components and initialize trap state.
            _collider = GetComponent<BoxCollider2D>();
            _collider.enabled = false;        
            _anim = GetComponent<Animator>();  

            _hitDamage = GetComponent<Damage>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // When something enters the firetrap's collider, deal damage.
            HitTarget(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // While the target stays in the firetrap, apply additional effects like knockback or bounce.
            MakeTargetJump(collision);
        }

        private void Update()
        {
            // Wait until the initial delay period has passed before starting the trap logic.
            if (Time.timeSinceLevelLoad < _startDelay) return;

            // Increment timer by time passed per frame.
            _currentTime += Time.deltaTime;

            // FIRE ACTIVE 
            if (_currentTime > _maxTime)
            {
                _anim.SetBool("IsFire", true);     // Trigger fire animation.
                _anim.SetBool("PreFire", false);   // Disable pre-fire animation.
                _collider.enabled = true;          // Enable collider (trap can deal damage now).
                _currentTime = 0f;                 // Reset timer for next cycle.
            }

            // PRE-FIRE WARNING  
            else if (_currentTime > _maxTime / 1.18)
            {
                _anim.SetBool("PreFire", true);    // Trigger pre-fire animation (warning visuals).
            }

            // FIRE COOLDOWN  
            else if (_currentTime > _maxTime / 3)
            {
                _anim.SetBool("IsFire", false);    // Stop fire animation.
                _collider.enabled = false;         // Disable collider (no damage now).
            }
        }
    }
}
