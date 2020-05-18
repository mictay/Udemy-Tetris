using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    #pragma warning disable 0649
    [SerializeField]
    private Shape[] m_allShapes;
    #pragma warning restore 0649

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Shape GetRandomShape()
    {
        int i = Random.Range(0, m_allShapes.Length);

        if (m_allShapes[i])
            return m_allShapes[i];
        else
        {
            Debug.Log("WARNING!  Shape null at index=" + i);
            return null;
        }

    }


    public Shape SpawnShape()
    {
        Shape shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;

        if (shape)
            return shape;
        else
        {
            Debug.Log("WARNING! shape is null");
            return null;
        }

    }

}
