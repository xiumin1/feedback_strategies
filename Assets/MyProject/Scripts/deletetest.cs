using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class deletetest : MonoBehaviour
{

    // Use this for initialization
    public UnityEvent suggestEvent;

     
    void Start()
    {
        
        Debug.Log("start");

        //if (suggestEvent == null)
        //{
        //    suggestEvent = new UnityEvent();
        //}
        //else
        //{
        //    suggestEvent.AddListener(test);  //add the control function name here

        //}

        //suggestEvent.Invoke();
    }

    void test()
    {
        Debug.Log("test() inside");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && suggestEvent != null)
        {
            //suggestEvent.Invoke();
            DelegateHandler.suggestDelegate += test;
        }
    }
}

//using UnityEngine;
//using UnityEngine.Events;

//[System.Serializable]
//public class MyIntEvent : UnityEvent<int>
//{
//}

//public class deletetest : MonoBehaviour
//{
//    public MyIntEvent m_MyEvent;

//    void Start()
//    {
//        if (m_MyEvent == null)
//            m_MyEvent = new MyIntEvent();

//        m_MyEvent.AddListener(Ping);
//    }

//    void Update()
//    {
//        if (Input.anyKeyDown && m_MyEvent != null)
//        {
//            m_MyEvent.Invoke(5);
//        }
//    }

//    void Ping(int i)
//    {
//        Debug.Log("Ping" + i);
//    }
//}