using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDict : MonoBehaviour
{

    public string oName;   //the name of the original object
    public int materialIndex; //indicate the color index of the object
    public int prefabIndex;  //indicate the shape index of the pbject
    public GameObject oriObject; //the original object   
    public GameObject targHolder;//where to put the original object
    public GameObject grabHolder; //the position for agent to grab the object
    public GameObject releaseHolder; //the position for agent to release the object
    public bool targTaken; //check if the target position has been taken or not
  
    public void Initialize(string name, int mid, int pid, GameObject ori, GameObject hold, GameObject gholder, GameObject rholder, bool taken)
    {
        oName = name;
        materialIndex = mid;
        prefabIndex = pid;
        oriObject = ori;
        
        targHolder = hold;
        grabHolder = gholder;
        releaseHolder = rholder;

        targTaken = taken;

          
    }

    public bool hitTarget()  //check if the object come to its target position, if comes, return true, otherwise, return false
    {
        float dist = Vector3.Distance(oriObject.transform.position, releaseHolder.transform.position);
        if (dist < 0.01f) return true;
        else return false;



    }
}
