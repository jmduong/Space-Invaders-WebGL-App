using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the main component used to organize multi-level scenes.
 *  Triggers the next levels and complete status.
 */
public class FullManager : MonoBehaviour
{
    public static FullManager s_fullmanager;
    public Transform[] Swarms;
    public int Level = 0;

    private void Awake()    =>  s_fullmanager = this;

    void Start()            =>  NextSwarm();

    public void NextSwarm() =>  StartCoroutine(Swarm());

    /*  Used to manage the change from one level to the next.
     *  Moves the next enemy swarm into the scene from above.
     */
    IEnumerator Swarm()
    {
        float time = Time.time;
        Vector3 a = Vector3.up * 5;
        Swarms[Level].gameObject.SetActive(true);
        while (Time.time - time < 1)
        {
            Swarms[Level].position = Vector3.Lerp(a, Vector3.zero, Time.time - time);
            yield return new WaitForEndOfFrame();
        }
        Swarms[Level].GetComponent<EnemyShift>().Begin();
        Player.s_player.Timer = 60;
        EnemyShift.s_enemyshift.Speed = 5f;
    }
}
