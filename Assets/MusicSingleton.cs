using UnityEngine;
using System.Collections;

public class MusicSingleton : MonoBehaviour {

    private static MusicSingleton instance = null;

	void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
