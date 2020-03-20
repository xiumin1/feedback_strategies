using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public int objNumber;  // the object numbers
   // public int objMaterialNumber; // the material numbers
    public int objPrefabNumber;  //the prefab numbers
    public Transform tableSurface;
    //public Transform agentPosition;

    public Dictionary<int, ObjDict> dict = new Dictionary<int, ObjDict>();  
    List<int> usedMaterial = new List<int>(); //for unique color

    List<GameObject> prefabs = new List<GameObject>();
    List<Material> materials = new List<Material>();
    List<Vector3> targetPosition = new List<Vector3>();

    //need to know how many objects, how many shapes, where is the table surface, where is the agent
    public void Initialize(int num, int shape)//, Transform T_surface, Transform A_pos)  // so the objects only has shape and color difference, no size difference
    {
        tableSurface = GameObject.Find("Surface").transform;

        objNumber = num;
       // objMaterialNumber = material;
        objPrefabNumber = shape;
        //tableSurface = T_surface;

        //agentPosition = A_pos;
        PrefabsLoad(); //load all the prefabs
        MaterialsLoad(); //load all the prefabs
        TargetPosition();//generate target positions
        //CreateTask();
    }

    public void CreateTask()  //create all objects for the current task
    {
        Vector3 targHolder_scale = new Vector3(1.2f*0.07f, 0.0001f, 1.2f *0.07f);
        for (int i = 0; i < objNumber; i++)
        {
            GameObject oriObj = CreateObject(); //orignal object has shape,color,transform
                     
            //oriObj.AddComponent<BoxCollider>();
            oriObj.AddComponent<Rigidbody>();
            oriObj.GetComponent<Rigidbody>().isKinematic = true;
            GameObject targHolder =GameObject.CreatePrimitive(PrimitiveType.Cube);//target holder to hold the object, its a plane
            targHolder.transform.position = targetPosition[i];
            targHolder.transform.localScale= targHolder_scale;
            //Material tmp = Resources.Load<Material>("c1"); //give a color to the holder
            //tmp.color = new Color(0.5f,1,1);
            //targHolder.GetComponent<Renderer>().material = tmp;

            GameObject grabHolder = Instantiate(oriObj);  //the position for agent to grab the object
            Destroy(grabHolder.GetComponent<BoxCollider>());
            Destroy(grabHolder.GetComponent<MeshRenderer>());

            GameObject releaseHolder = Instantiate(oriObj); //the position for agent to release the object
            releaseHolder.transform.position = new Vector3(targetPosition[i].x, oriObj.transform.position.y, targetPosition[i].z);
            Destroy(releaseHolder.GetComponent<BoxCollider>());
            Destroy(releaseHolder.GetComponent<MeshRenderer>());


            //ObjDict od = new ObjDict((i+1).ToString(),materials.Count,prefabs.Count,oriObj,targObj,targHolder,false);//now fill the dictionary
            ObjDict od = gameObject.AddComponent<ObjDict>();
            od.Initialize((i + 1).ToString(), materials.Count, prefabs.Count, oriObj, targHolder, grabHolder, releaseHolder, false);
            dict.Add(i, od);     
        }
    }

    public Vector3 InitPosition()  //initiate a position for each generated object in a range on the table
    {
        float length = 0.4f;
        float zmin = tableSurface.position.z;
        float zmax = tableSurface.position.z + 0.3f;
        float xmin = tableSurface.position.x - 0.5f * length;
        float xmax = tableSurface.position.x + 0.5f * length;

        Vector3 randPos=Vector3.zero;
        bool pFound = false;
        while (pFound == false) //find the init position that does not collide with other objects
        {
            int ct = 0;
            randPos = new Vector3(Random.Range(xmin, xmax), tableSurface.position.y+0.5f*0.07f, Random.Range(zmin, zmax) );
            for (int i = 0; i < dict.Count; i++)
            {
                float dist = Vector3.Distance(randPos, dict[i].oriObject.transform.position);
                if (dist<dict[0].oriObject.transform.localScale.x*1.5f) //distance between two objects
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
        float length = 0.8f;
        float tz = tableSurface.position.z -0.1f;//????? 
        float tx = tableSurface.position.x - 0.5f * length;
        float ty = tableSurface.position.y+0.0001f;
        for (int i = 0; i < objNumber; i++)
        {
            tx+= length / objNumber;
            targetPosition.Add(new Vector3(tx,ty,tz));  //create position in a fix distance to put those objects
        }
    }

    public GameObject CreateObject() //create one object
    {
        int materialIndex= 0;
        int prefabIndex = Random.Range(0,objPrefabNumber);
        
        GameObject myObject=Instantiate(prefabs[prefabIndex]);  //create object and select shape
        if (objPrefabNumber <= 1)  //generate unique color for each object
        {
            materialIndex = UniqueRandomInt(0, materials.Count);
            usedMaterial.Add(materialIndex);
            
        }
        else  //generate random color but not same color if they have same shape
        {
            bool mFound = false;
            while( mFound==false)
            {
                int ct = 0;
                materialIndex = Random.Range(0, materials.Count);
                for (int i = 0; i < dict.Count; i++)
                {
                    if (dict[i].prefabIndex == prefabIndex && dict[i].materialIndex==materialIndex)
                    {
                        ct +=1;
                    }
                }
                if (ct < 1) mFound = true;               
            }
        }
        myObject.GetComponent<Renderer>().material = materials[materialIndex];  //select color

        myObject.transform.localScale = 0.07f*myObject.transform.localScale;//Vector3(0.07f,0.07f,0.07f); give scale to each object
        myObject.transform.position = InitPosition();
        myObject.transform.rotation = Quaternion.identity;
        return myObject;
    }

    public void PrefabsLoad()  //load all the prefabs to list prefabs
    {
        GameObject[] gb = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/ObjectPrefab/");
        foreach (GameObject g in gb)
        {
            prefabs.Add(g);
        }
    }

    public void MaterialsLoad()  //load all the materials to list materials
    {
        Material[] mt = (Material[]) Resources.LoadAll<Material>("Materials/ObjectColor/");
        foreach (Material m in mt)
        {
            materials.Add(m);
        }
    }

    public void CleanTask()  //clean the objects on the table
    {
        for (int i = 0; i < dict.Count; i++)
        {
            Destroy(dict[i].oriObject);
            Destroy(dict[i].targHolder);
            Destroy(dict[i].grabHolder);
            Destroy(dict[i].releaseHolder);
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
}
