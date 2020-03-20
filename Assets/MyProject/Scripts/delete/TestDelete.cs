using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelete : MonoBehaviour {

    // Use this for initialization

    public Task task;

    public Dictionary<int, ObjDict> dict_move = new Dictionary<int, ObjDict>();
    List<int> usedObject = new List<int>();
    List<int> usedTarget = new List<int>();

    int objNum; //which object is moving in the dictionary
    int targNum;//which target holder is moving
    Transform obj; //transform of the current moving object
    int count;//count how many objects go to its right position


    void Start()
    {

        task = gameObject.AddComponent<Task>();
        task.Initialize(5, 3);//, cc.transform);
        task.CreateTask();
        dict_move = task.dict;

    }

    

    public int UniqueRandomInt_Object(int min, int max)  //to get the unrepeated value in a range(min, max)
    {
        int val = Random.Range(min, max);
        while (usedObject.Contains(val))
        {
            val = Random.Range(min, max);
        }
        return val;
    }

    public int UniqueRandomInt_Target(int min, int max)  //to get the unrepeated value in a range(min, max)
    {
        int val = Random.Range(min, max);
        while (usedTarget.Contains(val))
        {
            val = Random.Range(min, max);
        }
        return val;
    }

    void Update()
    {

        count = 0;
        int targChosen = Random.Range(0, dict_move.Count);
        for (int i = 0; i < dict_move.Count; i++)
        {

            objNum = UniqueRandomInt_Object(0, dict_move.Count);
            targNum = UniqueRandomInt_Target(0, dict_move.Count);
            //obj = dict_move[objNum].oriObject.transform;
            //obj.position = Vector3.Lerp(obj.position, dict_move[targNum].targHolder.transform.position,Time.deltaTime*10.0f);

            Vector3 pos = dict_move[targNum].targHolder.transform.position;
            dict_move[objNum].oriObject.transform.position = new Vector3(pos.x, pos.y + 0.07f * 0.5f, pos.z);

            if (objNum == targNum)
            {
                count += 1;
            }
        }
        Debug.Log("count= " + count);
    }

}
