using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class TriggerController : MonoBehaviour
{
    float _initialCooldownTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _initialCooldownTime = collision.gameObject.GetComponent<Health>().CooldownTimeAfterHit;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().CooldownTimeAfterHit = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<Health>().CooldownTimeAfterHit = _initialCooldownTime;
    }
}



// When the player enters, the script saves their current cooldown value.

// While they stay inside, it sets the cooldown to 0, meaning the player can take damage continuously (for example, from spikes).

// When the player leaves, it restores their original cooldown value.