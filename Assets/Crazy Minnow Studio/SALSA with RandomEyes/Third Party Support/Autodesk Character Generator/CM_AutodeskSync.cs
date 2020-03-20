using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.Autodesk
{
	/// <summary>
	/// This script acts as a proxy between SALSA with RandomEyes and Autodesk Character Generator characters,
	/// and allows users to link SALSA with RandomEyes to Autodesk characters without any model
	/// modifications.
	/// 
	/// Good default inspector values
	/// Salsa3D
	/// 	Trigger values will depend on your recordings
	/// 	Blend Speed: 10
	/// 	Range of Motion: 75
	/// RandomEyes3D
	/// 	Range of Motion: 60
	/// </summary>
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[AddComponentMenu("Crazy Minnow Studio/SALSA/Addons/Autodesk Character Generator/CM_AutodeskSync")]
	public class CM_AutodeskSync : MonoBehaviour 
	{
		public Salsa3D salsa3D; // Salsa3D mouth component
		public RandomEyes3D randomEyes3D; // RandomEyes3D eye componet
		public SkinnedMeshRenderer body; // Body SkinnedMeshRenderer (H_DDS_???Res)
		public SkinnedMeshRenderer prevBody; // Tracks body property changes
		public SkinnedMeshRenderer teeth; // Teeth SkinnedMeshRenderer (m_TeethDown)
		public string teethShapeName = "MouthOpen"; // Used in search for MouthOpen shape
		public int teethShapeIndex = -1; // Teeth MouthOpen shape index
		public string leftEyeName = "LeftEye"; // Used in search for left eye bone
		public GameObject leftEyeBone; // Left eye bone
		public string rightEyeName = "RightEye"; // Used in search for right eye bone
		public GameObject rightEyeBone; // Right eye bone
		public bool syncExpressions = false; // Optionally sync expressions
		public Vector3 eyeCenter = new Vector3(0, 270, 0);
		public List<CM_ShapeGroup> saySmall = new List<CM_ShapeGroup>(); // saySmall shape group
		public List<CM_ShapeGroup> sayMedium = new List<CM_ShapeGroup>(); // sayMedium shape group
		public List<CM_ShapeGroup> sayLarge = new List<CM_ShapeGroup>(); // sayLarge shape group
		public string[] shapeNames; // Shape name string array for name picker popups
		public bool initialize = true; // Initialize once

		private Transform[] children; // For searching through child objects during initialization
		private int tmpShapeIndex = -1; // Shape index for whichever shape is currently being managed
		private string srcShapeName = ""; // Shape name for whichever shape is currenly being managed
		private float srcShapeWeight = 0f; // Shape weight for whichever shape is currently being managed
		private float eyeSensativity = 500f; // Eye movement reduction from shape value to bone transform value
		private string blinkLeftShape = "LeyeClose"; // Blink_Left BlendShape name
		private string blinkRightShape = "ReyeClose"; // Blink_Right BlendShape name
		private int leftEyelidIndex = -1; // Blendshape index for the left eyelid
		private int rightEyelidIndex = -1; // Blendshape index for the right eyelid
		private float blinkWeight; // Blink weight from the shapeNull is applied to the body Blink_Left and Blink_Right BlendShapes
		private float vertical; // Vertical eye bone movement amount
		private float horizontal; // Horizontal eye bone movement amount
		private bool lockShapes; // Used to allow access to shape group shapes when SALSA is not talking

		/// <summary>
		/// Reset the component to default values
		/// </summary>
		void Reset()
		{
			initialize = true;
			GetSalsa3D();
			GetRandomEyes3D();
            GetBody();
            GetTeeth();
			GetEyeBones();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();
			SetDefaultSmall();
			SetDefaultMedium();
			SetDefaultLarge();
		}

        /// <summary>
        /// Initial setup
        /// </summary>
		void Start()
		{
			// Initialize
			GetSalsa3D();
			GetRandomEyes3D();
            GetBody();
            GetTeeth();
			GetEyeBones();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();
		}

        /// <summary>
        /// Perform the blendshape changes in LateUpdate for mechanim compatibility
        /// </summary>
		void LateUpdate() 
		{
			// Toggle shape lock to provide access to shape group shapes when SALSA is not talking
			if (salsa3D)
			{
				if (salsa3D.sayAmount.saySmall == 0f && salsa3D.sayAmount.sayMedium == 0f && salsa3D.sayAmount.sayLarge == 0f)
				{
					lockShapes = false;
				}
				else
				{
					lockShapes = true;
				}
			}

			if (salsa3D && body)
			{
				if (lockShapes)
				{
					for (int i = 0; i < saySmall.Count; i++)
					{
						body.SetBlendShapeWeight(
							saySmall[i].shapeIndex, ((saySmall[i].percentage / 100) * salsa3D.sayAmount.saySmall));
					}

					for (int i = 0; i < sayMedium.Count; i++)
					{
						body.SetBlendShapeWeight(
							sayMedium[i].shapeIndex, ((sayMedium[i].percentage / 100) * salsa3D.sayAmount.sayMedium));
					}

					for (int i = 0; i < sayLarge.Count; i++)
					{
						body.SetBlendShapeWeight(
							sayLarge[i].shapeIndex, ((sayLarge[i].percentage / 100) * salsa3D.sayAmount.sayLarge));
					}
				}

				if (teeth)
				{
					if (teethShapeIndex == -1)
					{
						teethShapeIndex = ShapeSearch(teeth, teethShapeName);
					}
					if (teethShapeIndex != -1)
					{
						teeth.SetBlendShapeWeight(teethShapeIndex, GetMaxMouthValue());
					}

					if (syncExpressions)
					{
						// Sync custom shapes
						for (int i = 0; i < body.sharedMesh.blendShapeCount; i++)
						{
							srcShapeWeight = body.GetBlendShapeWeight(i);
                            if (srcShapeWeight > 0)
                            {
                                // Get custom shapes source shape name
                                srcShapeName = body.sharedMesh.GetBlendShapeName(i);
                                // Try to split the shape name to remove the prefix
								if (srcShapeName.Contains(".")) 
									srcShapeName = srcShapeName.Split(new char[] { '.' })[1];
                                // Get shape index from teeth SkinnedMeshRender that ends with the same shape name
                                tmpShapeIndex = ShapeSearch(teeth, srcShapeName);
                                if (tmpShapeIndex > -1)
                                {
                                    teeth.SetBlendShapeWeight(tmpShapeIndex, srcShapeWeight);
                                }
                            }
						}
					}
				}
			}

			// Get Blink weight
			if (randomEyes3D)
			{
				blinkWeight = randomEyes3D.lookAmount.blink;

				// Perform blink action on eye lids
				if (body)
				{
					if (leftEyelidIndex == -1) leftEyelidIndex = ShapeSearch(body, blinkLeftShape);
					if (rightEyelidIndex == -1) rightEyelidIndex = ShapeSearch(body, blinkRightShape);
					
					if (leftEyelidIndex != -1) body.SetBlendShapeWeight(leftEyelidIndex, blinkWeight);
					if (rightEyelidIndex != -1) body.SetBlendShapeWeight(rightEyelidIndex, blinkWeight);
				}

				// Look
				if (leftEyeBone || rightEyeBone)
				{
                    // Apply eye movement weight direction variables
                    if (randomEyes3D.lookAmount.lookUp == 0
                        && randomEyes3D.lookAmount.lookDown == 0
                        && randomEyes3D.lookAmount.lookLeft == 0
                        && randomEyes3D.lookAmount.lookRight == 0)
                    {
                        vertical = (randomEyes3D.lookAmount.lookUp / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.y;
                        horizontal = (randomEyes3D.lookAmount.lookRight / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.x;
                    }

					if (randomEyes3D.lookAmount.lookUp > 0) 
						vertical = (randomEyes3D.lookAmount.lookUp / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.y;
					if (randomEyes3D.lookAmount.lookDown > 0) 
						vertical = -(randomEyes3D.lookAmount.lookDown / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.y;
					if (randomEyes3D.lookAmount.lookLeft > 0) 
						horizontal = -(randomEyes3D.lookAmount.lookLeft / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.x;
					if (randomEyes3D.lookAmount.lookRight > 0) 
						horizontal = (randomEyes3D.lookAmount.lookRight / eyeSensativity) * randomEyes3D.rangeOfMotion + eyeCenter.x;
					
					// Set eye bone rotations
					if (leftEyeBone) leftEyeBone.transform.localRotation = Quaternion.Euler(0, vertical, horizontal);
					if (rightEyeBone) rightEyeBone.transform.localRotation = Quaternion.Euler(0, vertical, horizontal);
				}
			}		
		}

		/// <summary>
		/// Call this when initializing characters at runtime
		/// </summary>
		public void Initialize()
		{
			Reset();
		}

		/// <summary>
		/// The the maximum SALSA open mouth value for the teeth BlendShape
		/// </summary>
		/// <returns>The max mouth value.</returns>
		private float GetMaxMouthValue()
		{
			float max = -1f;
			if (salsa3D)
			{
				if (salsa3D.sayAmount.saySmall > max) max = salsa3D.sayAmount.saySmall;
				if (salsa3D.sayAmount.sayMedium > max) max = salsa3D.sayAmount.sayMedium;
				if (salsa3D.sayAmount.sayLarge > max) max = salsa3D.sayAmount.sayLarge;
			}
			return max;
		}

		/// <summary>
		/// Get the Salsa3D component
		/// </summary>
		public void GetSalsa3D()
		{
			if (!salsa3D) salsa3D = GetComponent<Salsa3D>();
		}
		
		/// <summary>
		/// Get the RandomEyes3D component
		/// </summary>
		public void GetRandomEyes3D()
		{
			if (!randomEyes3D) randomEyes3D = GetComponent<RandomEyes3D>();
		}

        /// <summary>
        /// Find the Body child object SkinnedMeshRenderer
        /// </summary>
        public void GetBody()
        {
            SkinnedMeshRenderer[] smrs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrs.Length > 0)
            {
                for (int i = 0; i < smrs.Length; i++)
                {
                    if (smrs[i].sharedMesh.blendShapeCount > 0 && !smrs[i].name.ToLower().Contains("teeth"))
                    {
                        if (!body) body = smrs[i];
                    }
                }
            }
        }

        /// <summary>
        /// Find the Teeth child object SkinnedMeshRenderer
        /// </summary>
        public void GetTeeth()
        {
            SkinnedMeshRenderer[] smrs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrs.Length > 0)
            {
                for (int i = 0; i < smrs.Length; i++)
                {
                    if (smrs[i].sharedMesh.blendShapeCount > 0 && smrs[i].name.ToLower().Contains("teeth"))
                    {
                        if (!teeth) teeth = smrs[i];
                    }
                }
            }
        }

		/// <summary>
		/// Find left and right eye bones
		/// </summary>
		public void GetEyeBones()
		{
			Transform leftEyeTrans = ChildSearch(leftEyeName);
			if(leftEyeTrans) 
			{
				if (!leftEyeBone) leftEyeBone = leftEyeTrans.gameObject;
			}
			Transform rightEyeTrans = ChildSearch(rightEyeName);
			if (rightEyeTrans) 
			{
				if (!rightEyeBone) rightEyeBone = rightEyeTrans.gameObject;
			}
		}

		/// <summary>
        /// Find a child by name that ends with the search string. 
        /// This should compensates for BlendShape name prefixes variations.
		/// </summary>
		/// <param name="endsWith"></param>
		/// <returns></returns>
		public Transform ChildSearch(string endsWith)
		{
			Transform trans = null;
			
			children = transform.gameObject.GetComponentsInChildren<Transform>();
			
			for (int i=0; i<children.Length; i++)
			{
                if (children[i].name.EndsWith(endsWith)) trans = children[i];
			}
			
			return trans;
		}	
		
		/// <summary>
        /// Find a shape by name, that ends with the search string.
		/// </summary>
		/// <param name="skndMshRndr"></param>
		/// <param name="endsWith"></param>
		/// <returns></returns>
		public int ShapeSearch(SkinnedMeshRenderer skndMshRndr, string contains)
		{
			int index = -1;
			if (skndMshRndr)
			{
				for (int i=0; i<skndMshRndr.sharedMesh.blendShapeCount; i++)
				{
                    if (skndMshRndr.sharedMesh.GetBlendShapeName(i).Contains(contains))
					{
						index = i;
						break;
					}
				}
			}
			return index;
		}

		/// <summary>
		/// Populate the shapeName popup list
		/// </summary>
		public int GetShapeNames()
		{
			int nameCount = 0;
			
			if (body)
			{
				shapeNames = new string[body.sharedMesh.blendShapeCount];
				for (int i=0; i<body.sharedMesh.blendShapeCount; i++)
				{
					shapeNames[i] = body.sharedMesh.GetBlendShapeName(i);
					if (shapeNames[i] != "") nameCount++;
				}
			}
			
			return nameCount;
		}

		/// <summary>
		/// Set the body SkinnedMeshRenderer, this will also set the default SALSA shape groups
		/// </summary>
		/// <param name="bodySmr">Body smr.</param>
		public void SetBody(SkinnedMeshRenderer bodySmr)
		{
			this.body = bodySmr;
			if (saySmall.Count == 0) SetDefaultSmall();
			if (sayMedium.Count == 0) SetDefaultMedium();
			if (sayLarge.Count == 0) SetDefaultLarge();
		}

        /// <summary>
        /// Set the default saySmall shape group
        /// </summary>
        public void SetDefaultSmall()
        {
            
            int index = -1;
            string name = "";

            saySmall = new List<CM_ShapeGroup>();

            index = ShapeSearch(body, "AE_AA");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                saySmall.Add(new CM_ShapeGroup(index, name, 5f));
            }

            index = ShapeSearch(body, "Ax_E");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                saySmall.Add(new CM_ShapeGroup(index, name, 5f));
            }

            index = ShapeSearch(body, "UW_U");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                saySmall.Add(new CM_ShapeGroup(index, name, 20f));
            }

            index = ShapeSearch(body, "KG");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                saySmall.Add(new CM_ShapeGroup(index, name, 20f));
            }
        }
        /// <summary>
        /// Set the default sayMedium shape group
        /// </summary>
        public void SetDefaultMedium()
        {
            int index = -1; ;
            string name = "";

            sayMedium = new List<CM_ShapeGroup>();

            index = ShapeSearch(body, "H_EST");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 90f));
            }

            index = ShapeSearch(body, "FV");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 60f));
            }

            index = ShapeSearch(body, "RlipDown");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 50f));
            }

            index = ShapeSearch(body, "LlipDown");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 50f));
            }

            index = ShapeSearch(body, "RsmileClose");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 25f));
            }

            index = ShapeSearch(body, "LsmileClose");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 25f));
            }
        }
        /// <summary>
        /// Set the default sayLarge shape group
        /// </summary>
        public void SetDefaultLarge()
        {
            int index = -1; ;
            string name = "";

            sayLarge = new List<CM_ShapeGroup>();

            index = ShapeSearch(body, "UH_OO");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayLarge.Add(new CM_ShapeGroup(index, name, 80f));
            }

            index = ShapeSearch(body, "MouthOpen");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayLarge.Add(new CM_ShapeGroup(index, name, 80f));
            }
        }
	}
	
	/// <summary>
	/// Shape index and percentage class for SALSA/Fuse shape groups
	/// </summary>
	[System.Serializable]
	public class CM_ShapeGroup
	{
		public int shapeIndex;
		public string shapeName;
		public float percentage;
		
		public CM_ShapeGroup()
		{
			this.shapeIndex = 0;
			this.shapeName = "";
			this.percentage = 100f;
		}
		
		public CM_ShapeGroup(int shapeIndex, string shapeName, float percentage)
		{
			this.shapeIndex = shapeIndex;
			this.shapeName = shapeName;
			this.percentage = percentage;
		}
	}
}