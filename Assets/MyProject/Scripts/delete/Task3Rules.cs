using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task3Rules : MonoBehaviour {

    public int level;
    public int objNumber;

    public Transform tableSurface;
    //public Transform agentPosition;

    public Dictionary<int, ObjectDict> dict = new Dictionary<int, ObjectDict>();
    List<int> usedMaterial = new List<int>(); //for unique color

    List<GameObject> prefabs = new List<GameObject>();

    List<GameObject> animal_giant = new List<GameObject>();
    List<GameObject> animal_big = new List<GameObject>();
    List<GameObject> animal_medium = new List<GameObject>();
    List<GameObject> animal_small = new List<GameObject>();

    List<GameObject> human_female = new List<GameObject>();
    List<GameObject> human_male = new List<GameObject>();

    List<Material> materials = new List<Material>();
    List<Vector3> targetPosition = new List<Vector3>();
    List<GameObject> objects = new List<GameObject>();

    public void Initialize(int lev)//, Transform T_surface, Transform A_pos)  // so the objects only has shape and color difference, no size difference
    {
        tableSurface = GameObject.Find("Surface").transform;

        level = lev;
        //agentPosition = A_pos;
        PrefabsLoad(); //load all the prefabs
        
        CreateObjects(level);
        TargetPosition();//generate target positions
        //CreateTask();
    }

    public void CreateTask()  //create all objects for the current task
    {
        Vector3 targHolder_scale = new Vector3(1.2f * 0.07f, 0.0001f, 1.2f * 0.07f);
        for (int i = 0; i < objNumber; i++)
        {
            GameObject oriObj = objects[i]; //orignal object has shape,color,transform
           
            oriObj.AddComponent<Rigidbody>();
            oriObj.GetComponent<Rigidbody>().isKinematic = true;

            GameObject targHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);//target holder to hold the object, its a plane
            targHolder.transform.position = targetPosition[i];
            targHolder.transform.localScale = targHolder_scale;
            //Material tmp = Resources.Load<Material>("c1"); //give a color to the holder
            //tmp.color = new Color(0.5f,1,1);
            //targHolder.GetComponent<Renderer>().material = tmp;

            GameObject grabHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);  //the position for agent to grab the object
            grabHolder.transform.position = new Vector3(oriObj.transform.position.x, oriObj.transform.position.y + 0.08f, oriObj.transform.position.z);
            grabHolder.transform.rotation = oriObj.transform.rotation;
            grabHolder.transform.localScale = oriObj.transform.localScale;
            Destroy(grabHolder.GetComponent<MeshRenderer>());


            GameObject releaseHolder = GameObject.CreatePrimitive(PrimitiveType.Cube); //the position for agent to release the object
            releaseHolder.transform.localScale = oriObj.transform.localScale;
            releaseHolder.transform.position = new Vector3(targetPosition[i].x, oriObj.transform.position.y+  0.08f, targetPosition[i].z);
            Destroy(releaseHolder.GetComponent<MeshRenderer>());

            //ObjDict od = new ObjDict((i+1).ToString(),materials.Count,prefabs.Count,oriObj,targObj,targHolder,false);//now fill the dictionary
            ObjectDict od = gameObject.AddComponent<ObjectDict>();
            //od.Initialize(oriObj, targHolder, grabHolder, releaseHolder, false);
            dict.Add(i, od);
        }
    }

    public void CreateObjects(int lev)
    {
         switch(lev)
        {
            case 1:

                objects.Add(Instantiate(animal_giant[Random.Range(0,animal_giant.Count)],InitPosition(),Quaternion.identity));
                objects.Add(Instantiate(animal_big[Random.Range(0, animal_big.Count)], InitPosition(), Quaternion.identity));
                objects.Add(Instantiate(animal_medium[Random.Range(0, animal_medium.Count)], InitPosition(),Quaternion.identity));
                objects.Add(Instantiate(animal_small[Random.Range(0, animal_small.Count)], InitPosition(), Quaternion.identity));

                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    Destroy(objects[i].GetComponent<Animator>());
                }

                objNumber = objects.Count;
                break;
            case 2:

                objects.Add(Instantiate(human_female[Random.Range(0, human_female.Count)], InitPosition(), Quaternion.identity));
                objects.Add(Instantiate(human_male[Random.Range(0, human_male.Count)], InitPosition(), Quaternion.identity));
                

                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    //Destroy(objects[i].GetComponent<Animator>());
                }

                objNumber = objects.Count;

                break;

            case 3:

                break;

            default:
                break;

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
            randPos = new Vector3(Random.Range(xmin, xmax), tableSurface.position.y , Random.Range(zmin, zmax));
            for (int i = 0; i < dict.Count; i++)
            {
                float dist = Vector3.Distance(randPos, dict[i].oriObject.transform.position);
                if (dist < dict[0].oriObject.transform.localScale.x * 1.5f) //distance between two objects
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
        //load level 1, for animals
        GameObject[] gb1 = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/animals_giant/");
        GameObject[] gb2 = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/animals_big/");
        GameObject[] gb3 = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/animals_medium/");
        GameObject[] gb4 = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/animals_small/");

        foreach (GameObject g in gb1)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            g.transform.rotation = Quaternion.Euler(0.0f,180.0f,0.0f);
            animal_giant.Add(g);
        }
        foreach (GameObject g in gb2)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            
            animal_big.Add(g);
        }
        foreach (GameObject g in gb3)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            g.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            animal_medium.Add(g);
        }
        foreach (GameObject g in gb4)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            g.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            animal_small.Add(g);
        }
        //load level 2, for humans
        GameObject[] gbhuman1= (GameObject[])Resources.LoadAll<GameObject>("Prefabs/human_female/");
        GameObject[] gbhuman2= (GameObject[])Resources.LoadAll<GameObject>("Prefabs/human_male/");
        foreach (GameObject g in gbhuman1)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            g.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            human_female.Add(g);
        }
        foreach (GameObject g in gbhuman2)
        {
            g.transform.localScale = 0.07f * g.transform.localScale;
            g.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            human_male.Add(g);
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
