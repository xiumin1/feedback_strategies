using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;
using UnityEditor;
using Oculus;
using RootMotion.FinalIK;

public class oldspeechdelete : MonoBehaviour
{

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    GameObject surface;
    Actions action;
    Task4Rules task;
    TeachAction t_action;
    CorrectAction c_action;
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
    Transform target;

    Transform agent_rightHand;
    Transform user_rightHand;

    public bool done;

    public bool wrongsorting;
    public bool continue_called;
    public bool done_called;
    public bool demo_called;
    public bool start_called = true;

    void Start()
    {

        //head = GameObject.Find("Head").transform;
        surface = GameObject.Find("Surface");


        //help = GameObject.Find("HelpPanel");
        assistant = GameObject.Find("Assistant");

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
        // InitOVRHand();

        //test correctAction

        //task.CreateNewTask(3);
        ////task.ShowDemos();
        //action.ActionTaken(3);


        ////load audio source
        LoadAudios();
        audioSource = assistant.GetComponent<AudioSource>();
        PlayAudio(clips[0]);  //start to music and say the introduction of the system

        ////start the level loop
        keywords.Add("start", () => { StartCalled(); });
        keywords.Add("repeat", () => { StartCalled(); });
        keywords.Add("demostration", () => { DemoCalled(); });
        keywords.Add("continue", () => { ContinueCalled(); });
        keywords.Add("done", () => { DoneCalled(); });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KWRegOnPhraseRecognized;
        keywordRecognizer.Start();


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
        if (start_called == true)
            PlayAudio(clips[1]); //start to say the rule of the system
    }
    void DemoCalled()
    {
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
        action.ActionTaken(1);  //#1 is teachaction which can show a demo
        //done = t_action.teachactionDone;

    }

    void ContinueCalled()
    {
        continue_called = true;
        if (task.taskNumber_correct < 4)
        {
            PlayAudio(clips[3]); //agent say the "your task is here, take your time"
            if (task != null)
            {
                for (
                    int i = 0; i < task.activeNum; i++)
                {
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

    void DoneCalled()
    {

        wrongsorting = false;
        int wrongscore = 0;
        for (int i = 0; i < task.activeNum; i++)
        {
            if (dict[i].hitTarget() == false)
            {
                wrongscore += 1;
            }

        }
        if (continue_called == true)  //use to make sure no multiple done called
        {
            continue_called = false;
            if (wrongscore > 0)
            {
                wrongsorting = true;
                PlayAudio(clips[5]); //agent say this is a wrong sorting
                action.ActionTaken(2); //agent execute the correct action
                                       //c_action = gameObject.GetComponent<CorrectAction>();
            }
            else
            {
                PlayAudio(clips[4]);
                task.taskNumber_correct += 1;
            }
        }

    }


    void Update()
    {

        CubeOnTable();

        assistant.transform.position = new Vector3(0.3f, 0f, 1.28f);
        assistant.transform.rotation = Quaternion.Euler(0f, -180f, 0f);

        target = centerEye;

        if (demo_called || done_called)
        {
            lookAt.solver.bodyWeight = 0.5f;
            target = agent_rightHand; //set the agent look at target to her own hand

            if (t_action == null)
            {
                t_action = GetComponent<TeachAction>();
            }
            else
            {
                if (t_action.index > task.activeNum)
                {
                    lookAt.solver.bodyWeight = 1.0f;
                    target = centerEye; //set the agent look at target to the user HMD
                }
            }


        }

        if (continue_called)
        {
            target = user_rightHand;
        }
        //LookatControl(target);
        lookAt.solver.target = target;
        //lookAt.solver.head.transform = head;
    }


    void CubeOnTable() //if the cubes drop on the ground, bring it back to the table surface
    {
        for (int i = 0; i < task.activeNum; i++)
        {
            if (dict[i].oriObject.transform.position.y < surface.transform.position.y)
                dict[i].oriObject.transform.position = task.InitPosition();
        }
    }

    void PlayAudio(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip; //play some music before the user ready
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

        audio_finished = true;
        audioSource.Stop();

        audioSource.clip = null;


        Debug.Log("wait for seconds done");

        if (system_finished == true)
        {
            UnityEditor.EditorApplication.Exit(0);
        }
    }
}

