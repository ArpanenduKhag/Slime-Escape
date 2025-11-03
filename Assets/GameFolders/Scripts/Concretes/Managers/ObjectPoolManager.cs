using Abstracts;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectPoolManager handles object pooling —
// it reuses game objects instead of creating and destroying them repeatedly,
// improving performance (especially for bullets, enemies, or effects).
public class ObjectPoolManager : SingletonObject<ObjectPoolManager>
{
    [SerializeField] AddableToObjectPool[] _prefabs;  // Array of prefab objects that can be pooled (e.g., bullets, particles, enemies).
    [SerializeField] int _queueLength;                // Number of objects to pre-instantiate for each type.

    // Dictionary to store a queue (FIFO) of reusable objects for each object type.
    Dictionary<PoolObjectsEnum, Queue<AddableToObjectPool>> _objectsDictionary =
        new Dictionary<PoolObjectsEnum, Queue<AddableToObjectPool>>();

    // Awake runs before Start — ensures this manager acts as a Singleton (only one instance exists).
    private void Awake()
    {
        SingletonThisObject(this); // Sets this instance as the global ObjectPoolManager.
    }

    // Start initializes the pool system once the scene starts.
    private void Start()
    {
        InitalizePool();
    }

    // Creates the initial pool of objects for each prefab type.
    private void InitalizePool()
    {
        // Loop through all prefabs.
        for (int i = 0; i < _prefabs.Length; i++)
        {
            // Create a new queue for this prefab type.
            Queue<AddableToObjectPool> objectQueue = new Queue<AddableToObjectPool>();

            // Instantiate a fixed number of objects for the pool.
            for (int j = 0; j < _queueLength; j++)
            {
                AddableToObjectPool newObject = Instantiate(_prefabs[i]); // Create a new instance.
                newObject.gameObject.SetActive(false);                   // Deactivate it until needed.
                newObject.transform.parent = this.transform;             // Keep hierarchy clean.
                objectQueue.Enqueue(newObject);                          // Add it to the queue.
            }

            // Add this queue to the dictionary, using an enum key for easy access.
            _objectsDictionary.Add((PoolObjectsEnum)i, objectQueue);
        }
    }

    // Adds (returns) an object back into the pool after use.
    public void SetPool(AddableToObjectPool newObj)
    {
        newObj.gameObject.SetActive(false);              // Deactivate the object.
        newObj.transform.parent = this.transform;        // Reparent it under the pool manager.
        Queue<AddableToObjectPool> gameObjectsQueue = _objectsDictionary[newObj.ObjectType];
        gameObjectsQueue.Enqueue(newObj);                // Add it back to the queue.
    }

    // Retrieves (gets) an object from the pool when needed.
    public AddableToObjectPool GetFromPool(PoolObjectsEnum newObjectType)
    {
        Queue<AddableToObjectPool> gameObjectsQueue = _objectsDictionary[newObjectType];

        // If no objects are available, create a new one dynamically.
        if (gameObjectsQueue.Count == 0)
        {
            AddableToObjectPool newObj = Instantiate(_prefabs[(int)newObjectType]);
            gameObjectsQueue.Enqueue(newObj);
        }

        // Return one object from the pool for use in gameplay.
        return gameObjectsQueue.Dequeue();
    }
}



// It uses the Singleton pattern so that only one global instance of the pool exists across the whole game.

// Here’s how it works:

// At startup, it pre-instantiates a number of objects (like bullets or effects) and stores them in queues inside a dictionary.
// When a script needs an object, it calls GetFromPool() to reuse an inactive one.
// When the object is no longer needed, it’s returned to the pool using SetPool().
// This system reduces runtime lag and garbage collection overhead, especially when many objects appear and disappear rapidly — for example, in shooting or platformer games.