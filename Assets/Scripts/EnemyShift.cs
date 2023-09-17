using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the main component attached to the parent of enemy objects.
 *  In charge of moving the group left and right and down.
 */
public class EnemyShift : MonoBehaviour
{
    public static EnemyShift s_enemyshift;

    public List<Column> Columns = new List<Column>();
    public Transition Bonus_1, Bonus_2;

    private float _t = 0;
    public float Speed = 5f;
    public Transform Group;
    private int _lapse;
    [HideInInspector]
    public int EnemyCnt, Yellow, Red;
    public int MaxLapse = 5;

    private void Awake()
    { 
        s_enemyshift = this;
        Debug.Log(s_enemyshift.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Calculate when to speed up and trigger bonus enemies.
        for(int i = 0; i < Columns.Count; i++)
            EnemyCnt += Columns[i].Enemies.Count;
        Yellow = Mathf.CeilToInt(EnemyCnt * 0.5f);
        Red = Mathf.CeilToInt(Yellow * 0.5f);

        if(FullManager.s_fullmanager == null)
            StartCoroutine(ShiftSide());
        SelectColumn();
    }

    #region Movement

    public void Begin() =>  StartCoroutine(ShiftSide());

    IEnumerator ShiftSide(bool end = false, bool left = false)
    {
        _t = 0;
        Vector3 a = new Vector3(_lapse % 2 == 0 ? 0 : 5, -_lapse, 0);
        Vector3 b = new Vector3(_lapse % 2 == 0 ? 5 : 0, -_lapse, 0);
        if (end && ((_lapse % 2 != 0 && !left) || (_lapse % 2 == 0 && left)))
        {
            a = new Vector3(_lapse % 2 == 0 ? 5 : 0, -_lapse, 0);
            b = new Vector3(_lapse % 2 == 0 ? 0 : 5, -_lapse, 0);
        }
        while (_t < 1)
        {
            Group.position = Vector3.Lerp( a, b, _t);
            _t += Time.deltaTime / Speed;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(end ? ShiftSide(end, !left) : ShiftDown());
    }

    IEnumerator ShiftDown()
    {
        float time = Time.time;
        Vector3 a = new Vector3(_lapse % 2 == 0 ? 5 : 0, -_lapse, 0);
        Vector3 b = new Vector3(_lapse % 2 == 0 ? 5 : 0, -_lapse - 1, 0);
        while (Time.time - time < 1)
        {
            Group.position = Vector3.Lerp(a, b, Time.time - time);
            yield return new WaitForEndOfFrame();
        }
        _lapse++;
        StartCoroutine(_lapse <= MaxLapse ? ShiftSide() : ShiftSide(true, _lapse % 2 == 0 ? false : true));
    }
    #endregion

    /*  Method is used to select a random enemy to shoot after the interval time.
     *  Resets if the enemy is destroyed.
     *  If no enemies are available, move to next level or game complete.
     */
    public void SelectColumn()
    {
        if(Columns.Count > 0)
            Columns[Random.Range(0, Columns.Count)].StartShot();
        else
        {
            // WIN
            Debug.Log("WINNER");
            StopAllCoroutines();
            Player.s_player.Score += (int)(Player.s_player.Timer * 100);
            if(FullManager.s_fullmanager != null)
            {
                FullManager.s_fullmanager.Level++;
                if (FullManager.s_fullmanager.Level == FullManager.s_fullmanager.Swarms.Length)
                {
                    // Game Complete.
                    Player.s_player.Score += Player.s_player.Lives * 10000;
                    Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
                    foreach (Rigidbody rb in rbs)
                        rb.velocity = Vector3.zero;
                    Player.s_player.CalculateHighScore();
                }
                else
                {
                    // Next Level.
                    StartCoroutine(NextLevel());
                }
            }
            else
            {
                // Game Complete.
                Player.s_player.Score += Player.s_player.Lives * 1000;
                Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
                foreach (Rigidbody rb in rbs)
                    rb.velocity = Vector3.zero;
                Player.s_player.CalculateHighScore();
            }
        }
    }

    IEnumerator NextLevel()
    {
        float timer = 0;
        while (timer < 5)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        FullManager.s_fullmanager.NextSwarm();
    }
}
