using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*  Object Pool is used to distribute projectile objects among player and enemies.
 */
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool s_sharedInstance;

    [HideInInspector]
    public List<GameObject> PooledObjects = new List<GameObject>();
    [SerializeField]
    private GameObject _objectToPool;
    [SerializeField]
    private int _amountToPool = 10;

    void Awake() => s_sharedInstance = this;

    void Start()
    {
        GameObject tmp;
        for (int i = 0; i < _amountToPool; i++)
        {
            tmp = Instantiate(_objectToPool);
            tmp.SetActive(false);
            PooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = PooledObjects.Count - 1; i >= 0; i--)
        {
            if (PooledObjects[i] == null)
                PooledObjects.RemoveAt(i);
            else if (!PooledObjects[i].activeInHierarchy)
            {
                PooledObjects[i].SetActive(true);
                return PooledObjects[i];
            }
        }
        // If all objects are active, it means none are left. So create new one.
        GameObject tmp = Instantiate(_objectToPool);
        PooledObjects.Add(tmp);

        return tmp;
    }
}
