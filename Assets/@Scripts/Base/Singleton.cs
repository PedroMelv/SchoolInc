using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance { get { return GetInstance(); } }

    protected static bool isDestroying;

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    public static bool Exists()
    {
        return _instance != null;
    }

    protected static T GetInstance()
    {
        if (isDestroying) return null;

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
        isDestroying = true;
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
