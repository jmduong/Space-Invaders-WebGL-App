using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

/*  This is the main component attached to the player gameobject.
 */
public class Player : MonoBehaviour
{
    public static Player s_player;

    private bool _menu = false;
    private float _cooldown = 0;
    private int _lives = 3, _speed = 3;
    private List<int> _highScoreValues = new List<int>();
    private List<string> _highScoreNames = new List<string>();

    public bool Menu
    {
        get
        {
            return _menu;
        }
        set
        {
            _menu = value;
            MenuObj.SetActive(value);
            Time.timeScale = value ? 0 : 1;
        }
    }
    [HideInInspector]
    public float Timer = 60;
    public GameObject MenuObj, HighScoreSetup, HighScoreObj, GameOverObj;
    public Transform PlayerTransform;
    public int Lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;
            if(value <= 0)
            {
                // GAME OVER
                EnemyShift.s_enemyshift.StopAllCoroutines();
                CalculateHighScore();
                Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
                foreach (Rigidbody rb in rbs)
                    rb.velocity = Vector3.zero;
            }
        }
    }
    [HideInInspector]
    public int Score;
    public int Level = 1;
    public TextMeshProUGUI Text, HighScoreNames, HighScoreValues, GameOverText;
    public TMP_InputField InputField;

    private void Awake()
    { 
        s_player = this;
        Time.timeScale = 1;
    }

    private void Start()                    =>  InitialHighScores();

    public void ToggleMenu()                =>  Menu = !Menu;

    public void SceneChange(string scene)   =>  SceneManager.LoadScene(scene);

    public void Shoot()
    {
        GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject();
        obj.name = "Player Shot";
        obj.transform.position = PlayerTransform.position + Vector3.up;
        obj.GetComponent<Rigidbody>().velocity = Vector3.up * 3;
    }

    #region High Score
    /*  Calculate high scores before game starts.
     */
    public void InitialHighScores()
    {
        for(int i = 0; i < 10; i++)
            if (PlayerPrefs.GetInt(Level + " V" + i) != 0)
            {
                _highScoreValues.Add(PlayerPrefs.GetInt(Level + " V" + i));
                _highScoreNames.Add(PlayerPrefs.GetString(Level + " N" + i));

                HighScoreValues.text += "\n" + _highScoreValues[i];
                HighScoreNames.text += "\n" + _highScoreNames[i];
            }
            else
                break;
    }

    /*  Calculate high scores after game ends.
     */
    public void CalculateHighScore()
    {
        _highScoreValues.Add(Score);
        _highScoreValues.Sort((a, b) => b.CompareTo(a));
        // Determine if High Score was earned.
        GameOverObj.SetActive(true);
        if (_highScoreValues.Count <= 10 || _highScoreValues.Last() != Score)
        {
            HighScoreSetup.SetActive(true);
            GameOverText.text = "Achieved High Score";
        }
    }

    /*  Trigger if new high score is made.
     */
    public void AddHighScore()
    {
        int index = _highScoreValues.IndexOf(Score);
        _highScoreNames.Insert(index, InputField.text);
        CalculateFinalHighScore();
        HighScoreSetup.SetActive(false);
        HighScoreObj.SetActive(true);
    }

    /*  Add new high score and display results.
     */
    private void CalculateFinalHighScore()
    {
        HighScoreValues.text = "<b>Score</b>";
        HighScoreNames.text = "<b>Name</b>";
        for (int i = 0; i < _highScoreValues.Count; i++)
        {
            if (i >= 10)
                break;
            PlayerPrefs.SetInt(Level + " V" + i, _highScoreValues[i]);
            PlayerPrefs.SetString(Level + " N" + i, _highScoreNames[i]);

            HighScoreValues.text += "\n" + PlayerPrefs.GetInt(Level + " V" + i);
            HighScoreNames.text += "\n" + PlayerPrefs.GetString(Level + " N" + i);
        }
        PlayerPrefs.Save();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if(_lives > 0)
        {
            PlayerTransform.position += new Vector3((PlayerTransform.position.x > -8.5f || Input.GetAxis("Horizontal") > 0) &&
                                                    (PlayerTransform.position.x < 8.5f  || Input.GetAxis("Horizontal") < 0) ? Input.GetAxis("Horizontal") : 0,
                                                    (PlayerTransform.position.y > -3.6f || Input.GetAxis("Vertical") > 0)   && 
                                                    (PlayerTransform.position.y < 5.6f  || Input.GetAxis("Vertical") < 0)   ? Input.GetAxis("Vertical") : 0, 
                                                    0) * Time.deltaTime * _speed;
            if(EnemyShift.s_enemyshift.Columns.Count > 0)
                Timer -= Time.deltaTime;
            if (Timer <= 0) // Set to 0 to mean game over.
                Lives = 0;
            if (_cooldown > 0)
                _cooldown -= Time.deltaTime;
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
                _cooldown = 0.3f;
            }
            Text.text = string.Format("Lives: {0}\nScore: {1}\nTimer: {2}", _lives, Score, Timer.ToString("F2"));
        }
    }
}
