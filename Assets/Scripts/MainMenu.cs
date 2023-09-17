using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/*  This is the main component used to manage all methods needed for the initial scene.
 */
public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI HighScoreTitle, HighScoreNames, HighScoreValues;

    void Start()                        =>  LoadHighScores(1);

    public void LoadScene(string level) =>  SceneManager.LoadScene("Level " + level);

    /*  Load all high scores based on the level selected.
     *  By default, level 1 is selected in the beginning.
     */
    public void LoadHighScores(int level)
    {
        HighScoreTitle.text = level == 0 ? "High Score: Full Game" : "High Score: Level " + level;
        HighScoreValues.text = "<b>Score</b>";
        HighScoreNames.text = "<b>Name</b>";
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetInt(level + " V" + i) != 0)
            {
                HighScoreValues.text += "\n" + PlayerPrefs.GetInt(level + " V" + i);
                HighScoreNames.text += "\n" + PlayerPrefs.GetString(level + " N" + i);
            }
            else
                break;
        }

    }

    public void Exit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
