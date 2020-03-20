using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task4Rules : MonoBehaviour
{
    public int objNumber = 9;
    public int taskNumber_correct = 0; //
    public int activeNum;
    //public int level;
    public Transform tableSurface;
    //public Transform agentPosition;

    public Dictionary<int, ObjectDict> dict = new Dictionary<int, ObjectDict>();
    List<int> usedMaterial = new List<int>(); //for unique color, also used as sortID
    List<int> sortID = new List<int>();


    List<Material> matAnimals = new List<Material>();
    List<Material> matBuildings = new List<Material>();
    List<Material> matCountries = new List<Material>();
    List<Material> matPlanets = new List<Material>();
    List<Material> matFlags = new List<Material>();
    List<Material> matDemo = new List<Material>();

    List<List<Material>> mixedLevel = new List<List<Material>>(); //for mixed level task
    List<Material> matMixed = new List<Material>();

    public GameObject sourceCube;

    List<Vector3> targetPosition = new List<Vector3>();
    List<GameObject> objects = new List<GameObject>();

    List<GameObject> targHolder = new List<GameObject>();
    List<GameObject> grabHolder = new List<GameObject>();
    List<GameObject> releaseHolder = new List<GameObject>();

    public int[] matNum;//= new int[9] { 2, 4, 1, 6, 3, 7, 5, 9, 8 };
    public int[] matNum3 = new int[3] { 6, 1, 7 };
    public int[] matNum5 = new int[5] { 6, 1, 7, 5, 2 };
    public int[] matNum7 = new int[7] { 6, 1, 7, 5, 2, 3, 0 };
    public int[] matNum9 = new int[9] { 6, 1, 7, 5, 2, 3, 0, 4, 8 };

    public void Initialize()//, Transform T_surface, Transform A_pos)  // so the objects only has shape and color difference, no size difference
    {
        
        tableSurface = GameObject.Find("Surface").transform;
        //agentPosition = A_pos;
        PrefabsLoad(); //load all the prefabs
        MaterialsLoad();
        CreateObjects();
        TargetPosition();//generate target positions
        CreateFrame();


        CreateTask();
    }


    public void CreateTask()
    {
    
        //level = lev - 1; // levle from 0->3==> lev 1->4

        //for (int i = 0; i < dict.Count; i++)
        //{
        //    dict[i].oriObject.SetActive(false);
        //}

        if (dict != null)
        {
            for (int i= 0;i< dict.Count;i++)
            {
                Destroy(dict[i]);
            }
            dict.Clear();
            //sortID.Clear(); //clear to store the new values
            usedMaterial.Clear();
        }

        for (int i = 0; i < objNumber; i++)
        {
            //usedMaterial.Add(matNum[i]);
        }
        //for (int i = 0; i < objNumber; i++)
        //{
        //    int index = UniqueRandomInt(0, mixedLevel[level].Count);
        //    usedMaterial.Add(index);
        //    //objects[i].transform.Find("Top").GetComponent<Renderer>().material = mat[index];

        //    //SortIDnum(lev, usedMaterial);
        //}

       // SortIndex(usedMaterial); //here the material pictures has been sorted in an increased sortID order

        for (int j = 0; j < objNumber; j++)
        {
            //objects[j].gameObject.transform.position = new Vector3(0f,0f,0f);
            //objects[j].transform.GetChild(5).GetComponent<Renderer>().material = mat[usedMaterial[j]];
            for (int i = 0; i < 6; i++)
            {
                objects[j].transform.GetChild(i).GetComponent<Renderer>().material = matFlags[j];//mixedLevel[level][usedMaterial[j]];  //here only attach material to the top face of the cube 
            }
         
            //SortIDnum(level, usedMaterial);
        }

        for (int i = 0; i < objNumber; i++)
        {
            //objects[i].transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

            //targHolder[i].transform.position = targetPosition[i];

            //grabHolder[i].transform.position = new Vector3(objects[i].transform.position.x, objects[i].transform.position.y + 0.08f, objects[i].transform.position.z);
            //grabHolder[i].transform.rotation = objects[i].transform.rotation;

            //releaseHolder[i].transform.position = new Vector3(targetPosition[i].x, objects[i].transform.position.y + 0.08f, targetPosition[i].z);

            ObjectDict od = gameObject.AddComponent<ObjectDict>();
            od.Initialize(i, objects[i], targHolder[i], grabHolder[i], releaseHolder[i], false);
            dict.Add(i, od);
        }

        for (int i = 0; i < objNumber; i++)
        {
           
            dict[i].oriObject.SetActive(false);
            dict[i].targHolder.SetActive(false);
        }
    }
    public void ShowDemos()
    {
        
        activeNum = 4;
        for (int i = 0; i < 4; i++)
        {
            dict[i].oriObject.transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

            dict[i].targHolder.transform.position = targetPosition[i];

            dict[i].grabHolder.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y + 0.08f, dict[i].oriObject.transform.position.z);
            dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;

            dict[i].releaseHolder.transform.position = new Vector3(targetPosition[i].x, dict[i].oriObject.transform.position.y + 0.08f, targetPosition[i].z);

            for (int j = 0; j < 6; j++)
            {
                objects[i].transform.GetChild(j).GetComponent<Renderer>().material = matDemo[i];//matBuildings[i];//mixedLevel[level][usedMaterial[i]];  //here only attach material to the top face of the cube 
            }

            dict[i].oriObject.SetActive(true);
            dict[i].targHolder.SetActive(true);
        }
    }
    public void CreateNewTask(int cubeNum) //to show up the desired cubes and cube holder on the table
    {
        activeNum = cubeNum;
        //int k;
        
        switch (cubeNum)
        {
            case 3:
                matNum = new int[cubeNum];
                matNum =SortIndex(matNum3);
                for (int i = 0; i < cubeNum; i++)
                {
                    //k = matNum3[i];
                    dict[i].oriObject.transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

                    dict[i].targHolder.transform.position = targetPosition[i];

                    dict[i].grabHolder.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y + 0.08f, dict[i].oriObject.transform.position.z);
                    dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;
                    dict[i].grabHolder.transform.parent = dict[i].oriObject.transform;

                    dict[i].releaseHolder.transform.position = new Vector3(targetPosition[i].x, dict[i].oriObject.transform.position.y + 0.08f, targetPosition[i].z);

                    dict[i].oriObject.SetActive(true);
                    dict[i].targHolder.SetActive(true);
                }


                break;
            case 5:
                matNum = new int[cubeNum];
                matNum =SortIndex(matNum5);
                for (int i = 0; i < cubeNum; i++)
                {
                    //i = matNum5[i];
                    dict[i].oriObject.transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

                    dict[i].targHolder.transform.position = targetPosition[i];

                    dict[i].grabHolder.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y + 0.08f, dict[i].oriObject.transform.position.z);
                    dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;
                    dict[i].grabHolder.transform.parent = dict[i].oriObject.transform;

                    dict[i].releaseHolder.transform.position = new Vector3(targetPosition[i].x, dict[i].oriObject.transform.position.y + 0.08f, targetPosition[i].z);

                    dict[i].oriObject.SetActive(true);
                    dict[i].targHolder.SetActive(true);
                }
                break;
            case 7:
                matNum = new int[cubeNum];
                matNum =SortIndex(matNum7);
                for (int i = 0; i < cubeNum; i++)
                {
                    //i = matNum7[i];
                    dict[i].oriObject.transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

                    dict[i].targHolder.transform.position = targetPosition[i];

                    dict[i].grabHolder.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y + 0.08f, dict[i].oriObject.transform.position.z);
                    dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;
                    dict[i].grabHolder.transform.parent = dict[i].oriObject.transform;

                    dict[i].releaseHolder.transform.position = new Vector3(targetPosition[i].x, dict[i].oriObject.transform.position.y + 0.08f, targetPosition[i].z);

                    dict[i].oriObject.SetActive(true);
                    dict[i].targHolder.SetActive(true);
                }
                break;
            case 9:
                matNum = new int[cubeNum];
                matNum =SortIndex(matNum9);
                for (int i = 0; i < cubeNum; i++)
                {
                    //i = matNum9[i];
                    dict[i].oriObject.transform.position = InitPosition();//  new Vector3(InitPosition().x,InitPosition().y,InitPosition().z);

                    dict[i].targHolder.transform.position = targetPosition[i];

                    dict[i].grabHolder.transform.position = new Vector3(dict[i].oriObject.transform.position.x, dict[i].oriObject.transform.position.y + 0.08f, dict[i].oriObject.transform.position.z);
                    dict[i].grabHolder.transform.rotation = dict[i].oriObject.transform.rotation;
                    dict[i].grabHolder.transform.parent = dict[i].oriObject.transform;

                    dict[i].releaseHolder.transform.position = new Vector3(targetPosition[i].x, dict[i].oriObject.transform.position.y + 0.08f, targetPosition[i].z);

                    dict[i].oriObject.SetActive(true);
                    dict[i].targHolder.SetActive(true);
                }
                break;

        }

        
        for (int j = 0; j < matNum.Length ; j++)
        {
            //objects[j].gameObject.transform.position = new Vector3(0f,0f,0f);
            //objects[j].transform.GetChild(5).GetComponent<Renderer>().material = mat[usedMaterial[j]];
            for (int i = 0; i < 6; i++)
            {
                objects[j].transform.GetChild(i).GetComponent<Renderer>().material = matFlags[matNum[j]];//mixedLevel[level][usedMaterial[j]];  //here only attach material to the top face of the cube 
            }

            //SortIDnum(level, usedMaterial);
        }

        taskNumber_correct += 1;
    }
    public void CreateFrame()  //create all objects for the current task
    {      
        Vector3 targHolder_scale = new Vector3(1.2f * 0.07f, 0.0001f, 1.2f * 0.07f);
        for (int i = 0; i < objNumber; i++)
        {
            

            objects[i].AddComponent<Rigidbody>();
            //objects[i].AddComponent<DropSound>();
            //objects[i].GetComponent<Rigidbody>().isKinematic = true;
            // add the OVRGrabbable script to the grab object
            Collider[] col = new Collider[1];
            col[0] = objects[i].GetComponent<Collider>();
            OVRGrabbableExtended OED = objects[i].AddComponent<OVRGrabbableExtended>();
            OED.Init(col);
            OED.enabled = true;
          

            targHolder.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));//target holder to hold the object, its a plane
            
            targHolder[i].transform.localScale = targHolder_scale;
            targHolder[i].name = "plate" + i.ToString();
            targHolder[i].GetComponent<Renderer>().material.SetColor("white", Color.white);
            
            //Material tmp = Resources.Load<Material>("c1"); //give a color to the holder
            //tmp.color = new Color(0.5f,1,1);
            //targHolder.GetComponent<Renderer>().material = tmp;

            grabHolder.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));  //the position for agent to grab the object           
            grabHolder[i].transform.localScale = objects[i].transform.localScale;
            //grabHolder[i].transform.parent = objects[i].transform;
            Destroy(grabHolder[i].GetComponent<MeshRenderer>());
            if (grabHolder[i].GetComponent<BoxCollider>() != null)
                Destroy(grabHolder[i].GetComponent<BoxCollider>());
            

            releaseHolder.Add(GameObject.CreatePrimitive(PrimitiveType.Cube)); //the position for agent to release the object
            releaseHolder[i].transform.localScale = objects[i].transform.localScale;
              //releaseHolder[i].transform.parent = targHolder[i].transform;
            Destroy(releaseHolder[i].GetComponent<MeshRenderer>());
            if(releaseHolder[i].GetComponent<BoxCollider>()!=null)
                Destroy(releaseHolder[i].GetComponent<BoxCollider>());
            //ObjDict od = new ObjDict((i+1).ToString(),materials.Count,prefabs.Count,oriObj,targObj,targHolder,false);//now fill the dictionary

            objects[i].name= "object" + i.ToString();           
            grabHolder[i].name = "grabHolder" + i.ToString();
            releaseHolder[i].name = "releaseHolder" + i.ToString();
        }
    }
  

    public void CreateObjects( )
    {
        for (int i = 0; i < objNumber; i++)
        {
            objects.Add(Instantiate(sourceCube));
            //objects.Add(Instantiate(sourceCube, InitPosition(), Quaternion.identity));
        }
    }

    public Vector3 InitPosition()  //initiate a position for each generated object in a range on the table
    {
        float length = 0.4f;
        float zmin = tableSurface.position.z;
        float zmax = tableSurface.position.z + 0.3f;
        float xmin = tableSurface.position.x - 0.5f * length;
        float xmax = tableSurface.position.x + 0.5f * length;

        Vector3 randPos = Vector3.zero;
        bool pFound = false;
        while (pFound == false) //find the init position that does not collide with other objects
        {
            int ct = 0;
            randPos = new Vector3(Random.Range(xmin, xmax), tableSurface.position.y+0.035f, Random.Range(zmin, zmax));
            for (int i = 0; i < objects.Count; i++)  //change dict[i]->objects
            {
                float dist = Vector3.Distance(randPos, objects[i].transform.position); //change dict[i].oriObject->objects[i]
                if (dist < objects[i].transform.localScale.x * 1.5f) //distance between two objects
                {
                    ct += 1;
                }
            }
            if (ct < 1) pFound = true;
        }
        return randPos;
    }

    public void TargetPosition()
    {
        float length = 1.0f;
        float tz = tableSurface.position.z - 0.1f;//????? 
        float tx = tableSurface.position.x - 0.5f * length;
        float ty = tableSurface.position.y + 0.0001f;
        for (int i = 0; i < objNumber; i++)
        {
            tx += length / objNumber;
            targetPosition.Add(new Vector3(tx, ty, tz));  //create position in a fix distance to put those objects
        }
    }

    public void PrefabsLoad()  // add more codes to load all the prefabs, load all the prefabs to list prefabs
    {
        sourceCube = Resources.Load<GameObject>("Prefabs/ObjectPrefab/Cube");
    }
    public void MaterialsLoad()
    {
        Material[] m1= (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/Animals/");
        Material[] m2 = (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/Buildings/");
        Material[] m3 = (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/Countries/");
        Material[] m4 = (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/Planets/");
        Material[] m5= (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/FlagCountries/Group1/");
        Material[] m6 = (Material[])Resources.LoadAll<Material>("Materials/CubeMaterial/DemoCountries/");


        foreach (Material m in m1)
        {
            matAnimals.Add(m);
        }

        foreach (Material m in m2)
        {
            matBuildings.Add(m);
        }

        foreach (Material m in m3)
        {
            matCountries.Add(m);
        }

        foreach (Material m in m4)
        {
            matPlanets.Add(m);
        }

        foreach (Material m in m5)
        {
            matFlags.Add(m);
        }

        foreach (Material m in m6)
        {
            matDemo.Add(m);
        }

        mixedLevel.Add(matPlanets);
        mixedLevel.Add(matCountries);
        mixedLevel.Add(matBuildings);
        mixedLevel.Add(matAnimals);
    }

    public void CleanTask()  //clean the objects on the table
    {

        for (int i = 0; i < dict.Count; i++)
        {
            Destroy(dict[i].oriObject);
            Destroy(dict[i].targHolder);
            Destroy(dict[i].grabHolder);
            Destroy(dict[i].releaseHolder);
            Destroy(dict[i]);
        }
    }

    public int UniqueRandomInt(int min, int max)  //to get the unrepeated value in a range(min, max)
    {
        int val = Random.Range(min, max);
        while (usedMaterial.Contains(val))
        {
            val = Random.Range(min, max);
        }
        return val;
    }

    public int[] SortIndex(int[] input) //bubble sort from min->max
    {
        int temp=0;
        for (int i = 0; i < input.Length -1; i++)
        {
            for (int j = 0; j < input.Length-1; j++)
            {
                if(input[j]>input[j+1])
                {
                    temp = input[j + 1];
                    input[j + 1] = input[j];
                    input[j] = temp;

                }
            }
        }
        return input;
    }

    

    public void CreateMixedLevelObjects(int num) //for level 5 or 6, not finished yet???????????????????????????
    {
        mixedLevel.Add(matPlanets);
        mixedLevel.Add(matCountries);
        mixedLevel.Add(matBuildings);
        mixedLevel.Add(matAnimals);

        for (int i = 0; i < num; i++)
        {
            int mixed=Random.Range(0,mixedLevel.Count);
            int mixedItem = Random.Range(0,mixedLevel[mixed].Count);
            int mixedID = (int)Mathf.Pow(10f, mixed * 1.0f) + mixedItem; //calculate the sortID for mixed condition

            matMixed.Add(mixedLevel[mixed][mixedItem]);
            sortID.Add(mixedID);
        }

    }

    public void SortIDnum(int leve, List<int> sourceList) //for the mixture sorting
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            int num = sourceList[i] + (int)Mathf.Pow(10f, leve*1.0f);
            sortID.Add(num);
        }
    }

}
