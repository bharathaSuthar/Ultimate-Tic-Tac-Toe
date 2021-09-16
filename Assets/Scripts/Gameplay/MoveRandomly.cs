using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandomly : MonoBehaviour
{
    float moveSpeed;
    Vector3 moveDirection;
    float zRotation;
    public bool activate;
    void Start()
    {
        activate = false;
        moveSpeed = 0.1f;
        zRotation = Random.Range(-0.5f, 0.5f);
        moveDirection = new Vector3( Random.Range(-1f,1f), Random.Range(-1f,1f), 0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        activate = EventManager.isGameOver;
        if(activate){
            MoveRandom();
        }
    }
    void MoveRandom(){
        transform.Translate( moveDirection * moveSpeed * Time.deltaTime, Space.Self );
        transform.Rotate( new Vector3( 0, 0, zRotation)*Time.deltaTime );
    }
}
