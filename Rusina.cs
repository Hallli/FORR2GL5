using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rusina : MonoBehaviour
{
private void OnCollisionEnter2D(Collision2D obj){
if(obj.gameObject.tag=="drepa"){
    Destroy(gameObject);
}
}

}
