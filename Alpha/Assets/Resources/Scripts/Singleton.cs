using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    //Returns the instance of this singleton.
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] objects = (T[])FindObjectsOfType(typeof(T));

                if (objects.Length == 0)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
                else if(objects.Length > 1)
                {
                    Debug.LogError("Only one instance of " + typeof(T) +
                       " should exist in the scene.");
                }

                instance = objects[0];
            }

            return instance;
        }
    }
}
