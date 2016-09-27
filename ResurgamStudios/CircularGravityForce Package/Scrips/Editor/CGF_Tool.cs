/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for adding CGF tool menu , menu context, and gameobejct items.
*******************************************************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CircularGravityForce
{
    public class CGF_Tool : EditorWindow
    {
        #region Enumes

        enum CGFOptions
        {
            _3D,
            _2D
        }

        //Constructor
        public CGF_Tool()
        {
        }

        #endregion

        #region MenuItems / ToolBar

        [MenuItem("GameObject/3D Object/CGF", false, 0)]
        public static void GameObject_3D_Object_CGF()
        {
            CreateSimpleCGF();
        }

        [MenuItem("GameObject/2D Object/CGF 2D", false, 0)]
        public static void GameObject_2D_Object_CGF()
        {
            CreateSimpleCGF2D();
        }

        [MenuItem("CONTEXT/CGF/Controls->Add 'AxisControls'", false)]
        public static void AddAxisControls(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_AxisControls>();
        }

        [MenuItem("CONTEXT/CGF/Controls->Add 'KeyControls'", false)]
        public static void AddKeyControls(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_KeyControls>();
        }

        [MenuItem("CONTEXT/CGF2D/Controls->Add 2D 'AxisControls'", false)]
        public static void AddAxisControls2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_AxisControls2D>();
        }

        [MenuItem("CONTEXT/CGF2D/Controls->Add 2D 'KeyControls'", false)]
        public static void AddKeyControls2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_KeyControls2D>();
        }

        [MenuItem("CONTEXT/CGF/Mods->Add 'Pulse'", false)]
        public static void AddPulse(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_Pulse>();
        }

        [MenuItem("CONTEXT/CGF/Mods->Add 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_SizeByRaycast>();
        }

        [MenuItem("CONTEXT/CGF2D/Mods->Add 2D 'Pulse'", false)]
        public static void AddPulse2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_Pulse2D>();
        }

        [MenuItem("CONTEXT/CGF2D/Mods->Add 2D 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_SizeByRaycast2D>();
        }

        [MenuItem("CONTEXT/CGF/Triggers->Add 'Enable'", false)]
        static void CONTEXT_CircularGravity_Create_Enable(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            CreateEnableTrigger(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF2D/Triggers->Add 2D 'Enable'", false)]
        static void CONTEXT_CircularGravity2D_Create_Enable(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            CreateEnableTrigger2D(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF/Triggers->Add 'Hover'", false)]
        static void CONTEXT_CircularGravity_Create_Hover(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            CreateHoverTrigger(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF2D/Triggers->Add 2D 'Hover'", false)]
        static void CONTEXT_CircularGravity2D_Create_Hover(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            CreateHoverTrigger2D(cgf.gameObject, cgf);
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Quick CGF #%q", false, 1)]
        public static void QuickCGF()
        {
            CreateSimpleCGF();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Quick CGF 2D #&q", false, 2)]
        public static void QuickCGF2D()
        {
            CreateSimpleCGF2D();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Wizard #%w", false, 3)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(CGF_Tool));

#if (UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                editorWindow.titleContent = new GUIContent("CGF Wizard");
#else
            editorWindow.title = "CGF Wizard";
#endif
        }
        
        [MenuItem("Tools/Resurgam Studios/CGF/Controls/3D/Add 'AxisControls'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Controls/3D/Add 'KeyControls'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/3D/Add 'Pulse'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/3D/Add 'SizeByRaycast'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/3D/Add 'Enable'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/3D/Add 'Hover'", true)]
        public static bool CGF_Validation()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<CGF>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Controls/2D/Add 2D 'AxisControls'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Controls/2D/Add 2D 'KeyControls'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/2D/Add 2D 'Pulse'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/2D/Add 2D 'SizeByRaycast'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/2D/Add 2D 'Enable'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/2D/Add 2D 'Hover'", true)]
        public static bool CGF_Validation2D()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<CGF2D>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Controls/3D/Add 'AxisControls'", false)]
        public static void AddAxisControls()
        {
            Selection.activeGameObject.AddComponent<CGF_AxisControls>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Controls/3D/Add 'KeyControls'", false)]
        public static void AddKeyControls()
        {
            Selection.activeGameObject.AddComponent<CGF_KeyControls>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Controls/2D/Add 2D 'AxisControls'", false)]
        public static void Add2DAxisControls()
        {
            Selection.activeGameObject.AddComponent<CGF_AxisControls2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Controls/2D/Add 2D 'KeyControls'", false)]
        public static void Add2DKeyControls()
        {
            Selection.activeGameObject.AddComponent<CGF_KeyControls2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/3D/Add 'Pulse'", false)]
        public static void AddPulse()
        {
            Selection.activeGameObject.AddComponent<CGF_Pulse>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/3D/Add 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast()
        {
            Selection.activeGameObject.AddComponent<CGF_SizeByRaycast>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/2D/Add 2D 'Pulse'", false)]
        public static void Add2DPulse()
        {
            Selection.activeGameObject.AddComponent<CGF_Pulse2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/2D/Add 2D 'SizeByRaycast'", false)]
        public static void Add2DSizeByRaycast()
        {
            Selection.activeGameObject.AddComponent<CGF_SizeByRaycast2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/3D/Add 'Enable'", false)]
        public static void Trigger_AddEnable()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF>();

                    CreateEnableTrigger(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF();
                CreateEnableTrigger(cgf, cgf.GetComponent<CGF>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/3D/Add 'Hover'", false)]
        public static void Trigger_AddHover()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF>();

                    CreateHoverTrigger(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF();
                CreateHoverTrigger(cgf, cgf.GetComponent<CGF>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/2D/Add 2D 'Enable'", false)]
        public static void Trigger2D_AddEnable()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF2D>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF2D>();

                    CreateEnableTrigger2D(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF2D();
                CreateEnableTrigger2D(cgf, cgf.GetComponent<CGF2D>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/2D/Add 2D 'Hover'", false)]
        public static void Trigger2D_AddHover()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF2D>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF2D>();

                    CreateHoverTrigger2D(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF2D();
                CreateHoverTrigger2D(cgf, cgf.GetComponent<CGF2D>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Basic Overview", false)]
        public static void SupportTutVidBasicOverview()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=4Y4vxsAgVWg");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Advanced Overview", false)]
        public static void SupportTutVidAdvanceOverview()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=qEi8GQRxQAY");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Simple Car", false)]
        public static void SupportTutVidCar()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=cc5Wlsesgbo");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Simple Rocket", false)]
        public static void SupportTutVidRocket()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=edrS9BTXx1E");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Simple Ball", false)]
        public static void SupportTutVidBall()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=Ba9Quxq7x08");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Simple Hovercraft", false)]
        public static void SupportTutVidHovercraft()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=bdh0ekHca7U");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Simple Planets", false)]
        public static void SupportTutVidPlanets()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=LAYr6b7NQ20");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Tutorial Videos/Filtering", false)]
        public static void SupportTutVidFiltering()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=oiXBFhMO4cM");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Unity Form", false)]
        public static void SupportUnityForm()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/circular-gravity-force.217100/");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Asset Store", false)]
        public static void SupportAssetStore()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/8181");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Website", false)]
        public static void SupportWebsite()
        {
            Application.OpenURL("http://resurgamstudios.com/cgf.html");
        }

        #endregion

        #region Properties

        //GUI Properties
        private Vector2 scrollPos;

        private CGFOptions cgfOptions = CGFOptions._3D;

        private bool advancedOptions = false;

        //3D
        private CGF.Shape cgfShape = CGF.Shape.Sphere;
        private CGF.ForceType cgfForceType = CGF.ForceType.ForceAtPosition;
        private ForceMode cgfForceMode = ForceMode.Force;
        private float cfgExplosionForceUpwardsModifier = 0f;
        private float cfgTorqueMaxAngularVelocity = 10f;
        private float cgfSize = 5;
        private Vector3 cgfBoxSize = Vector3.one * 5f;
        private float cgfCapsuleRadius = 2;
        private float cgfForcePower = 10;
        private bool modAxisControls = false;
        private bool modKeyControls = false;
        private bool modPulse = false;
        private bool modSize = false;
        private bool triggerEnable = false;
        private bool triggerHover = false;
        private bool buttonCreate3D = false;


        //2D
        private CGF2D.Shape2D cgfShape2D = CGF2D.Shape2D.Sphere;
        private CGF2D.ForceType2D cgfForceType2D = CGF2D.ForceType2D.ForceAtPosition;
        private ForceMode2D cgfForceMode2D = ForceMode2D.Force;
        private float cgfSize2D = 5;
        private Vector2 cgfBoxSize2D = Vector2.one * 5f;
        private float cgfForcePower2D = 10;
        private bool modAxisControls2D = false;
        private bool modKeyControls2D = false;
        private bool modPulse2D = false;
        private bool modSize2D = false;
        private bool triggerEnable2D = false;
        private bool triggerHover2D = false;
        private bool buttonCreate2D = false;

        #endregion

        #region Unity Functions

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Circular Gravity Force:", EditorStyles.boldLabel);
            cgfOptions = (CGFOptions)EditorGUILayout.EnumPopup("   Type:", cgfOptions);

            switch (cgfOptions)
            {
                case CGFOptions._3D:
                    DrawCGFGUI();
                    break;
                case CGFOptions._2D:
                    DrawCGF2DGUI();
                    break;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        void Update()
        {
            switch (cgfOptions)
            {
                case CGFOptions._3D:
                    UpdateCGFGUI();
                    break;
                case CGFOptions._2D:
                    UpdateCGF2DGUI();
                    break;
            }
        }

        private void DrawCGFGUI()
        {
            cgfShape = (CGF.Shape)EditorGUILayout.EnumPopup("   Shape:", cgfShape);

            cgfForceType = (CGF.ForceType)EditorGUILayout.EnumPopup("   Force Type:", cgfForceType);

            cgfForceMode = (ForceMode)EditorGUILayout.EnumPopup("   Force Mode:", cgfForceMode);

            cgfForcePower = EditorGUILayout.FloatField("   Force Power:", cgfForcePower);

            if (cgfForceType == CGF.ForceType.ExplosionForce)
            {
                cfgExplosionForceUpwardsModifier = EditorGUILayout.FloatField("   Upwards Modifier:", cfgExplosionForceUpwardsModifier);
            }
            else if (cgfForceType == CGF.ForceType.Torque)
            {
                cfgTorqueMaxAngularVelocity = EditorGUILayout.FloatField("   Max Angular Velocity:", cfgTorqueMaxAngularVelocity);
            }

            if (cgfShape != CGF.Shape.Box)
            {
                cgfSize = EditorGUILayout.FloatField("   Size:", cgfSize);
                if (cgfSize < 0)
                    cgfSize = 0;
            }
            else
            {
                cgfBoxSize = EditorGUILayout.Vector3Field("   Box Size:", cgfBoxSize);
                if (cgfBoxSize.x < 0)
                    cgfBoxSize = new Vector3(0, cgfBoxSize.y, cgfBoxSize.z);
                if (cgfBoxSize.y < 0)
                    cgfBoxSize = new Vector3(cgfBoxSize.x, 0, cgfBoxSize.z);
                if (cgfBoxSize.z < 0)
                    cgfBoxSize = new Vector3(cgfBoxSize.x, cgfBoxSize.y, 0);

            }

            if (cgfShape == CGF.Shape.Capsule)
            {
                cgfCapsuleRadius = EditorGUILayout.FloatField("   Capsule Radius:", cgfCapsuleRadius);
                if(cgfCapsuleRadius < 0)
                    cgfCapsuleRadius = 0;
            }

            EditorGUILayout.Space();

            advancedOptions = EditorGUILayout.Foldout(advancedOptions, "Advanced Options");

            if (advancedOptions)
            {
                EditorGUILayout.LabelField("   Controls:", EditorStyles.boldLabel);
                modAxisControls = EditorGUILayout.Toggle("   Add 'Axis Controls':", modAxisControls);
                modKeyControls = EditorGUILayout.Toggle("   Add 'Key Controls':", modKeyControls);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("   Mods:", EditorStyles.boldLabel);
                modPulse = EditorGUILayout.Toggle("   Add 'Pulse':", modPulse);
                modSize = EditorGUILayout.Toggle("   Add 'Size By Raycast':", modSize);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("   Tiggers:", EditorStyles.boldLabel);
                triggerEnable = EditorGUILayout.Toggle("   Create 'Enable':", triggerEnable);
                triggerHover = EditorGUILayout.Toggle("   Create 'Hover':", triggerHover);
            }

            EditorGUILayout.Space();

            buttonCreate3D = GUILayout.Button("Create CGF");
        }

        private void DrawCGF2DGUI()
        {
            cgfShape2D = (CGF2D.Shape2D)EditorGUILayout.EnumPopup("   Shape:", cgfShape2D);

            cgfForceType2D = (CGF2D.ForceType2D)EditorGUILayout.EnumPopup("   Force Type:", cgfForceType2D);

            cgfForceMode2D = (ForceMode2D)EditorGUILayout.EnumPopup("   Force Mode:", cgfForceMode2D);

            cgfForcePower2D = EditorGUILayout.FloatField("   Force Power:", cgfForcePower2D);

            if (cgfShape2D != CGF2D.Shape2D.Box)
            {
                cgfSize2D = EditorGUILayout.FloatField("   Size:", cgfSize2D);
                if (cgfSize2D < 0)
                    cgfSize2D = 0;
            }
            else
            {
                cgfBoxSize2D = EditorGUILayout.Vector2Field("   Box Size:", cgfBoxSize2D);
                if (cgfBoxSize2D.x < 0)
                    cgfBoxSize2D = new Vector3(0, cgfBoxSize2D.y);
                if (cgfBoxSize2D.y < 0)
                    cgfBoxSize2D = new Vector3(cgfBoxSize2D.x, 0);

            }

            EditorGUILayout.Space();

            advancedOptions = EditorGUILayout.Foldout(advancedOptions, "Advanced Options");

            if (advancedOptions)
            {
                EditorGUILayout.LabelField("   Controls:", EditorStyles.boldLabel);
                modAxisControls2D = EditorGUILayout.Toggle("   Add 'Axis Controls':", modAxisControls2D);
                modKeyControls2D = EditorGUILayout.Toggle("   Add 'Key Controls':", modKeyControls2D);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("   Mods:", EditorStyles.boldLabel);
                modPulse2D = EditorGUILayout.Toggle("   Add 'Pulse':", modPulse2D);
                modSize2D = EditorGUILayout.Toggle("   Add 'Size By Raycast':", modSize2D);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("   Tiggers:", EditorStyles.boldLabel);
                triggerEnable2D = EditorGUILayout.Toggle("   Create 'Enable':", triggerEnable2D);
                triggerHover2D = EditorGUILayout.Toggle("   Create 'Hover':", triggerHover2D);
            }

            EditorGUILayout.Space();

            buttonCreate2D = GUILayout.Button("Create CGF");
        }

        private void UpdateCGFGUI()
        {
            if (buttonCreate3D)
            {
                //Creates empty gameobject.
                GameObject cgf = new GameObject();

                //Creates Circular Gravity Force component
                CGF circularGravity = cgf.AddComponent<CGF>();

                //Adds Controls
                if (modAxisControls)
                {
                    cgf.AddComponent<CGF_AxisControls>();
                }

                if (modKeyControls)
                {
                    cgf.AddComponent<CGF_KeyControls>();
                }

                //Adds Mods
                if (modPulse)
                {
                    cgf.AddComponent<CGF_Pulse>();
                }
                if (modSize)
                {
                    cgf.AddComponent<CGF_SizeByRaycast>();
                }

                //Sets up properties
                circularGravity._shape = cgfShape;
                circularGravity._forceType = cgfForceType;
                circularGravity.ForcePower = cgfForcePower;
                circularGravity._forceMode = cgfForceMode;
                circularGravity._forceTypeProperties.ExplosionForceUpwardsModifier = cfgExplosionForceUpwardsModifier;
                circularGravity._forceTypeProperties.TorqueMaxAngularVelocity = cfgTorqueMaxAngularVelocity;
                circularGravity.Size = cgfSize;
                circularGravity.BoxSize = cgfBoxSize;
                circularGravity.CapsuleRadius = cgfCapsuleRadius;
                circularGravity._drawGravityProperties.GravityLineMaterial = new Material(Shader.Find("GUI/Text Shader"));

                //Sets gameojbect Name
                cgf.name = "CGF";

                FocusGameObject(cgf);

                if (triggerEnable)
                {
                    CreateEnableTrigger(cgf, circularGravity);
                }
                if (triggerHover)
                {
                    CreateHoverTrigger(cgf, circularGravity);
                }

                //Disables the buttonCreateNewCGF
                buttonCreate3D = false;
            }
        }

        private void UpdateCGF2DGUI()
        {
            if (buttonCreate2D)
            {
                //Creates empty gameobject.
                GameObject cgf = new GameObject();

                //Creates Circular Gravity Force component
                CGF2D circularGravity = cgf.AddComponent<CGF2D>();

                //Adds Controls
                if (modAxisControls2D)
                {
                    cgf.AddComponent<CGF_AxisControls2D>();
                }

                if (modKeyControls2D)
                {
                    cgf.AddComponent<CGF_KeyControls2D>();
                }

                //Adds Mods
                if (modPulse2D)
                {
                    cgf.AddComponent<CGF_Pulse2D>();
                }
                if (modSize2D)
                {
                    cgf.AddComponent<CGF_SizeByRaycast2D>();
                }

                //Sets up properties
                circularGravity._shape2D = cgfShape2D;
                circularGravity._forceType2D = cgfForceType2D;
                circularGravity.ForcePower = cgfForcePower2D;
                circularGravity._forceMode2D = cgfForceMode2D;
                circularGravity.Size = cgfSize2D;
                circularGravity.BoxSize = cgfBoxSize2D;

                //Sets gameojbect Name
                cgf.name = "CGF 2D";

                FocusGameObject(cgf, true);

                if (triggerEnable2D)
                {
                    CreateEnableTrigger2D(cgf, circularGravity);
                }
                if (triggerHover2D)
                {
                    CreateHoverTrigger2D(cgf, circularGravity);
                }

                //Disables the buttonCreateNewCGF
                buttonCreate2D = false;
            }
        }

        #endregion

        #region Events

        private static GameObject CreateSimpleCGF()
        {
            var cgf = CGF.CreateCGF();

            FocusGameObject(cgf);

            return cgf;
        }
        private static GameObject CreateSimpleCGF2D()
        {
            var cgf = CGF2D.CreateCGF();

            FocusGameObject(cgf, true);

            return cgf;
        }

        private static void CreateEnableTrigger(GameObject cgf = null, CGF circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Enable";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + Vector3.right * 6f;
            triggerEnableObj.transform.rotation = Quaternion.Euler(0, 90, 0);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj);
            }
        }

        private static void CreateHoverTrigger(GameObject cgf = null, CGF circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Hover";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + Vector3.left * 6f;
            triggerEnableObj.transform.rotation = Quaternion.Euler(-180, 0, 0);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj);
            }
        }

        private static void CreateEnableTrigger2D(GameObject cgf = null, CGF2D circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Enable";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger2D>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger2D>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + new Vector3(0, 6f);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj, true);
            }
        }

        private static void CreateHoverTrigger2D(GameObject cgf = null, CGF2D circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Hover";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger2D>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger2D>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + new Vector3(0, -6f);
            triggerEnableObj.transform.rotation = Quaternion.Euler(0, 0, 180f);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj, true);
            }
        }

        private static void FocusGameObject(GameObject focusGameObject, bool in2D = false)
        {
            //Sets the create location for the Circular Gravity Force gameobject
            if (SceneView.lastActiveSceneView != null)
            {
                if (in2D)
                    focusGameObject.transform.position = new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0f);
                else
                    focusGameObject.transform.position = SceneView.lastActiveSceneView.pivot;

                //Sets the Circular Gravity Force gameobject selected in the hierarchy
                Selection.activeGameObject = focusGameObject;

                //focus the editor camera on the Circular Gravity Force gameobject
                SceneView.lastActiveSceneView.FrameSelected();
            }
            else
            {
                focusGameObject.transform.position = Vector3.zero;
            }
        }

        #endregion
    }
}