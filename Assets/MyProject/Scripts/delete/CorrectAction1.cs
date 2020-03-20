using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class CorrectAction1 : MonoBehaviour
{
    public Actions action;
    public float speed;
    public Task4Rules task;
    public Dictionary<int, ObjectDict> dict = new Dictionary<int, ObjectDict>();

    public FullBodyBipedIK ik;
    public LookAtIK lookAt;
    //public GameObject Agent;
    //public Transform agent;

    private Transform R_shoulderPos;
    private float rot_x, rot_y;
    public Transform R_hand;

    public int index = 0;

    

    public Transform start_obj;
    public Transform end_obj;
    //private Transform move_obj;

    private float startTime;
    private float journeyLength;
    private float fracJourney;
    private float distJourney;

    private float threshold = 0.03f; //can also use to adjust the pose will stuck somewhere or not

    private bool step_1 = true;
    private bool step_2 = true;
    private bool step_3 = true;

    void Start()
    {

        action = GameObject.Find("Surface").GetComponent<Actions>();
        speed = action.userSpeed;
        task = action.task;
        dict = task.dict;


        //to get the right shoulder position
        ik = GameObject.Find("Assistant").GetComponent<FullBodyBipedIK>();
        R_shoulderPos = ik.solver.rightArmChain.nodes[0].transform;
        R_hand = ik.solver.rightArmChain.nodes[2].transform;

        startTime = Time.time;

        index = getIndex(); //the first cube that is in the wrong position

        for (int i = 0; i < task.activeNum; i++)
        {
            dict[i].grabHolder.transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (index < task.activeNum )
        {

           
            if (step_1)
            {

                start_obj = action.RH_ease.transform; //GameObject.Find("RightHand").transform;//
                end_obj = dict[index].grabHolder.transform;//  grabHolder[index].transform;
                HandMove_object(start_obj, end_obj);

                R_hand.GetComponent<HandPoser>().poseRoot = action.releasePoser.transform;// releasePoser.transform;

                if (Mathf.Abs(fracJourney - 1) < threshold)
                {
                    step_1 = false;
                    dict[index].oriObject.transform.parent = R_hand;  //setup the hand as parent transform                                                                                                                                                                                                                
                    R_hand.GetComponent<HandPoser>().poseRoot = end_obj.GetChild(0); //grab object and hold hand poser

                    startTime = Time.time;
                }
            }
            else
            {
                if (step_2)   //move to location and pickup the cube
                {
                    dict[index].oriObject.GetComponent<Rigidbody>().isKinematic = true;
                    start_obj = dict[index].grabHolder.transform;
                    end_obj = action.RH_ease.transform;
                    HandMove_object(start_obj, end_obj);
                    if (Mathf.Abs(fracJourney - 1) < threshold)
                    {
                        dict[index].oriObject.GetComponent<Rigidbody>().isKinematic = false;
                        //make sure the objects put in the target position and rotation
                        dict[index].oriObject.transform.position = new Vector3(end_obj.transform.position.x, end_obj.transform.position.y - 0.07f, end_obj.transform.position.z);
                        dict[index].oriObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

                        //dict[index].oriObject.transform.parent = null; //here set the surface as parent

                        //start_obj.GetComponent<Rigidbody>().isKinematic = false;
                        //here set the parent to a fix object,in case later the parent move will bring the object move too, the point is to release the object from hand                                                   
                        //R_hand.GetComponent<HandPoser>().poseRoot = end_obj.GetChild(0); //release object and loose hand poser

                        step_2 = false;
                        startTime = Time.time;
                    }
                }
                else   //move to release location and release the cube
                {
                    if (step_3)
                    {
                        dict[index].oriObject.GetComponent<Rigidbody>().isKinematic = true;
                        start_obj = action.RH_ease.transform;
                        end_obj = dict[index].releaseHolder.transform;
                        HandMove_object(start_obj, end_obj);
                        if (Mathf.Abs(fracJourney - 1) < threshold)
                        {
                            dict[index].oriObject.GetComponent<Rigidbody>().isKinematic = false;
                            //make sure the objects put in the target position and rotation
                            dict[index].oriObject.transform.position = new Vector3(end_obj.transform.position.x, end_obj.transform.position.y - 0.07f, end_obj.transform.position.z);
                            dict[index].oriObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

                            dict[index].oriObject.transform.parent = null; //here set the surface as parent

                            //start_obj.GetComponent<Rigidbody>().isKinematic = false;
                            //here set the parent to a fix object,in case later the parent move will bring the object move too, the point is to release the object from hand                                                   
                            R_hand.GetComponent<HandPoser>().poseRoot = end_obj.GetChild(0); //release object and loose hand poser

                            step_3 = false;
                            startTime = Time.time;
                        }
                    }
                    else
                    {
                        start_obj = dict[index].releaseHolder.transform;
                        end_obj = action.RH_ease.transform;// dict[index].grabHolder.transform;
                        HandMove_object(start_obj, end_obj);

                        if (Mathf.Abs(fracJourney - 1) < threshold)
                        {
                            
                            //dict[index].oriObject.transform.parent = null;
                            R_hand.GetComponent<HandPoser>().poseRoot = null;// end_obj.GetChild(0);  //grab object and hold hand poser
                            startTime = Time.time;
                            dict[index].grabHolder.transform.position = dict[index].oriObject.transform.position;
                            dict[index].grabHolder.transform.rotation = dict[index].oriObject.transform.rotation;
                            

                            index = getIndex();
                            step_1 = true;
                            step_2 = true;
                            step_3 = true;
                            //if (index == task.activeNum)
                            //{
                            //    task.taskNumber_correct += 1;

                            //    for (int i = 0; i < task.activeNum; i++)
                            //    {
                            //        dict[i].grabHolder.transform.position = dict[i].oriObject.transform.position;
                            //        dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;
                            //        dict[i].grabHolder.transform.parent = dict[i].oriObject.transform;
                            //    }
                            //}
                        }
                    }
                }

            }

        }
        else
        {
            ik.solver.rightHandEffector.positionWeight = 0.0f;
            ik.solver.rightHandEffector.rotationWeight = 0.0f; //back to idle pose                               
                                                               //back to idle for left hand
        }
    }

    public int getIndex()
    {
        int num=0;
        while (num < task.activeNum)
        {
            if (dict[num].hitTarget() == false) break;
            num += 1;
        }
        return num;
    }

    public Vector3 endEffectorRotation(Transform handPivot, Transform shoulder)
    {
        float distance = Vector3.Distance(handPivot.position, shoulder.position);
        float rot_y = shoulder.position.x - handPivot.position.x;
        float rot_x = handPivot.position.y - shoulder.position.y;  //because of the right direction


        float angl_y = Mathf.Asin(rot_y / distance) * Mathf.Rad2Deg;
        float angl_x = Mathf.Asin(rot_x / distance) * Mathf.Rad2Deg;
        Vector3 rot = new Vector3(angl_x / 2, angl_y, 0);

        return rot;

    }


    public void HandMove_object(Transform startPos, Transform endPos) //startPos, endPos are the cube transform, cube->pivot->hand
    {
        //first rotate the hand Pivot to the right position
        //rotate pivot position to make hand reach rotation right

        Vector2 Sdis = new Vector2(startPos.position.x, startPos.position.z);
        Vector2 Edis = new Vector2(endPos.position.x, endPos.position.z);

        journeyLength = Vector2.Distance(Sdis, Edis);
        distJourney = (Time.time - startTime) * speed;
        fracJourney = distJourney / journeyLength;
        fracJourney = -2 * Mathf.Pow(fracJourney, 3.0f) + 3 * Mathf.Pow(fracJourney, 2.0f);//

        startPos.GetChild(0).transform.rotation = Quaternion.Euler(endEffectorRotation(startPos.transform, R_shoulderPos));
        endPos.GetChild(0).transform.rotation = Quaternion.Euler(endEffectorRotation(endPos.transform, R_shoulderPos));


        R_hand.transform.position = Vector3.Slerp(startPos.GetChild(0).position, endPos.GetChild(0).position, fracJourney);
        R_hand.transform.rotation = Quaternion.Slerp(startPos.GetChild(0).transform.rotation, endPos.GetChild(0).transform.rotation, fracJourney);


        float hand_height;
        if (step_1 == true || step_2 == true)
        {
            hand_height = 4.0f * journeyLength * 0.5f * (fracJourney - Mathf.Pow(fracJourney, 2.0f)); // y=B*[0.25-(x-0.5)^2]=B*(x-x^2), x range from (0-1) create a simple curve to move hand


        }

        else
        {
            hand_height = 0;
        }

        R_hand.transform.position = new Vector3(R_hand.transform.position.x, R_hand.transform.position.y + hand_height, R_hand.transform.position.z);

        ik.solver.rightHandEffector.position = R_hand.transform.position;//pos;
        ik.solver.rightHandEffector.rotation = R_hand.transform.rotation;

        //ik.solver.leftHandEffector.position = new Vector3(0.4f,1.0f,1.3f);
        //ik.solver.leftHandEffector.positionWeight = 1.0f;
        ik.solver.rightHandEffector.positionWeight = 1.0f;
        ik.solver.rightHandEffector.rotationWeight = 1.0f;


        //lookAt.solver.IKPosition = move_obj.position;
        //lookAt.solver.IKPositionWeight = 1.0f;
        //       lookAt.solver.Update();
        //if (Vector3.Distance(ik.solver.leftArmChain.nodes[1].transform.position, ik.solver.leftArmChain.nodes[2].transform.position)>0.1f)
            ik.solver.Update();


        //Vector2 Sdis = new Vector2(startPos.position.x, startPos.position.z);
        //Vector2 Edis = new Vector2(endPos.position.x, endPos.position.z);

        //journeyLength = Vector2.Distance(Sdis, Edis);
        //distJourney = (Time.time - startTime) * speed;
        //fracJourney = distJourney / journeyLength;
        //fracJourney = -2 * Mathf.Pow(fracJourney, 3.0f) + 3 * Mathf.Pow(fracJourney, 2.0f);//

        //startPos.GetChild(0).transform.rotation = Quaternion.Euler(endEffectorRotation(startPos.transform, R_shoulderPos));
        //endPos.GetChild(0).transform.rotation = Quaternion.Euler(endEffectorRotation(endPos.transform, R_shoulderPos));


        //R_hand.transform.position = Vector3.Slerp(startPos.GetChild(0).position, endPos.GetChild(0).position, fracJourney);
        //R_hand.transform.rotation = Quaternion.Slerp(startPos.GetChild(0).transform.rotation, endPos.GetChild(0).transform.rotation, fracJourney);

        //float hand_height = 2f * journeyLength * 0.5f * (fracJourney - Mathf.Pow(fracJourney, 2.0f)); // y=B*[0.25-(x-0.5)^2]=B*(x-x^2), x range from (0-1) create a simple curve to move hand

        //R_hand.transform.position = new Vector3(R_hand.transform.position.x, R_hand.transform.position.y + hand_height, R_hand.transform.position.z);

        //ik.solver.rightHandEffector.position = R_hand.transform.position;//pos;
        //ik.solver.rightHandEffector.rotation = R_hand.transform.rotation;

        ////ik.solver.leftHandEffector.position = new Vector3(0.4f,1.0f,1.3f);
        ////ik.solver.leftHandEffector.positionWeight = 1.0f;
        //ik.solver.rightHandEffector.positionWeight = 1.0f;
        //ik.solver.rightHandEffector.rotationWeight = 1.0f;


        ////lookAt.solver.IKPosition = move_obj.position;
        ////lookAt.solver.IKPositionWeight = 1.0f;
        ////       lookAt.solver.Update();
        //ik.solver.Update();
    }

    //public void CleanTeachAction()
    //{
    //    for (int i = 0; i < dict.Count; i++)
    //    {
    //        if (grabHolder[i] != null)
    //            Destroy(grabHolder[i]);
    //        if (releaseHolder[i] != null)
    //            Destroy(releaseHolder[i]);

    //    }
    //    if (RH_ease != null)
    //        Destroy(RH_ease);
    //}
}

