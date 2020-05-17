using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private Board m_gameBoard;

    private Spawner m_spawner;

    //currently active falling shape
    private Shape m_activeShape;

    private float waitTime = .5f;

    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

        if (!m_gameBoard || !m_spawner)
            Debug.Log("WARNING!  Board or Spawner is null");

        m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

        m_activeShape = m_spawner.SpawnShape();

        //Unity doc site says this is slower
        //m_gameBoard = GameObject.FindObjectOfType<Board>();
        //m_spawner = GameObject.FindObjectOfType<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {

        if(!m_gameBoard || !m_spawner)
        {
            return;
        }

        timer += Time.deltaTime;

        if(m_activeShape)
        {

            if(timer  > waitTime)
            {
                m_activeShape.MoveDown();
                timer = timer - waitTime;
            }

        }

    }


}
