using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance { get { return GetInstance(); } }

    protected static bool isQuitting;

    protected virtual void Awake()
    {
        _instance = this as T;

        Application.quitting += () =>
        {
            Debug.Log("Application is quitting");
            isQuitting = true;
        };

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += (PlayModeStateChange) => {
            if (PlayModeStateChange == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("PlayMode Changed to ExitingPlayMode");
                isQuitting = true;
            }
        };
#endif
    }

    public static bool Exists()
    {
        return _instance != null;
    }

    protected static T GetInstance()
    {
        if(isQuitting) return null;

        if (_instance == null) _instance = FindObjectOfType<T>();

        if(_instance == null )
        {
            GameObject newInstance = new GameObject(typeof(T).Name);
            _instance = newInstance.AddComponent<T>();
        }

        return _instance;
    }

    private void OnDestroy()
    {
        if(_instance == this)
            _instance = null;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        isQuitting = true;
    }
}

public class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            base.Awake();
            return;
        }

        Destroy(this.gameObject);
    }
}

public class SingletonPersistent<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            base.Awake();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(this.gameObject);
    }
}

public class SingletonObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;
    public static T Instance { get => GetInstance(); }

    private void Awake()
    {
        if(_instance == null)
            _instance = this as T;
    }

    private static T GetInstance()
    {
        if(_instance == null)
        {
#if UNITY_EDITOR
            if(!AssetDatabase.IsValidFolder("Resources/SO_Instances"))
            {
                AssetDatabase.CreateFolder("Resources", "SO_Instances");
            }
#endif
            _instance = (T)Resources.Load("SO_Instances/" + typeof(T).Name, typeof(T));
#if UNITY_EDITOR
            if (_instance == null)
            {
                _instance = ScriptableObject.CreateInstance(typeof(T)) as T;
                AssetDatabase.CreateAsset(_instance, "Resources/SO_Instances");
            }
#endif
        }

        return _instance;
    }
}
