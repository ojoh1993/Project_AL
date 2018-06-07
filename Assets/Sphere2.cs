using UnityEngine;
using System.Collections;

public class Sphere2 : Sphere { 

    //	  Use this for initialization
    new void Start()
    {
        base.Start(); 
        MAX_POWER = 300;
        MAX_HP = 19;
        HP = MAX_HP;
        _Sphere.GetComponent<Rigidbody>().mass = MASS = .12f;
    
    }

    //	  Update is called once per frame
    new void Update()
    {
        //≈œ »Æ¿Œ
        if (main_scr.turn == 1) base.Update();
    }

    new void OnCollisionEnter(Collision collisioninfo)
    {
        base.OnCollisionEnter(collisioninfo);
        if (HP == 0)
        {
            if (main_scr.turn == 1) main_scr.turn_over();
            main_scr.allow_sp_control = true;
            main_scr.sp2_remain--;
            _Sphere.SetActive(false);
        }
    }



}
