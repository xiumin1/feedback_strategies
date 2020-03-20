using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;
using UnityEditor;
using Oculus;
using RootMotion.FinalIK;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpeechTest : MonoBehaviour {

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    //public FullBodyBipedIK ik;
    GameObject surface; 
    Actions action;
    Task4Rules task;
    TeachAction t_action;
    CorrectAction c_action;
    SuggestAction s_action;
    GameObject ease;
    GameObject help;

    public Dictionary<int, ObjectDict> dict = new Dictionary<int, ObjectDict>();


    public List<AudioClip> clips = new List<AudioClip>();
    public AudioSource audioSource;

   
    bool system_finished = false; // use to check if the system finished or not
    bool audio_finished = false;


    GameObject assistant;
    //to setup the lookat scripts
    LookAtIK lookAt;
    Transform centerEye;
    public Transform target;

    Transform agent_rightHand;
    Transform user_rightHand;

    public bool done;

    public bool wrongsorting;
    public bool continue_called=false;
    public bool done_called = false;
    public bool demo_called = false;
    public bool start_called = true;
    public bool suggest_called = true;

    int wrongnumber = 0;
    int wrongscore = 0;
    public bool suggest_done = true;
    bool demoaudio_finished = false;

  
    bool done1 = false;
    bool correct_found = false;

    bool no_continue = false;
    bool no_suggest = false;
    bool no_demo = false;
    bool no_done = false;
    bool userstart = false;

    public string dataPath = "D:/Users/paras/Desktop/VR Study Systems/data.txt";
    //Material red, m1;
    void Start () {

        surface = GameObject.Find("Surface");

        
        //help = GameObject.Find("HelpPanel");
        assistant = GameObject.Find("Assistant");

        //ik = assistant.GetComponent<FullBodyBipedIK>();
        //setup lookat bones
        lookAt = assistant.GetComponent<LookAtIK>();
        centerEye = GameObject.Find("CenterEyeAnchor").transform;


        agent_rightHand = GameObject.Find("RightHand").transform;
        user_rightHand = GameObject.Find("RightHandAnchor").transform;
        //initialize the task
        task = surface.AddComponent<Task4Rules>();
        task.Initialize();
        action = surface.AddComponent<Actions>();
        action.Initialize(task);
        dict = task.dict;

        //s_action = gameObject.GetComponent<SuggestAction>();
        // InitOVRHand();

        //test correctAction
        
        //task.CreateNewTask(3);
        ////task.ShowDemos();
        //action.ActionTaken(3);


        ////load audio source
        LoadAudios();
        audioSource = assistant.GetComponent<AudioSource>();
        PlayAudio(clips[0]);  //start to music and say the introduction of the system
        //MoveTargHolder();
        ////start the level loop
        keywords.Add("start", ()=> { StartCalled(); });
        //keywords.Add("repeat", () => { StartCalled(); });
        keywords.Add("demostration", ()=> { DemoCalled(); });
        keywords.Add("continue", ()=> { ContinueCalled(); });
        keywords.Add("done", () => { DoneCalled(); });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KWRegOnPhraseRecognized;
        keywordRecognizer.Start();
           
        Debug.Log("play audio 3");

        if (File.Exists(dataPath) == false)
        {
            var outfile = new StreamWriter(dataPath, true);
            outfile.WriteLine("Create File  " + System.DateTime.Now.ToString());
            outfile.WriteLine("------  " + System.DateTime.UtcNow.ToString());
            outfile.Close();
        }
        else
        {
            var outfile = new StreamWriter(dataPath, true);
            outfile.WriteLine("*****************************PARTICIPANT***CORRECTIVE FEEDBACK*****************************"+ System.DateTime.UtcNow.ToString());
            outfile.WriteLine("****************************** system 1 with picture set 1 ************************");
            //outfile.WriteLine("------  " + sce_name.name + "  ------  " + System.DateTime.UtcNow.ToString());
            outfile.Close();
        }

    }

    void KWRegOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {            
            keywordAction.Invoke();
        }
    }

    void StartCalled()
    {
        userstart = true;
        if (start_called == true)
        {
            var outfile = new StreamWriter(dataPath, true);
            outfile.WriteLine("start called once at UTC time = " + System.DateTime.UtcNow.ToString());
            outfile.Close();

            PlayAudio(clips[1]); //start to say the rule of the system
            //start_called = false;
        }
            
    }
    void DemoCalled()
    {
        if (!no_demo)
        {
            if (demo_called == false)
            {
                var outfile = new StreamWriter(dataPath, true);
                outfile.WriteLine("demo called once at UTC time = " + System.DateTime.UtcNow.ToString());
                outfile.Close();

                userstart = true;
                start_called = false;//to control the start introduction, never call it again

                // if (audioSource.isPlaying) audioSource.Stop();

                //demo_called = true;

                PlayAudio(clips[2]);

                task.ShowDemos();

                //task.CreateTask(LEVEL);

                t_action = gameObject.GetComponent<TeachAction>();
                if (t_action != null) Destroy(t_action);
                ease = GameObject.Find("RH_ease_clone");
                if (ease != null) Destroy(ease);
                StartCoroutine(waitDemoAudio(13.0f));
                //action.ActionTaken(1);  //#1 is teachaction which can show a demo
                //done = t_action.teachactionDone;
                //demo_called = true;
                no_demo = true;
            }

        }
        else
        {
            PlayAudio(clips[6]);
        }

    }

    void ContinueCalled()
    {
        //if (c_action != null)
        //{
        //    if(c_action.R_hand.GetComponent<HandPoser>().poseRoot!=null)
        //    c_action.R_hand.GetComponent<HandPoser>().poseRoot = null;
        //}
        if (!no_continue)
        {
            var outfile = new StreamWriter(dataPath, true);
            outfile.WriteLine("continue called once at UTC time = " + System.DateTime.UtcNow.ToString());
            outfile.Close();

            demo_called = true;
            continue_called = true;
            done_called = false;
            userstart = true;
            correct_found = false;
            //start_called = false;

            if (CheckCorrect() == true) //until all correct, you can proceed to next task
            {
                if (task.taskNumber_correct < 4)
                {
                    PlayAudio(clips[3]); //agent say the "your task is here, take your time"
                    if (task != null)
                    {
                        //int k = 0;

                        for (int i = 0; i < task.activeNum; i++)
                        {
                            //k = task.matNum[i] - 1;
                            dict[i].oriObject.SetActive(false);
                            dict[i].targHolder.SetActive(false);
                        }

                    }

                    task.CreateNewTask(task.taskNumber_correct * 2 + 3);  //number=taskNumber_correct*2+3

                }

                else
                {
                    PlayAudio(clips[6]);
                }
            }
        }
        else
        {
            PlayAudio(clips[6]);
        }



    }

    

    void DoneCalled()
    {
        if (!no_done)
        {
           

            done_called = true;
            wrongsorting = false;
            wrongscore = 0;
            for (int i = 0; i < task.activeNum; i++)
            {
                if (dict[i].hitTarget() == false)
                {
                    wrongscore += 1;
                }

            }

            var outfile = new StreamWriter(dataPath, true);
            outfile.WriteLine("done called once at UTC time = " + System.DateTime.UtcNow.ToString() +"  wrongscore = "+ wrongscore.ToString());
            outfile.Close();

            if (continue_called == true)  //use to make sure no multiple done called
            {
                continue_called = false;
                if (wrongscore > 0)
                {
                    wrongsorting = true;
                    PlayAudio(clips[5]); //agent say this is a wrong sorting
                    MoveTargHolder();
                    action.ActionTaken(2); 
                    //agent execute the correct action
                                           //c_action = gameObject.GetComponent<CorrectAction>();
                }
                else
                {
                    PlayAudio(clips[4]);
                   // task.taskNumber_correct += 1;

                    if (task.taskNumber_correct > 4)
                    {

                        PlayAudio(clips[6]);
                        no_demo = true;
                        no_continue = true;
                        no_suggest = true;
                    }
                    else
                    {
                        PlayAudio(clips[4]);
                    }

                }
            }

        }
        else
        {
            PlayAudio(clips[6]);
        }

    }

    void Update ()
    {

        if (!userstart)
        {
            PlayAudio_start(clips[0]);
        }

        CubeOnTable();
        if (assistant.transform.position != Vector3.zero)
        {
            
            assistant.transform.position = new Vector3(0.3f, 0f, 1.28f);
            assistant.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }

        target = centerEye;

        if (demo_called == false)
        {
            if (start_called == false)
            {
                lookAt.solver.bodyWeight = 1.0f;
                // target = agent_rightHand; //set the agent look at target to her own hand

                if (t_action == null)
                {
                    t_action = GetComponent<TeachAction>();
                }
                else
                {
                    if (t_action.index < task.activeNum + 1)

                    {
                        lookAt.solver.bodyWeight = 1.0f;
                        target = agent_rightHand; //set the agent look at target to the user HMD
                    }
                }
            }

        }

        if (done_called == true && wrongscore > 0)
        {
            lookAt.solver.bodyWeight = 1.0f;

            //target = agent_rightHand; //set the agent look at target to her own hand

            if (c_action == null)
            {
                c_action = GetComponent<CorrectAction>();
            }
            else
            {
                if (c_action.index < task.activeNum)

                {
                    lookAt.solver.bodyWeight = 1.0f;
                    target = agent_rightHand; //set the agent look at target to the user HMD
                }
            }
        }

        if (continue_called)
        {
            target = user_rightHand;
        }


        //LookatControl(target);

        if (target.transform.position != Vector3.zero)
        {

            lookAt.solver.target = target;
        }





        //lookAt.solver.head.transform = head;

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCalled();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DemoCalled();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ContinueCalled();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DoneCalled();
        }
    }

    void MoveTargHolder()
    {
        float x, y, z;
        for (int i = 0; i < task.activeNum; i++)
        {

            if (dict[i].hitTarget() == false)
            {
                //dict[task.matNum[i]].grabHolder.transform.parent = null;
                x = dict[i].oriObject.transform.position.x;
                y = dict[i].oriObject.transform.position.y;
                z = dict[i].oriObject.transform.position.z;
               
                dict[i].grabHolder.transform.position = new Vector3(x, y+0.07f, z);

                //dict[task.matNum[i]].grabHolder.transform.parent = dict[task.matNum[i]].oriObject.transform;

            }
        }
    }

    void CubeOnTable() //if the cubes drop on the ground, bring it back to the table surface
    {
        if (task.taskNumber_correct < 5)
        {
            for (int i = 0; i < task.activeNum; i++)
            {
                //k = task.matNum[i] - 1;
                if (dict[i].oriObject.transform.position.y < surface.transform.position.y)
                    dict[i].oriObject.transform.position = task.InitPosition();
            }
        }
        

    }

    bool CheckOnHolder()
    {
        int ct = 0;
        if (continue_called == true)
        {
            bool comein;
            //int ki = 0;
            //int kj = 0;
            for (int i = 0; i < task.activeNum; i++)
            {
                comein = true;
                //ki = task.matNum[i] - 1;
                for (int j = 0; j < task.activeNum; j++)
                {
                    //kj = task.matNum[j] - 1;
                    if (comein == true)
                    {
                        if (Vector3.Distance(dict[i].targHolder.transform.position, dict[i].oriObject.transform.position) < 0.05f)
                        {
                            ct += 1;
                            comein = false;


                        }
                    }
                }


            }
        }
        

        if (ct < task.activeNum)
            return false;   //not all cubes are on holder
        else
        {
            //suggest_action = true;
            return true;   //all cubes are on holder
            
        }
            

    }

    bool CheckCorrect()
    {
        //int ct=0;
        wrongnumber = 0;
        if (task.taskNumber_correct == 0)
        {
            return true;
        }
        else
        {
            //int k = 0;
            for (int i = 0; i < task.activeNum; i++)
            {
                //k = task.matNum[i] - 1;
                if (dict[i].hitTarget() == false)
                {
                    wrongnumber += 1;
                }
            }

            if (wrongnumber > 0)
                return false;  //some cubes on in wrong positions
            else
                return true;  // all cubes are in right positions
        }
        
    }

    void PlayAudio(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = clip; //play some music before the user ready
        audioSource.Play();
    }

    void PlayAudio_start(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();

            if (audioSource.clip.length > 0)
            {
                StartCoroutine(waitforSeconds(audioSource.clip.length));
            }
        }
    }

    void LoadAudios()
    {
        AudioClip[] ac = (AudioClip[])Resources.LoadAll<AudioClip>("Audios/Correct");
        foreach (AudioClip a in ac)
        {
            clips.Add(a);
        }
    }

    IEnumerator waitforSeconds(float audioLength)
    {
        

        yield return new WaitForSeconds(audioLength);

        Debug.Log("ssss");


        //audio_finished = true;
        //audioSource.Stop();

        //audioSource.clip = null;

        //Debug.Log("play audio 2");
        //Debug.Log("wait for seconds done");
        
        //if (system_finished == true)
        //{
        //    UnityEditor.EditorApplication.Exit(0);
        //}
    }

    IEnumerator waitDemoAudio(float length)
    {
        yield return new WaitForSeconds(length);
        action.ActionTaken(1);

    }

    IEnumerator waitNextSuggest(float t)
    {
        //suggest_done= s_action.suggest_done;
        yield return new WaitForSeconds(t);
        if (CheckCorrect())
        {
            if (correct_found == false)
            {
                if (audioSource.isPlaying) audioSource.Stop();

                //task.taskNumber_suggest += 1;
                if (task.taskNumber_correct > 3)
                {
                    PlayAudio(clips[6]);
                    no_demo = true;
                    no_continue = true;
                    no_suggest = true;
                }
                else
                {
                    PlayAudio(clips[4]);
                }
                    
                correct_found = true;
                continue_called = false;
            }

        }

        else
        {
            if (audioSource.isPlaying) audioSource.Stop();

            MoveTargHolder();
            PlayAudio(clips[7 + wrongnumber]);
            action.ActionTaken(3);
            s_action = gameObject.GetComponent<SuggestAction>();
            suggest_done = s_action.suggest_done;
            //StartCoroutine(waitColorChange(3.0f));
        }


        //suggest_done = s_action.suggest_done;

    }

    IEnumerator waitColorChange(float t)
    {
        //second set all target holder color to red if the object on it is not right
        //int k;
       
        yield return new WaitForSeconds(t);

       

    }
}
