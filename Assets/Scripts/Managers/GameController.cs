using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private Board m_gameBoard;

    private Spawner m_spawner;

    //currently active falling shape
    private Shape m_activeShape;


    [SerializeField]
    [Range(.02f, 1f)]
    private float m_keyRepeatRate = .15f;

    [SerializeField]
    [Range(.02f, 1f)]
    private float m_downRepeatRate = .01f;

    [SerializeField]
    [Range(.02f, 1f)]
    private float m_rotateRepeatRate = .25f;

    private float m_keyRepeatTimer = 0.0f;
    private float m_downRepeatTimer = 0.0f;
    private float m_rotateRepeatTimer = 0.0f;

    private bool m_isGameOver = false;

    public GameObject m_gameOverPanel;

    SoundManager m_soundManager;

    /********************************************************************************
     * 
     */
    void Start()
    {
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

        if (!m_gameBoard || !m_spawner || !m_soundManager)
            Debug.Log("WARNING!  GameBoard, Spawner, or SoundManager is null");

        m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

        m_activeShape = m_spawner.SpawnShape();

        //Unity doc site says this is slower
        //m_gameBoard = GameObject.FindObjectOfType<Board>();
        //m_spawner = GameObject.FindObjectOfType<Spawner>();

        if(m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

    }

    /********************************************************************************
     * 
     */
    void Update()
    {

        if (!m_activeShape || !m_gameBoard || !m_spawner && m_isGameOver)
        {
            Debug.Log("WARNING!  Instance of Shape or Board is missing");
            return;
        }

        PlayerInput();
    }

    /********************************************************************************
     * 
     */
    void PlayerInput()
    {
        float deltaTime = Time.deltaTime;

        if (!m_gameBoard || !m_spawner)
        {
            return;
        }

        m_keyRepeatTimer += deltaTime;        
        m_rotateRepeatTimer += deltaTime;
        m_downRepeatTimer += deltaTime;

        if (Input.GetButton("MoveRight") && m_keyRepeatTimer > m_keyRepeatRate || Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_keyRepeatTimer = 0;
            m_activeShape.MoveRight();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            } else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }

        }
        else if (Input.GetButton("MoveLeft") && m_keyRepeatTimer > m_keyRepeatRate || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_keyRepeatTimer = 0;
            m_activeShape.MoveLeft();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }

        }
        else if (Input.GetButtonDown("Rotate") && m_rotateRepeatTimer > m_rotateRepeatRate && Input.GetKey(KeyCode.LeftShift))
        {
            m_rotateRepeatTimer = 0;
            m_activeShape.RotateLeft();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateRight();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }

        }
        else if (Input.GetButtonDown("Rotate") && m_rotateRepeatTimer > m_rotateRepeatRate)
        {
            m_rotateRepeatTimer = 0;
            m_activeShape.RotateRight();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateLeft();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }

        }
        else if (Input.GetButton("MoveDown") && (m_keyRepeatTimer > m_keyRepeatRate) || m_downRepeatTimer > m_downRepeatRate)
        {
            m_keyRepeatTimer = 0;
            m_downRepeatTimer = 0;

            if (m_activeShape)
            {
                m_activeShape.MoveDown();

                if (!m_gameBoard.IsValidPosition(m_activeShape))
                {
                    if(m_gameBoard.IsOverLimit(m_activeShape))
                    {
                        GameOver();
                    }
                    else
                    {
                        LandShape();
                    }
                    
                }

            }

        }

    }

    /********************************************************************************
     * 
     */
    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_isGameOver = true;
        m_gameOverPanel.SetActive(true);
    }

    /********************************************************************************
    * 
    */
    private void LandShape()
    {
        m_keyRepeatTimer = 0;
        m_downRepeatTimer = 0;
        m_rotateRepeatTimer = 0;

        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        m_activeShape = m_spawner.SpawnShape();
        m_gameBoard.ClearAllRows();
    }

    /********************************************************************************
    * 
    */
    public void Restart()
    {
        LoadLevel(0);
    }

    // reloads the currently active scene
    public static void ReloadLevel()
    {
        // reload by scene name
        //LoadLevel(SceneManager.GetActiveScene().name);

        // or by index
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    // loads a level by index with some error checking
    public static void LoadLevel(int levelIndex)
    {
        // if the index is valid...
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            // load the scene by index
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("LoadLevel Error: invalid scene specified!");
        }
    }

    // loads a level by name with error checking
    public static void LoadLevel(string levelName)
    {
        // if the scene is in the BuildSettings, load the scene
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("LoadLevel Error: invalid scene specified!");
        }
    }

    // plays a sound with an option volume multiplier
    void PlaySound(AudioClip clip, float volMultiplier = 1.0f)
    {
        if (m_soundManager.m_fxEnabled && clip)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier, 0.05f, 1f));
        }
    }

}
