using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the component used to reference enemies in the same column.
 */
public class Column : MonoBehaviour
{
    [SerializeField]
    public List<Enemy> Enemies = new List<Enemy>();

    private void Awake()
    {
        // Assign all enem children under gameobject. 
        foreach (Enemy enemy in Enemies)
            enemy.Col = this;
    }

    /*  Initiate timer to shoot based on interval of enemy object.
     *  Method is used in EnemyShift script.
     */
    public void StartShot()
    {
        StopAllCoroutines();
        StartCoroutine(ShotInterval());
    }

    IEnumerator ShotInterval()
    {
        float time = Time.time;
        while (Time.time - time < Enemies[0].Interval)
            yield return new WaitForEndOfFrame();
        Enemies[0].Shoot();
        StartShot();
    }
}
