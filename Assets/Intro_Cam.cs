using UnityEngine;
using System.Collections;

public class Intro_Cam : MonoBehaviour {

    public float cam_rotation_angle;
    private float Z;
    private Vector3 pos;
    private Vector3 start_pos, dest_pos;
    private Vector3 start_angle, dest_angle;
    public float tick, F, MAX;
    public enum Cam_mode { intro, to_main, main };
    public Cam_mode mod;
	// Use this for initialization
	void Start () {

        cam_rotation_angle = 0;
        Z = -125;
        pos = new Vector3(0, 63, Z);
        transform.position = pos;
        transform.rotation = Quaternion.Euler(21f,0f,0f);

        mod = Cam_mode.intro;

        dest_pos = new Vector3(0f, 46f, 0f);
        start_angle = new Vector3(transform.rotation.eulerAngles.x,
                                        transform.rotation.eulerAngles.y,
                                        transform.rotation.eulerAngles.z);
        dest_angle = new Vector3(90f, 0f, 0f);
        tick = 0f;
        F = 1.5f;
        MAX = 1 / Time.deltaTime;
    }


	
	// Update is called once per frame
	void Update () {


        if (mod == Cam_mode.intro)
        {
            cam_rotation_angle = (cam_rotation_angle + Time.deltaTime * 20) % 360;
            pos.Set(Z * Mathf.Sin(cam_rotation_angle * Mathf.Deg2Rad),
                                             this.transform.position.y,
                                              Z * Mathf.Cos(cam_rotation_angle * Mathf.Deg2Rad));
            transform.position = pos;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, cam_rotation_angle, transform.rotation.eulerAngles.z);
        }
        else if (mod == Cam_mode.to_main) {
            if (tick < MAX) tick += 1f;
            else {
                this.GetComponent<Cam_Mov>().enabled = true;
                mod = Cam_mode.main;
            }
            transform.position = start_pos + (dest_pos - start_pos)*tick/MAX;
            transform.rotation = Quaternion.Euler(start_angle.x + (dest_angle - start_angle).x*Mathf.Pow(tick/MAX,3),
                                                    start_angle.y + (dest_angle - start_angle).y * Mathf.Pow(tick / MAX,5),
                                                    start_angle.z + (dest_angle - start_angle).z * Mathf.Pow(tick / MAX,5)
                                                    );
        }

        if (Input.GetMouseButtonDown(0)&&mod==Cam_mode.intro) {
            Switch_Mod();
        }

    }

    public void Switch_Mod() {
        if (mod == Cam_mode.main || mod== Cam_mode.to_main)
        {
            GetComponent<Cam_Mov>().stop_cam();     
            this.GetComponent<Cam_Mov>().enabled = false;
            mod = Cam_mode.intro;
            tick = 0;
        }
        else if (mod == Cam_mode.intro) {
            mod = Cam_mode.to_main;
            start_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            start_angle = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
