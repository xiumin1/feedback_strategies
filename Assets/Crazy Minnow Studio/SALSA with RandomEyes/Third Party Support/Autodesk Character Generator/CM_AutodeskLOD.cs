using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;
using CrazyMinnow.SALSA.Autodesk;

namespace CrazyMinnow.SALSA.Autodesk
{
    [AddComponentMenu("Crazy Minnow Studio/SALSA/Addons/Autodesk Character Generator/CM_AutodeskLOD")]
    public class CM_AutodeskLOD : MonoBehaviour
    {
        public GameObject characterRoot;
        public CM_AutodeskSync autodeskSync; // CM_AutodeskSync script
        [Range(0, 3)]
        public int lod = 3; // 0=croud, 1=low, 2=medium, 3=high
        public List<GameObject> croud; // Link in the CrowdRes DDS and the "c_" objects
        public List<GameObject> low; // Link in the LowRes DDS and the "l_" objects
        public List<GameObject> mid; // Link in the MediumRes DDS and the "m_" objects
        public List<GameObject> high; // Link in the HighRes DDS and the "h_" objects

        private RandomEyes3D[] res; // Array of RandomEyes3D components
        private RandomEyes3D reCustomShapes; // The RandomEyes3D instance uses for custom shapes
        private int lastLod; // Track when the lod has been changed

        private void Start()
        {
            if (!characterRoot)
            {
                characterRoot = gameObject;
            }

            if (characterRoot)
            {
                // Get RandomEyes3D reference from the character root
                res = characterRoot.GetComponents<RandomEyes3D>();
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i].useCustomShapesOnly)
                        reCustomShapes = res[i];
                }
                // Get CM_AutoDeskSync reference from the character root
                autodeskSync = characterRoot.GetComponent<CM_AutodeskSync>();

                if (autodeskSync)
                {
                    GetCroudLOD();
                    GetLowLOD();
                    GetMidLOD();
                    GetHighLOD();
                }
            }
        }

        /// <summary>
        /// LOD switch
        /// </summary>
        private void Update()
        {
            if (characterRoot && autodeskSync)
            {
                if (lastLod != lod)
                {
                    switch (lod)
                    {
                        case 0: // Croud
                            if (croud.Count > 0)
                                SetLOD(0);
                            break;
                        case 1: // Low
                            if (low.Count > 0)
                                SetLOD(1);
                            break;
                        case 2: // Mid
                            if (mid.Count > 0)
                                SetLOD(2);
                            break;
                        case 3: // High
                            if (high.Count > 0)
                                SetLOD(3);
                            break;
                    }
                    lastLod = lod;
                }
            }
        }

        private void GetCroudLOD()
        {
            foreach (Transform child in characterRoot.transform)
            {
                if (child.name.ToLower().StartsWith("c_") || child.name.ToLower().Contains("crowdres"))
                {
                    croud.Add(child.gameObject);
                }
            }
        }

        private void GetLowLOD()
        {
            foreach (Transform child in characterRoot.transform)
            {
                if (child.name.ToLower().StartsWith("l_") || child.name.ToLower().Contains("lowres"))
                {
                    low.Add(child.gameObject);
                }
            }
        }

        private void GetMidLOD()
        {
            foreach (Transform child in characterRoot.transform)
            {
                if (child.name.ToLower().StartsWith("m_") || child.name.ToLower().Contains("midres"))
                {
                    mid.Add(child.gameObject);
                }
            }
        }

        private void GetHighLOD()
        {
            foreach (Transform child in characterRoot.transform)
            {
                if ((child.name.ToLower().StartsWith("h_") && !child.name.ToLower().Contains("dds")) || child.name.ToLower().Contains("highres"))
                {
                    high.Add(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Activate a specified LOD level and remap SkinnedMeshRenderer components
        /// </summary>
        /// <param name="lod"></param>
        public void SetLOD(int lod)
        {
            this.lod = lod;
            this.lastLod = this.lod;

            switch (lod)
            {
                case 0:
                    DisableAll();
                    for (int i = 0; i < croud.Count; i++)
                    {
                        croud[i].SetActive(true);
                        if (croud[i].name.ToLower().Contains("dds"))
                        {
                            autodeskSync.body = croud[i].GetComponent<SkinnedMeshRenderer>();
                            reCustomShapes.skinnedMeshRenderer = autodeskSync.body;
                        }
                        if (croud[i].name.ToLower().Contains("teethdown"))
                            autodeskSync.teeth = croud[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case 1:
                    DisableAll();
                    for (int i = 0; i < low.Count; i++)
                    {
                        low[i].SetActive(true);
                        if (low[i].name.ToLower().Contains("dds"))
                        {
                            autodeskSync.body = low[i].GetComponent<SkinnedMeshRenderer>();
                            reCustomShapes.skinnedMeshRenderer = autodeskSync.body;
                        }
                        if (low[i].name.ToLower().Contains("teethdown"))
                            autodeskSync.teeth = low[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case 2:
                    DisableAll();
                    for (int i = 0; i < mid.Count; i++)
                    {
                        mid[i].SetActive(true);
                        if (mid[i].name.ToLower().Contains("dds"))
                        {
                            autodeskSync.body = mid[i].GetComponent<SkinnedMeshRenderer>();
                            reCustomShapes.skinnedMeshRenderer = autodeskSync.body;
                        }
                        if (mid[i].name.ToLower().Contains("teethdown"))
                            autodeskSync.teeth = mid[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case 3:
                    DisableAll();
                    for (int i = 0; i < high.Count; i++)
                    {
                        high[i].SetActive(true);
                        if (high[i].name.ToLower().Contains("dds"))
                        {
                            autodeskSync.body = high[i].GetComponent<SkinnedMeshRenderer>();
                            reCustomShapes.skinnedMeshRenderer = autodeskSync.body;
                        }
                        if (high[i].name.ToLower().Contains("teethdown"))
                            autodeskSync.teeth = high[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
            }
        }

        /// <summary>
        /// Disable all res levels
        /// </summary>
        private void DisableAll()
        {
            int iterations = croud.Count;
            if (low.Count > iterations) iterations = low.Count;
            if (mid.Count > iterations) iterations = mid.Count;
            if (high.Count > iterations) iterations = high.Count;

            for (int i = 0; i < iterations; i++)
            {
                if (i < croud.Count) croud[i].SetActive(false);
                if (i < low.Count) low[i].SetActive(false);
                if (i < mid.Count) mid[i].SetActive(false);
                if (i < high.Count) high[i].SetActive(false);
            }
        }
    }
}