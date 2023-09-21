using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HorizontalWeapon : MonoBehaviour
{
    private List<GameObject> _objectsInAttackZone;

    private void OnCollisionEnter2D(Collision2D other)
    {
        _objectsInAttackZone.Add(other.gameObject);
        print(other.gameObject.name);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        _objectsInAttackZone.Remove(other.gameObject);
    }
    
    
}
