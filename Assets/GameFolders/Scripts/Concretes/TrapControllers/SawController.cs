using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abstracts;
using Combat;

public class SawController : Traps
{
    [SerializeField] Transform[] _patrolPoints;         
    [SerializeField] float _moveSpeed;            
    [SerializeField] Transform SawPositionsParent;  

    Vector2[] _patrolPositions;                     
    Vector2 _movePos;                                
    int _patrolPointIndex = 0;                       
    Vector3 _currentPointPos;
    Vector3 _startPointPos;                          
    
    private void Awake()
    {
        // Inherit the _hitDamage functionality from the Traps class
        // and get the Damage component attached to this saw.
        _hitDamage = GetComponent<Damage>();
    }

    private void Start()
    {
        // Initialize first patrol point and record starting position.
        _currentPointPos = _patrolPoints[0].position;
        _startPointPos = transform.position;

        // Store patrol points and organize them under the parent for clarity.
        _patrolPositions = new Vector2[_patrolPoints.Length];
        foreach (Transform position in _patrolPoints)
        {
            position.SetParent(SawPositionsParent);
        }
    }

    private void Update()
    {
        // Move toward the current patrol point.
        _movePos = _currentPointPos - transform.position;
        transform.Translate(_movePos.normalized * _moveSpeed * Time.deltaTime, Space.World);

        // If the saw is close enough to the current patrol point, go to the next one.
        if (Vector3.Distance(transform.position, _currentPointPos) <= 1f)
        {
            NextPoint();
        }
    }

    private void NextPoint()
    {
        _patrolPointIndex++;

        if (_patrolPointIndex == _patrolPoints.Length)
        {
            // Return to start point once all patrol points are covered.
            _currentPointPos = _startPointPos;
            return;
        }
        else if (_patrolPointIndex > _patrolPoints.Length)
        {
            // Reset index when reaching the end of patrol list.
            _patrolPointIndex = 0;
        }

        // Set the next target patrol point.
        _currentPointPos = _patrolPoints[_patrolPointIndex].position;
    }

    // Detect player collisions to apply damage and knockback.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HitTarget(collision);       
            MakeTargetJump(collision);  
        }
    }
}
