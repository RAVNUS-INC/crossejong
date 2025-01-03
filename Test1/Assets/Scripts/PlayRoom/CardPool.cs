using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // �̸� ������ ������Ʈ ������
    public int poolSize = 60; // �ʱ� Ǯ ũ��

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // �ʱ� Ǯ ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Ǯ�� �����ִ� ������Ʈ�� ���� ��� ���� ����
            GameObject obj = Instantiate(prefab);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
