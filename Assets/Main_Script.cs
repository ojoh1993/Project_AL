
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Main_Script : MonoBehaviour {
    

    public enum Game_State { Intro, IN_Game };
    public Game_State cur_game_state;
    public int turn;
    public int max_player;
    public int max_sphere_amount;
    public int sp1_remain, sp2_remain;
    public bool allow_sp_control;

    public GameObject Main_Cam;   //메인 카메라를 담는 변수
    public float cam_rotation_angle;


    private GameObject logo;
    private GameObject back;
    private GameObject Touch_To_Start;
    private GameObject Text_GameOver;
    private GameObject Black_out;
    private int Text_tick;
    private int T_Switch;
    private int Cam_tick;

    //게임 컴포넌트들
    private GameObject Sphere1,Sphere2,Obstacle;
    private List<GameObject> _sp1, _sp2, _ob;
    private GameObject m;
   
    // Use this for initialization
    void Start () {

        this.turn = 0;
        this.max_player = 2;
        this.max_sphere_amount = 2;
        
        _sp1 = new List<GameObject>();
        _sp2 = new List<GameObject>();
        _ob = new List<GameObject>();

        //가로모드 고정
        Screen.orientation = ScreenOrientation.Landscape;
        
        cur_game_state = Game_State.Intro;

        Main_Cam = GameObject.Find("Main Camera").gameObject;                  //메인 카메라
        //카메라
        cam_rotation_angle = 0;
        //기타 UI
        Touch_To_Start = GameObject.Find("Text").gameObject;
        logo=GameObject.Find("Logo");
        //back=GameObject.Find("Back");
        Text_GameOver = GameObject.Find("Text_GameOver");
        Black_out = GameObject.Find("Black_out");
        //back.SetActive(false);
        Text_tick = 0;
        T_Switch = 1;
        
        //게임 컴포넌트들을 가지고 온다.
        Sphere1 = Resources.Load("Sphere1") as GameObject;
        Sphere2 = Resources.Load("Sphere2") as GameObject;
        Obstacle = Resources.Load("Obstacle") as GameObject;
        m = Resources.Load("Smoke Trail") as GameObject;
        m.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", new Vector4(1f, .54f, .66f, 1f));

    }

    // Update is called once per frame
    void Update () {
        
        //인트로 화면 처리
        if (cur_game_state == Game_State.Intro)
        {
            //텍스트 깜빡이기 처리
            Text_tick += T_Switch;
            if (T_Switch == +1 && Text_tick > (1 / Time.deltaTime))
            {
                T_Switch = -1; Touch_To_Start.SetActive(false);
            }
            if (T_Switch == -1 && Text_tick < 0)
            {
                T_Switch = 1; Touch_To_Start.SetActive(true);
            }
            //뒤로가기 누르면 종료
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            //클릭시 게임 메인으로 넘어감
            if (Input.GetMouseButtonDown(0))
            {
                sp1_remain = 2;
                sp2_remain = 2;
                allow_sp_control = true;

                logo.SetActive(false);
                //back.SetActive(true);
                Touch_To_Start.SetActive(false);
                cur_game_state = Game_State.IN_Game;
                
                _sp1.Add(Instantiate(Sphere1) as GameObject);
                _sp1[0].transform.parent = GameObject.Find("GameObjects").transform;
                _sp1[0].transform.position = new Vector3(2.5f, 51f, 2.5f)+Random.onUnitSphere;

                _sp1.Add(Instantiate(Sphere1) as GameObject);
                _sp1[1].transform.parent = GameObject.Find("GameObjects").transform;
                _sp1[1].transform.position = new Vector3(-2.5f, 52f, 2.5f) + Random.onUnitSphere;

                _sp2.Add(Instantiate(Sphere2) as GameObject);
                _sp2[0].transform.parent = GameObject.Find("GameObjects").transform;
                _sp2[0].transform.position = new Vector3(-2.5f, 53f, -2.5f) + Random.onUnitSphere;

                _sp2.Add(Instantiate(Sphere2) as GameObject);
                _sp2[1].transform.parent = GameObject.Find("GameObjects").transform;
                _sp2[1].transform.position = new Vector3(2.5f, 54f, -2.5f) + Random.onUnitSphere;


                _ob.Add(Instantiate(Obstacle) as GameObject);
                _ob[0].transform.parent = GameObject.Find("GameObjects").transform;
                _ob[0].transform.position = new Vector3(0, 49f, 0);



            }
        }
        //게임 안에서
        else if (cur_game_state == Game_State.IN_Game)
        {
            //뒤로가기를 누를 경우 인트로로 혹은 뒤로가기 버튼을 클릭할 경우(?)-추가하지 말까..
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cur_game_state = Game_State.Intro;
                reset();
            }
            //게임 오버 조건
            if (sp1_remain == 0)
            {
                cur_game_state = Game_State.Intro;
                allow_sp_control = false;
                StartCoroutine("Fade_out");
            }
            else if (sp2_remain == 0)
            {
                cur_game_state = Game_State.Intro;
                allow_sp_control = false;
                StartCoroutine("Fade_out");
            }
        }
        
    }


    private void reset()
    {
        this.turn = 0;
        m.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", new Vector4(1f, .54f, .66f, 1f));

        Main_Cam.GetComponent<Intro_Cam>().Switch_Mod();
        Main_Cam.transform.position = new Vector3(-3f, 63f, -125f);
        Main_Cam.transform.rotation = Quaternion.Euler(21f, 0f, 0f);
       logo.SetActive(true);
        // back.SetActive(false);
        Touch_To_Start.SetActive(true);

        for (int i = 0; i < max_sphere_amount; i++)
        {
            Destroy(_sp1[i]);
            Destroy(_sp2[i]);
        }
        Destroy(_ob[0]);
        _sp1.Clear();
        _sp2.Clear();
        _ob.Clear();

    }

    IEnumerator Fade_out() {
        Debug.Log("Coroutine Start");
        Image b = Black_out.GetComponent<Image>();
        for (float f = 0f; f <= 1f; f += Time.deltaTime)
        {
            Color asdf = b.color;
            asdf.a = f;
            b.color = asdf;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        reset();
        for (float f = 1f; f >= 0f; f -= Time.deltaTime*2)
        {
            Color asdf = b.color;
            asdf.a = f;
            b.color = asdf;
            yield return null;
        }
    }
    

    //턴 관련한 이슈들도 여기서 일단은..
    public void turn_over() {

        turn = (turn + 1) % max_player;

        if (turn == 0){
            m.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", new Vector4(1f, .54f, .66f, 1f));
        }

        else if (turn == 1){
            m.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", new Vector4(.46f, .75f, 1f, 1f));
        }

    }

}
