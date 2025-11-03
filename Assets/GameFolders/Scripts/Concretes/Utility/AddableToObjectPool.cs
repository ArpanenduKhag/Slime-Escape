using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component marks a GameObject as "poolable" — meaning it can be managed by the ObjectPoolManager.
public class AddableToObjectPool : MonoBehaviour
{
    // Defines what type of object this is (for example: Bullet, Enemy, Particle, etc.).
    // The PoolObjectsEnum helps the ObjectPoolManager identify and organize pooled objects by type.
    [SerializeField] PoolObjectsEnum _objectType;

    // Public read-only property to access the object’s type.
    public PoolObjectsEnum ObjectType => _objectType;
}


// This script acts as a tag or identifier for objects that belong to the Object Pooling System.

// Each object that can be pooled — such as a bullet, explosion effect, or enemy — has this script attached to it.

// The _objectType (of type PoolObjectsEnum) tells the ObjectPoolManager what kind of object it is,
// so the manager knows which pool to store or retrieve it from.

// In simple terms, it’s like putting a label on a box so the pool manager knows where to place or find that object type efficiently.


//This script is the connecting bridge between the game object and the Object Pool Manager — it tells the system what type of object it is so that pooling and reusing become organized and efficient
