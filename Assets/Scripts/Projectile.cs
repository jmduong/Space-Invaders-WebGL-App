using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the main component attached to projectiles.
 */
public class Projectile : MonoBehaviour
{
    private float _timer = 0;

    private void OnEnable() =>  _timer = 0;

    public void OnTriggerEnter(Collider other)
    {
        // Check the other gameobject to see if it is a player or enemy. Names are set by player/enemy.
        if (name == "Enemy Shot" && other.CompareTag("Player"))
        {
            Player.s_player.Lives--;
            gameObject.SetActive(false);
        }
        else if (name == "Player Shot" && other.CompareTag("Respawn"))
        {
            other.GetComponent<Enemy>().Destroyed();
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 8)
            gameObject.SetActive(false);
    }
}
