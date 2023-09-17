using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the main component attached to enemy gameobjects with the "Respawn" tag.
 */
public class Enemy : MonoBehaviour
{
    public Column Col;

    public int Interval, Score;

    // Trigger when gameobject is destroyed. Triggered by projectile script.
    public void Destroyed()
    {
        Player.s_player.Score += Score;
        if(Col != null) // Exception are bonus enemies.
        {
            Col.Enemies.Remove(this);

            // Used to check when to speed up enemy shift.
            int remaining = 0;
            for (int i = 0; i < EnemyShift.s_enemyshift.Columns.Count; i++)
                remaining += EnemyShift.s_enemyshift.Columns[i].Enemies.Count;

            if (remaining == EnemyShift.s_enemyshift.Red)
            {
                EnemyShift.s_enemyshift.Speed = 1f;
                Debug.Log("BONUS Start 2");
                EnemyShift.s_enemyshift.Bonus_2.StartShift();
            }
            else if (remaining == EnemyShift.s_enemyshift.Yellow)
            {
                EnemyShift.s_enemyshift.Speed = 3f;
                Debug.Log("BONUS Start 1");
                EnemyShift.s_enemyshift.Bonus_1.StartShift();
            }

            if (Col.Enemies.Count == 0)
            {
                Col.StopAllCoroutines();
                EnemyShift.s_enemyshift.Columns.Remove(Col);
                EnemyShift.s_enemyshift.SelectColumn();
                Destroy(Col.gameObject);
            }
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Player.s_player.Lives--;
    }

    /*  Used to shoot a projectile. 
     *  Triggered by Column script.
     */
    public void Shoot()
    {
        GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject();
        obj.name = "Enemy Shot";
        obj.transform.position = transform.position + Vector3.down;
        obj.GetComponent<Rigidbody>().velocity = Vector3.down * 2;
    }
}
