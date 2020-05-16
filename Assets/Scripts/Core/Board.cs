using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    public Transform m_emptySprite;

    [SerializeField]
    public int m_height = 30;

    [SerializeField]
    public int m_width = 10;

    [SerializeField]
    public int m_header = 8;

    [SerializeField]
    public Transform[,] m_grid;

    void Awake()
    {
        m_grid = new Transform[m_width, m_height];
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawEmptyCells()
    {
        
        if(m_emptySprite)
        {

            for (int y = 0; y < m_height - m_header; y++)
            {

                for (int x = 0; x < m_width; x++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
                    clone.name = "Board Space (x=" + x.ToString() + " y=" + y.ToString() + " )";
                    clone.transform.parent = transform;
                }

            }


        } else
        {
            Debug.Log("WARNING!  Please assign the emptySquare prefab");
        }

    }

}
