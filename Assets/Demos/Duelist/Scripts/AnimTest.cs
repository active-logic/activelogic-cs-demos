using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour{

    float stamp;

    //void Start() => Invoke("Hit", 4f);
    //void Hit() => animator.CrossFade("Strike", 0.1f);

    void Start(){
        stamp = Time.time;
    }

    void Update(){
        if(Time.time-stamp>3) animator.CrossFade("Strike", 0.1f);
    }

    Animator animator => GetComponentInChildren<Animator>();

}
