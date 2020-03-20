using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CrazyMinnow.SALSA.Autodesk
{
    [CustomEditor(typeof(CM_AutodeskSetup))]
    public class CM_AutodeskSetupEditor : Editor
    {
        private CM_AutodeskSetup autodeskSetup; // CM_AutodeskSetup reference

        public void OnEnable()
        {
            // Get reference
            autodeskSetup = target as CM_AutodeskSetup;

            // Run Setup
            autodeskSetup.Setup();

            // Remove setup component
            DestroyImmediate(autodeskSetup);
        }
    }
}