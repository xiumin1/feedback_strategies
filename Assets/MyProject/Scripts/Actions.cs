using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour{

	//public int actionNum;

    public Task4Rules task;

    public float userSpeed;

    public Dictionary<int, ObjectDict> dict = new Dictionary<int, ObjectDict>();

    public Component comT;

    public GameObject easePoser;
    public GameObject grabPoser;
    public GameObject releasePoser;
    public GameObject pointPoser;

    public GameObject RH_grab;
    public GameObject RH_release;
    public GameObject RH_point;

    public GameObject RH_ease;
    public Vector3 RH_easePos;
    public Quaternion RH_easeRot;

    private bool AddHandPosers_called = false;

    public void Initialize(Task4Rules t)  // init actions(action number, task)
    {
        
        task = t;

        dict = t.dict;

        easePoser = GameObject.Find("RH_ease");
        grabPoser = GameObject.Find("RH_grab");
        releasePoser = GameObject.Find("RH_release");
        pointPoser = GameObject.Find("RH_point");

        //AddHandPosers();
    }

    public void AddHandPosers()
    {
        AddHandPosers_called = true;
        for (int i = 0; i < dict.Count; i++)
        {
            
            RH_grab = Instantiate(grabPoser);
            
            RH_grab.transform.position = new Vector3(dict[i].grabHolder.transform.position.x, dict[i].grabHolder.transform.position.y, dict[i].grabHolder.transform.position.z + 0.035f);//new Vector3(objects[i].transform.position.x, objects[i].transform.position.y, objects[i].transform.position.z+0.2f);           
            RH_grab.transform.parent = dict[i].grabHolder.transform;
            RH_grab.transform.localScale = new Vector3(1f, 1f, 1f);

            RH_release = Instantiate(releasePoser);
            
            RH_release.transform.position = new Vector3(dict[i].releaseHolder.transform.position.x, dict[i].releaseHolder.transform.position.y, dict[i].releaseHolder.transform.position.z + 0.035f);
            RH_release.transform.parent = dict[i].releaseHolder.transform;
            RH_release.transform.localScale = new Vector3(1f, 1f, 1f);

            RH_point = Instantiate(pointPoser);
            
            RH_point.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y+0.07f, dict[i].transform.position.z);
            RH_point.transform.parent = dict[i].oriObject.transform;
            RH_point.transform.localScale = new Vector3(1f,1f,1f);
        }

        RH_ease = Instantiate(easePoser);
        RH_ease.name = "RH_ease_clone";
        //RH_ease.transform.position = new Vector3(RH_ease.transform.position.x, RH_ease.transform.position.y+0.8f, RH_ease.transform.position.z);//newpos;       
        RH_ease.transform.position = new Vector3(0.065f, 1.068f, 0.993f);
        RH_easePos = RH_ease.transform.position;
        RH_easeRot = RH_ease.transform.rotation;
    }

    public void ActionTaken(int actionNum)
    {
        if (AddHandPosers_called == false)
        {
            AddHandPosers();
        }
            
        if (comT != null)  // check if there is action running or not
        {
            Destroy(comT);
        }
        switch (actionNum)
        {
            case 1:

                userSpeed = 0.3f;
                comT=gameObject.AddComponent<TeachAction>();
               
               // TeachAction(userSpeed, task);
                break;
            case 2:
                userSpeed = 0.3f;
                comT = gameObject.AddComponent<CorrectAction>();
                //TeachAction(userSpeed, task);
                break;
            case 3:
                userSpeed = 0.3f;
                comT = gameObject.AddComponent<SuggestAction>();
                break;
            case 4:
                userSpeed = 1.0f;
                comT = gameObject.AddComponent<SuggestAction2>();
                break;
            case 5:
                break;
            default:
                break;

        }
    }

    //public void CleanAction()  //clean the objects on the table
    //{
    //    if (comT != null)  // check if there is action running or not
    //    {
    //        gameObject.GetComponent<TeachAction>().CleanTeachAction();
    //        Destroy(comT);
    //    }
    //    //Destroy(gameObject.GetComponent<Actions>());
    //    //dict.Clear();
    //    //usedMaterial.Clear(); //clear the list that hold all the used random material numbers for the next level game
    //    //sortID.Clear();
    //}
}
