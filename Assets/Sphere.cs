using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour {

    ////////////////공의 물리적 특성에 관여 하는 영역////////////////////////////////////////////
    //public
    public float MAX_POWER;                                     //이 공이 낼수 있는 최대 힘을 의미
    public enum moving_state { init, stop, drag, drive };
    public moving_state cur_moving_state;   //공의 현재 상태를 나타냄
    protected int MAX_HP;                                       //공의 최대 체력
    public int HP;                                              //공의 현재 체력
    //protected
    protected Vector3 Click_pos;     //마우스, 터치를 시작한 지점
    protected Vector3 Current_pos;   //현재 마우스, 터치가 위치한 지점
    protected Vector3 diff;          //Click_pos와 Current_pos의 차
    protected float MASS;             //이 공의 질량
    //private
    private struct power_info        //공에 가해질 힘의 크기와 방향에 관한 정보

    {
        public float angle;
        public float scale;
        public power_info(float a, float s)
        {
            angle = a;
            scale = s;

        }
    }
    private power_info pow;          //해당 정보를 사용할 구조체(?)
    

    /////////////////이외의 특성에 관여하는 영역////////////////////////
    //public
    public GameObject _Sphere;//오브젝트 자기 자신을 가리키기 위해(?)
    public GameObject Arrow;//공이 이동할 방향과 크기를 알려주는 화살을 담는다 
    public GameObject ArrowHead;//화살촉
    public GameObject HP_prefab;//HP증감을 표시 할 문자 

    //protected
    protected Main_Script main_scr;  //메인 스크립트를 담는다.
    protected AudioSource CrashSound;//충돌음을 담는 변수
    //private





    //	  Use this for initialization
    protected void Start()
    {
        ////// 모든 공들이 공통적으로 초기화 해야하는 대상들
        main_scr = GameObject.Find("GameObjects").GetComponent<Main_Script>();  //메인 스크립트 로딩
        _Sphere = transform.gameObject;                                         //자기 자신
        CrashSound = GetComponent<AudioSource>();                               //충돌음
        HP_prefab = Resources.Load("Text_HP") as GameObject;                    //충돌시 HP증감 표시 글자
        pow = new power_info(0f, 0f);                                           //힘 정보도 초기화
        cur_moving_state = moving_state.init;                                   //처음 상태
        ////// 공마다 다른 값을 가져야 할 대상들
     }

    //	  Update is called once per frame
    protected void Update()
    {
        if (cur_moving_state == moving_state.stop)
        {//공이 정지중일 때				

            if(main_scr.allow_sp_control)
            if (Input.GetMouseButtonDown(0))
            {     //최초 클릭시

                RaycastHit hit = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out hit)
                    &&(hit.transform==_Sphere.transform)) {

                    cur_moving_state = moving_state.drag;
                    Click_pos = Input.mousePosition;
                    Arrow = _Sphere.transform.Find("Arrow").gameObject;
                    ArrowHead = _Sphere.transform.Find("Arrow/Head").gameObject;
                    Arrow.SetActive(true);

                }
            }
        }
        else if (cur_moving_state == moving_state.drag)
        {//화살표가 나타나고 드래그 중

            if (Input.GetMouseButtonUp(0))
            {//놓았을 때
                main_scr.allow_sp_control = false;
                cur_moving_state = moving_state.drive;
                Arrow.SetActive(false);
                _Sphere.GetComponent<Rigidbody>().AddForce(new Vector3(-pow.scale * Mathf.Cos(pow.angle) , 0f, pow.scale * Mathf.Sin(pow.angle)));
                
                diff = new Vector3(0, 0, 0);
                Arrow.transform.localScale = new Vector3(0, 0, 0);
            }
            else {//계속 드래그 중일때
                Current_pos = Input.mousePosition;
                diff = Current_pos - Click_pos;

                pow.angle = -Mathf.Atan2(diff.y, diff.x);

                pow.scale = Mathf.Min(Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y)/50*MAX_POWER,MAX_POWER);

                Arrow.transform.rotation = Quaternion.Euler(90f, pow.angle * Mathf.Rad2Deg - 90f, 0f);

                Arrow.transform.localScale = new Vector3(1, (2*pow.scale)/ MAX_POWER, 1);
                //자식 오브젝트까지 같이 키워버려서 원래대로 되돌린다..??
                if(pow.scale>0)
                ArrowHead.transform.localScale = new Vector3(1, MAX_POWER / (2*pow.scale), 1);
                Debug.Log(pow.angle+" "+pow.scale);
            }
        }
        else if (cur_moving_state == moving_state.drive)
        {//공이 움직이는 중
            if (_Sphere.GetComponent<Rigidbody>().velocity.magnitude < 1)
            {//속력이 일정 미만으로 내려가면
                cur_moving_state = moving_state.stop;
                main_scr.turn_over();
                main_scr.allow_sp_control = true;
            }
        }
        
    }

    protected void OnCollisionEnter(Collision collisioninfo)
    {

        if (cur_moving_state == moving_state.init)
        {
            cur_moving_state = moving_state.stop;
            return;
        }
        //충돌시 카메라를 흔든다. - 오픈소스 이용
        main_scr.Main_Cam.GetComponent<Cam_Mov>().shake = _Sphere.GetComponent<Rigidbody>().velocity.magnitude / 10;
        main_scr.Main_Cam.GetComponent<Cam_Mov>().shakeAmount = _Sphere.GetComponent<Rigidbody>().velocity.magnitude / 200;
        //충돌시 소리를 재생한다.
        CrashSound.volume = _Sphere.GetComponent<Rigidbody>().velocity.magnitude / 200;
        CrashSound.PlayOneShot(CrashSound.clip);

        //데미지 표시 글자를 뿌려줄 위치. 이거 정확한 위치를 어떻게 가져오지
        Vector3 pos = new Vector3(0.22f + 0.5f * (_Sphere.transform.localPosition.x + 4.23f) / 8f, 0.07f + 0.89f * (_Sphere.transform.localPosition.z + 5.26f) / 9.06f, 0);


        //데미지를 입어야 하는 물체와 충돌
        if (collisioninfo.gameObject.tag == "Damaging_Objects")
        {

            Debug.Log(collisioninfo.gameObject.name);

            //충돌시 데미지를 입는다.
            int damage = Mathf.RoundToInt(_Sphere.GetComponent<Rigidbody>().velocity.magnitude / 3);

            //HP의 범위는 0~
            if (HP > 0)
            {
                HP = HP - damage;
            }
            else
            {
                HP = 0;
            }

            //공이 위치하는 곳에 체력 감소 수치를 뿌린다.
            if (damage > 0)
            {
                GameObject Player_HP = Instantiate(HP_prefab) as GameObject;
                Player_HP.GetComponent<GUIText>().text = "-" + damage;
                Player_HP.GetComponent<Transform>().position = pos;
            }
        }
        //회복되는 물체와 충돌
        else if (collisioninfo.gameObject.tag == "Healing_Objects")
        {
            int healing_amount = Mathf.RoundToInt(_Sphere.GetComponent<Rigidbody>().velocity.magnitude / 4);
            if (HP + healing_amount < MAX_HP)
                HP = HP + healing_amount;
            else
                HP = MAX_HP;
            //공이 위치하는 곳에 체력 증가 수치를 뿌린다.            
            GameObject Player_HP = Instantiate(HP_prefab) as GameObject;
            Player_HP.GetComponent<GUIText>().text = "+" + healing_amount;
            Player_HP.GetComponent<Transform>().position = pos;

        }

        //이외의 물체와 충돌
        else {

        }


        //구체의 밝기는 HP에 비례한다.
        Light lt = _Sphere.transform.Find("Point light").gameObject.GetComponent<Light>();
        lt.intensity = 8 * HP / MAX_HP;


    }
    
}
