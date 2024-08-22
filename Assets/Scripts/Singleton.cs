using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance => instance;
    static T instance = null;

    protected virtual void Awake()
    {
        InitSingleton();
    }

    void InitSingleton()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        instance.name += $"[{GetType().Name}]";
    } //Prevents this class to be instancied multiples times
}
