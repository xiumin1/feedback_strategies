using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.Autodesk
{
	/// <summary>
	/// This is the custom inspector for CM_AutodeskSync, a script that acts as a proxy between 
	/// SALSA with RandomEyes and Autodesk Character Generator characters, and allows users to link SALSA with 
	/// RandomEyes to Autodesk characters without any model modifications.
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[CustomEditor(typeof(CM_AutodeskSync))]
	public class CM_AutodeskSyncEditor : Editor 
	{
		private CM_AutodeskSync autodeskSync; // CM_AutodeskSync reference
		private bool dirtySmall; // SaySmall dirty inspector status
		private bool dirtyMedium; // SayMedum dirty inspector status
		private bool dirtyLarge; // SayLarge dirty inspector status
		
		private float width = 0f; // Inspector width
		private float addWidth = 10f; // Percentage
		private float deleteWidth = 10f; // Percentage
		private float shapeNameWidth = 60f; // Percentage
		private float percentageWidth = 30f; // Percentage
		
		public void OnEnable()
		{
			// Get reference
			autodeskSync = target as CM_AutodeskSync;
			
			// Initialize
			if (autodeskSync.initialize)
			{
				autodeskSync.GetSalsa3D();
				autodeskSync.GetRandomEyes3D();
                autodeskSync.GetBody();
                autodeskSync.GetTeeth();
				autodeskSync.GetEyeBones();
				if (autodeskSync.saySmall == null) autodeskSync.saySmall = new List<CM_ShapeGroup>();
				if (autodeskSync.sayMedium == null) autodeskSync.sayMedium = new List<CM_ShapeGroup>();
				if (autodeskSync.sayLarge == null) autodeskSync.sayLarge = new List<CM_ShapeGroup>();
				autodeskSync.GetShapeNames();
				autodeskSync.SetDefaultSmall();
				autodeskSync.SetDefaultMedium();
				autodeskSync.SetDefaultLarge();
				autodeskSync.initialize = false;
			}
		}
		
		public override void OnInspectorGUI()
		{
			// Minus 45 width for the scroll bar
			width = Screen.width - 50f;
			
			// Set dirty flags
			dirtySmall = false; 
			dirtyMedium = false;
			dirtyLarge = false;
			
			// Keep trying to get the shapeNames until I've got them
			if (autodeskSync.GetShapeNames() == 0) autodeskSync.GetShapeNames();

			// If the body SkinnedMeshRenderer has changed, set the default shape groups
			if (autodeskSync.prevBody != autodeskSync.body)
			{
				if (autodeskSync.saySmall.Count == 0) autodeskSync.SetDefaultSmall();
				if (autodeskSync.sayMedium.Count == 0) autodeskSync.SetDefaultMedium();
				if (autodeskSync.sayLarge.Count == 0) autodeskSync.SetDefaultLarge();
				autodeskSync.prevBody = autodeskSync.body;
			}
			
			// Make sure the CM_ShapeGroups are initialized
			if (autodeskSync.saySmall == null) autodeskSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (autodeskSync.sayMedium == null) autodeskSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (autodeskSync.sayLarge == null) autodeskSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();
			
			GUILayout.Space(10);
			EditorGUILayout.BeginVertical(new GUILayoutOption[] {GUILayout.Width(width)});
			autodeskSync.salsa3D = EditorGUILayout.ObjectField(
				"Salsa3D", autodeskSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
			autodeskSync.randomEyes3D = EditorGUILayout.ObjectField(
				"RandomEyes3D", autodeskSync.randomEyes3D, typeof(RandomEyes3D), true) as RandomEyes3D;
			autodeskSync.body = EditorGUILayout.ObjectField(
				"Body", autodeskSync.body, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
			autodeskSync.teeth = EditorGUILayout.ObjectField(
				"Teeth", autodeskSync.teeth, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
			autodeskSync.leftEyeBone = EditorGUILayout.ObjectField(
				"Left Eye Bone", autodeskSync.leftEyeBone, typeof(GameObject), true) as GameObject;
			autodeskSync.rightEyeBone = EditorGUILayout.ObjectField(
				"Right Eye Bone", autodeskSync.rightEyeBone, typeof(GameObject), true) as GameObject;
			autodeskSync.syncExpressions = EditorGUILayout.Toggle(
				new GUIContent("Sync Expressions", "Only enable if you're using custom shapes or shape groups, it's more processor intensive."), 
				autodeskSync.syncExpressions);
			EditorGUILayout.EndVertical();
			
			
			GUILayout.Space(10);
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)}); // Horizontal rule
			GUILayout.Space(10);
			
			
			if (autodeskSync.body)
			{
				EditorGUILayout.LabelField("SALSA shape groups");
				GUILayout.Space(10);
				
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SaySmall Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					autodeskSync.saySmall.Add(new CM_ShapeGroup());
                    autodeskSync.initialize = false;
                }
				EditorGUILayout.EndHorizontal();
				if (autodeskSync.saySmall.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();
					
					for (int i=0; i<autodeskSync.saySmall.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + autodeskSync.saySmall[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							autodeskSync.saySmall.RemoveAt(i);
							dirtySmall = true;
							break;
						}
						if (!dirtySmall)
						{
							autodeskSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
								autodeskSync.saySmall[i].shapeIndex, autodeskSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							autodeskSync.saySmall[i].shapeName = 
								autodeskSync.body.sharedMesh.GetBlendShapeName(autodeskSync.saySmall[i].shapeIndex);
							autodeskSync.saySmall[i].percentage = EditorGUILayout.Slider(
								autodeskSync.saySmall[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
                            autodeskSync.initialize = false;
                        }
						GUILayout.EndHorizontal();
					}
				}
				
				GUILayout.Space(10);
				
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SayMedium Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					autodeskSync.sayMedium.Add(new CM_ShapeGroup());
                    autodeskSync.initialize = false;
                }
				EditorGUILayout.EndHorizontal();
				if (autodeskSync.sayMedium.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();
					
					for (int i=0; i<autodeskSync.sayMedium.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + autodeskSync.sayMedium[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							autodeskSync.sayMedium.RemoveAt(i);
							dirtyMedium = true;
							break;
						}
						if (!dirtyMedium)
						{
							autodeskSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
								autodeskSync.sayMedium[i].shapeIndex, autodeskSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							autodeskSync.sayMedium[i].shapeName = 
								autodeskSync.body.sharedMesh.GetBlendShapeName(autodeskSync.sayMedium[i].shapeIndex);
							autodeskSync.sayMedium[i].percentage = EditorGUILayout.Slider(
								autodeskSync.sayMedium[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
                            autodeskSync.initialize = false;
                        }
						GUILayout.EndHorizontal();
					}
				}
				
				GUILayout.Space(10);
				
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SayLarge Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					autodeskSync.sayLarge.Add(new CM_ShapeGroup());
                    autodeskSync.initialize = false;
                }
				EditorGUILayout.EndHorizontal();
				if (autodeskSync.sayLarge.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();
					
					for (int i=0; i<autodeskSync.sayLarge.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + autodeskSync.sayLarge[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							autodeskSync.sayLarge.RemoveAt(i);
							dirtyLarge = true;
							break;
						}
						if (!dirtyLarge)
						{
							autodeskSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
								autodeskSync.sayLarge[i].shapeIndex, autodeskSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							autodeskSync.sayLarge[i].shapeName = autodeskSync.body.sharedMesh.GetBlendShapeName(autodeskSync.sayLarge[i].shapeIndex);
							autodeskSync.sayLarge[i].percentage = EditorGUILayout.Slider(
								autodeskSync.sayLarge[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
                            autodeskSync.initialize = false;
                        }
						GUILayout.EndHorizontal();
					}
				}
			}
		}
	}
}
