﻿using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _hasInstance;

    protected bool IsUsed = false;

    void Awake()
    {
        CheckDuplication();
        if (IsUsed && CheckDontDestroyOnLoad())
        {
            DontDestroyOnLoad(gameObject);
        }

        OnAwake();
    }

    void OnDestroy()
    {
        if (IsUsed)
        {
            _hasInstance = false;
            _instance = (T)((object)null);
        }

        OnDestroyed();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnDestroyed()
    {

    }

    protected virtual bool CheckDontDestroyOnLoad()
    {
        return true;
    }

    private void CheckDuplication()
    {
        Singleton<T> singleton = this;
        T[] array = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (array.Length >= 2)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Singleton<T> singleton2 = array[i] as Singleton<T>;
                if (singleton2.IsUsed)
                {
                    singleton = singleton2;
                    break;
                }

                if (this != singleton2)
                {
                    Destroy(singleton2.gameObject);
                }
            }
        }

        if (singleton != this)
        {
            Destroy(gameObject);
        }

        if (_instance == null)
        {
            SetInstance(singleton as T);
        }
    }

    private static void FindOrCreateInstance()
    {
        T[] array = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (array.Length >= 1)
        {
            SetInstance(array[0]);
        }
        else
        {
            Create(typeof(T).ToString());
        }
    }

    protected static void SetInstance(T instance)
    {
        _instance = instance;
        _hasInstance = _instance != null;
        Singleton<T> singleton = _instance as Singleton<T>;
        singleton.IsUsed = true;
    }

    public static bool Exist()
    {
        if (!_hasInstance)
        {
            FindOrCreateInstance();
        }

        return _hasInstance;
    }

    public static T Create(string name)
    {
        if (_hasInstance)
        {
            return _instance;
        }

        GameObject gameObject = new GameObject(name);
        T t = gameObject.AddComponent<T>();
        SetInstance(t);

        return t;
    }

    public static T Instance()
    {
        if (!_hasInstance)
        {
            FindOrCreateInstance();
        }

        return _instance;
    }

    public static bool HasInstance()
    {
        return _hasInstance;
    }
}