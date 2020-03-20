using UnityEngine;
using System.Collections;
using CrazyMinnow.SALSA.Autodesk;

namespace CrazyMinnow.SALSA.Autodesk
{
    public class LODTester : MonoBehaviour
    {
        public CM_AutodeskLOD autodeskLOD;
        public int lod = 3;
        public bool set;

        void Update()
        {
            if (set)
            {
                set = false;
                autodeskLOD.SetLOD(lod);
            }
        }
    }
}