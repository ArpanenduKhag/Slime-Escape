using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abstracts
{
    public class SingletonObject<T> : MonoBehaviour
    {
        // Public static reference to the single instance of this class.
        public static T Instance { get; private set; }

        // 'entity' is usually 'this' when called from the child class.
        protected void SingletonThisObject(T entity)
        {
            // If there’s no existing instance, assign this object as the instance.
            if (Instance == null)
            {
                Instance = entity;

                // Prevent this GameObject from being destroyed when changing scenes.
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // If another instance already exists, destroy this duplicate object.
                Destroy(this.gameObject);
            }
        }
    }
}



// It works using generics (T) — meaning any class that inherits from it (like GameManager : SingletonObject<GameManager>) automatically becomes a singleton.

// Inside the method SingletonThisObject():

// If there’s no instance, it assigns the current object as the main instance and marks it as DontDestroyOnLoad, so it stays alive when changing scenes.

// If another instance already exists, it destroys the duplicate, ensuring there’s always only one.
