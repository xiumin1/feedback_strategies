using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDict : MonoBehaviour {

    public GameObject oriObject; //the original object   
    public GameObject targHolder;//where to put the original object
    public GameObject grabHolder; //the position for agent to grab the object
    public GameObject releaseHolder; //the position for agent to release the object
    public bool targTaken; //check if the target position has been taken or not

    public int sortID; //for the mix cube sorting to compare if it is right sorted or not

    public void Initialize(int id, GameObject ori, GameObject hold, GameObject gholder, GameObject rholder, bool taken)
    {
        sortID = id;
        oriObject = ori;
        targHolder = hold;
        grabHolder = gholder;
        releaseHolder = rholder;
        targTaken = taken;
        //grabHolder.transform.position = oriObject.transform.position;
        //grabHolder.transform.parent = oriObject.transform;
    }

    public bool hitTarget()  //check if the object come to its target position, if comes, return true, otherwise, return false
    {
        float dist = Vector3.Distance(oriObject.transform.position, releaseHolder.transform.position);
        if (dist < 0.1f) return true;
        else return false;
    }

    
}
