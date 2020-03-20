-----------------
Version 2.6.0
-----------------
CM_AutodeskSync is designed to be used in congunction with SALSA with RandomEyes, and Autodesk Character Generator models, as outlined in the workflow created by:

Crazy Minnow Studio, LLC
CrazyMinnowStudio.com

The workflow is documented at the following URL, along with a downloadable zip file that contains the supporting files.

http://crazyminnowstudio.com/posts/using-autodesk-character-generator-with-salsa-with-randomeyes-v1.2/

Package Contents
----------------
Crazy Minnow Studio/SALSA with RandomEyes/Third Party Support/
	Autodesk Character Generator
		Editor
			CM_AutodeskSyncEditor.cs
				Custom inspector for CM_AutodeskSync.cs
			CM_AutodeskSetupEditor.cs
				Custom inspector for CM_AutodeskSetup.cs
		CM_AutodeskSync.cs
			Helper script to apply Salsa and RandomEyes BlendShape data to Autodesk Character BlendShapes.
		CM_AutodeskSetup.cs
			SALSA 1-click Autodesk setup script for new Autodesk characters.
		ReadMe.txt
			This readme file.			
	Shared
		CM_RandomMovement.CS
			Random movement script for simple precedural idle animations.


Installation Instructions
-------------------------
1. Install SALSA with RandomEyes into your project.
	Select [Window] -> [Asset Store]
	Once the Asset Store window opens, select the download icon, and download and import [SALSA with RandomEyes].

2. Import the SALSA with RandomEyes Autodesk Character Generator support package.
	Select [Assets] -> [Import Package] -> [Custom Package...]
	Browse to the [SALSA_3rdPartySupport_AutodeskCharacterGenerator.unitypackage] file and [Open].


Usage Instructions
------------------
1. Autodesk allows the export of multi-resolution characters. These characters contain several of the following child object:
	c_{SOME NAME} = These are crowd resolution objects.
	l_{SOME NAME} = These are low resolution objects.
	m_{SOME NAME} = These are medium resolution objects.
	h_{SOME NAME} = These are high resolution objects.
You should first add one of these characters to your scene, choose the resolution you wish to use, and delete all child objects that aren't part of that resolution set. Create a prefab of the selected resolution.

2. Add the Autodesk character prefab, that contains BlendShapes, to your scene.

3. Select the character root, then select:
	[Component] -> [Crazy Minnow Studio] -> [Autodesk Character Generator] -> [SALSA 1-Click Autodesk Setup]
	This will add and configure the necessary components for a complete SALSA setup.


What [SALSA 1-Click Autodesk Setup] does
----------------------------------------
1. It adds the following components:
	[Component] -> [Crazy Minnow Studio] -> [Salsa3D] (for lip sync)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for eyes)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for custom shapes)
	[Component] -> [Crazy Minnow Studio] -> [Autodesk Character Generator] -> [CM_AutodeskSync] (for syncing SALSA with RandomEyes to your Autodesk character)

2. On the Salsa3D component, it leaves the SkinnedMeshRenderer empty, and sets the SALSA [Range of Motion] to 75. (Set this to your preference)

3. On the RandomEyes3D componet for eyes, it leaves the SkinnedMeshRenderer empty, and sets the [Range of Motion] to 60. (Set this to your preference)

4. On the RandomEyes3D component for custom shapes, it attempts to find and link the main SkinnedMeshRenderer with BlendShapes. If you've exported a multi resolution character, delete the resolutions you're not using and create a prefab with only the resolution you wish to use. For example, a multi-res character will contain several of the following child object:
	c_{SOME NAME} = These are crowd resolution objects
	l_{SOME NAME} = These are low resolution objects
	m_{SOME NAME} = These are medium resolution objects
	h_{SOME NAME} = These are high resolution objects

5. On the RandomEyes3D component for custom shapes, it checks [Use Custom Shapes Only], Auto-Link's all custom shapes, and removes them from random selection. 
	You should selectively include small shapes like eyebrow and facial twitches in random selection to add natural random facial movement.

6. On the CM_AutodeskSync.cs component it attempts to link the following:
	Salsa3D
	RandomEyes3D (for eyes)
	The main SkinnedMeshRenderer with BlendShapes.
	The teeth SkinnedMeshRenderer with BlendShapes.
	The Left and Right eye bones.