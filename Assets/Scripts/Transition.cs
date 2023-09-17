using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the component used to trigger bonus enemy gameobjects.
 */
public class Transition : MonoBehaviour
{
    private float _t;
    public bool FromLeft;

    public void StartShift()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(ShiftSide());
    }

    IEnumerator ShiftSide()
    {
        _t = 0;
        Vector3 a = 10 * (FromLeft ? Vector3.left : Vector3.right);
        Vector3 b = -a;
        while (_t < 1)
        {
            transform.position = Vector3.Lerp(a, b, _t);
            _t += Time.deltaTime / 10;
            yield return new WaitForEndOfFrame();
        }
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
