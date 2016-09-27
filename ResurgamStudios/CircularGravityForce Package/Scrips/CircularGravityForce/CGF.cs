/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Core logic for Circular Gravity Force.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace CircularGravityForce
{
    [AddComponentMenu("Physics/Circular Gravity Force", -1)]
    public class CGF : MonoBehaviour
    {
        #region Enums

        //Force Types
        public enum ForceType
        {
            ForceAtPosition,
            Force,
            Torque,
            ExplosionForce,
            GravitationalAttraction
        }

        //Force Types
        public enum Shape
        {
            Sphere,
            Capsule,
            Raycast,
            Box,
        }

        //Effect Types
        public enum PhysicsEffect
        {
            None,
            NoGravity
        }
        #endregion

        #region Classes

        //Manages all force type properties
        [System.Serializable]
        public class ForceTypeProperties
        {
            [SerializeField, Tooltip("Adjustment to the apparent position of the explosion to make it seem to lift objects.")]
            private float explosionForceUpwardsModifier = 0f;
            public float ExplosionForceUpwardsModifier
            {
                get { return explosionForceUpwardsModifier; }
                set { explosionForceUpwardsModifier = value; }
            }

            [SerializeField, Tooltip("The maximimum angular velocity of the rigidbody.")]
            private float torqueMaxAngularVelocity = 10f;
            public float TorqueMaxAngularVelocity
            {
                get { return torqueMaxAngularVelocity; }
                set { torqueMaxAngularVelocity = value; }
            }
        }

        //Manages all constraint properties
        [System.Serializable]
        public class ConstraintProperties
        {
            public enum AlignDirection
            {
                Up,
                Down,
                Left,
                Right,
                Forward,
                Backword
            }

            [SerializeField, Tooltip("Aligns GameObjects to force point.")]
            private bool alignToForce = false;
            public bool AlignToForce
            {
                get { return alignToForce; }
                set { alignToForce = value; }
            }

            [SerializeField, Tooltip("If alignToForce is enabled, lets you pick the align direction of the GameObjects.")]
            private AlignDirection alignDirection = AlignDirection.Up;
            public AlignDirection _alignDirection
            {
                get { return alignDirection; }
                set { alignDirection = value; }
            }

            [SerializeField, Tooltip("Rotation speed that the GameObjects align.")]
            private float slerpSpeed = 8f;
            public float SlerpSpeed
            {
                get { return slerpSpeed; }
                set { slerpSpeed = value; }
            }

            [SerializeField, Tooltip("GameObject filtering options.")]
            private GameobjectFilter gameobjectFilter;
            public GameobjectFilter _gameobjectFilter
            {
                get { return gameobjectFilter; }
                set { gameobjectFilter = value; }
            }

            [SerializeField, Tooltip("Tag filtering options.")]
            private TagFilter tagFilter;
            public TagFilter _tagFilter
            {
                get { return tagFilter; }
                set { tagFilter = value; }
            }

            [SerializeField, Tooltip("Layer filtering options.")]
            private LayerFilter layerFilter;
            public LayerFilter _layerFilter
            {
                get { return layerFilter; }
                set { layerFilter = value; }
            }
        }

        //Manages all effected objects for when using special effect
        [System.Serializable]
        public class SpecialEffect
        {
            [SerializeField, Tooltip("Physics effect options.")]
            private PhysicsEffect physicsEffect = PhysicsEffect.None;
            public PhysicsEffect _physicsEffect
            {
                get { return physicsEffect; }
                set { physicsEffect = value; }
            }

            [SerializeField, Tooltip("Time that GameObject is effected.")]
            private float timeEffected = 0.0f;
            public float TimeEffected
            {
                get { return timeEffected; }
                set { timeEffected = value; }
            }

            [SerializeField, Tooltip("GameObject that is attached when effected.")]
            private GameObject attachedGameObject;
            public GameObject AttachedGameObject
            {
                get { return attachedGameObject; }
                set { attachedGameObject = value; }
            }

            [SerializeField, Tooltip("Holds all the effected objects.")]
            private EffectedObjects effectedObjects;
            public EffectedObjects _effectedObjects
            {
                get { return effectedObjects; }
                set { effectedObjects = value; }
            }

            //Constructor
            public SpecialEffect()
            {
                _effectedObjects = new EffectedObjects();
            }
        }

        [SerializeField, Tooltip("Used for toggling filter options.")]
        private bool toggleFilterOptions;
        public bool ToggleFilterOptions
        {
            get { return toggleFilterOptions; }
            set { toggleFilterOptions = value; }
        }

        //GameObject filtering options
        [System.Serializable]
        public class GameobjectFilter
        {
            //GameObject filter options
            public enum GameObjectFilterOptions
            {
                Disabled,
                OnlyAffectListedGameobjects,
                DontAffectListedGameobjects,
            }

            [SerializeField, Tooltip("GameObject filter options.")]
            private GameObjectFilterOptions gameObjectFilterOptions = GameObjectFilterOptions.Disabled;
            public GameObjectFilterOptions _gameobjectFilterOptions
            {
                get { return gameObjectFilterOptions; }
                set { gameObjectFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed GameObject used for the filter.")]
            private List<GameObject> gameobjectList;
            public List<GameObject> GameobjectList
            {
                get { return gameobjectList; }
                set { gameobjectList = value; }
            }
        }

        //Tag filtering options
        [System.Serializable]
        public class TagFilter
        {
            //Tag filter options
            public enum TagFilterOptions
            {
                Disabled,
                OnlyAffectListedTags,
                DontAffectListedTags,
            }

            [SerializeField, Tooltip("Tag filter options.")]
            private TagFilterOptions tagFilterOptions = TagFilterOptions.Disabled;
            public TagFilterOptions _tagFilterOptions
            {
                get { return tagFilterOptions; }
                set { tagFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed tags used for the filter.")]
            private List<string> tagsList;
            public List<string> TagsList
            {
                get { return tagsList; }
                set { tagsList = value; }
            }
        }

        //Trigger Area Filter
        [System.Serializable]
        public class TriggerAreaFilter
        {
            //Trigger Options
            public enum TriggerAreaFilterOptions
            {
                Disabled,
                OnlyAffectWithinTigger,
                DontAffectWithinTigger,
            }

            [SerializeField, Tooltip("Trigger area filter options.")]
            private TriggerAreaFilterOptions triggerAreaFilterOptions = TriggerAreaFilterOptions.Disabled;
            public TriggerAreaFilterOptions _triggerAreaFilterOptions
            {
                get { return triggerAreaFilterOptions; }
                set { triggerAreaFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed triggers used for the filter.")]
            private Collider triggerArea;
            public Collider TriggerArea
            {
                get { return triggerArea; }
                set { triggerArea = value; }
            }
        }

        //Tag filtering options
        [System.Serializable]
        public class LayerFilter
        {
            //Tag filter options
            public enum LayerFilterOptions
            {
                Disabled,
                OnlyAffectListedLayers,
                DontAffectListedLayers,
            }

            [SerializeField, Tooltip("Layer filter options.")]
            private LayerFilterOptions layerFilterOptions = LayerFilterOptions.Disabled;
            public LayerFilterOptions _layerFilterOptions
            {
                get { return layerFilterOptions; }
                set { layerFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed layers used for the filter.")]
            private List<string> layerList;
            public List<string> LayerList
            {
                get { return layerList; }
                set { layerList = value; }
            }
        }

        //Draw gravity properties
        [System.Serializable]
        public class DrawGravityProperties
        {
            [SerializeField, Tooltip("Thinkness of the line drawn.")]
            private float thickness = 0.05f;
            public float Thickness
            {
                get { return thickness; }
                set { thickness = value; }
            }

            [SerializeField, Tooltip("Material on line.")]
            private Material gravityLineMaterial;
            public Material GravityLineMaterial
            {
                get { return gravityLineMaterial; }
                set { gravityLineMaterial = value; }
            }

            [SerializeField, Tooltip("Enable/Disables drawing gravity force lines.")]
            private bool drawGravityForce = false;
            public bool DrawGravityForce
            {
                get { return drawGravityForce; }
                set { drawGravityForce = value; }
            }

            //Used to see gravity area from gizmos
            private bool drawGravityForceGizmos = true;
            public bool DrawGravityForceGizmos
            {
                get { return drawGravityForceGizmos; }
                set { drawGravityForceGizmos = value; }
            }
        }

        //Draw gravity properties
        [System.Serializable]
        public class MemoryProperties
        {
            [SerializeField, Tooltip("See affected colliders in gizmo.")]
            private bool seeColliders = false;
            public bool SeeColliders
            {
                get { return seeColliders; }
                set { seeColliders = value; }
            }

            [SerializeField, Tooltip("See affected raycasthits in gizmo.")]
            private bool seeRaycastHits = false;
            public bool SeeRaycastHits
            {
                get { return seeRaycastHits; }
                set { seeRaycastHits = value; }
            }

            [SerializeField, Tooltip("Used for toggling memory properties.")]
            private bool toggleMemoryProperties;
            public bool ToggleMemoryProperties
            {
                get { return toggleMemoryProperties; }
                set { toggleMemoryProperties = value; }
            }

            [SerializeField, Tooltip("Use Non-Alloc physics.")]
            private bool nonAllocPhysics = false;
            public bool NonAllocPhysics
            {
                get { return nonAllocPhysics; }
                set { nonAllocPhysics = value; }
            }

            [SerializeField, Tooltip("Collider buffer size for the Non-Alloc 'Sphere' and 'Box' types.")]
            private int colliderBuffer = 100;
            public int ColliderBuffer
            {
                get { return colliderBuffer; }
                set { colliderBuffer = value; }
            }

            [SerializeField, Tooltip("RaycastHit buffer size for the Non-Alloc 'Capsule' and 'Raycast' shapes.")]
            private int raycastHitBuffer = 100;
            public int RaycastHitBuffer
            {
                get { return raycastHitBuffer; }
                set { raycastHitBuffer = value; }
            }
        }

        //Manages all effected objects for when using SpecialEffect
        public class EffectedObjects
        {
            //List of all EffectedObject
            private List<EffectedObject> effectedObjectList;
            public List<EffectedObject> EffectedObjectList
            {
                get { return effectedObjectList; }
                set { effectedObjectList = value; }
            }

            //Constructor
            public EffectedObjects()
            {
                EffectedObjectList = new List<EffectedObject>();
            }

            //Used to add to the effectedObjectList
            public void AddedEffectedObject(Rigidbody touchedObject, PhysicsEffect physicsEffect, GameObject attachedGameObject)
            {
                if (EffectedObjectList.Count == 0)
                {
                    EffectedObject effectedObject = new EffectedObject(Time.time, touchedObject, physicsEffect);
                    EffectedObjectList.Add(effectedObject);

                    CreateAttachedGameObject(touchedObject, attachedGameObject);
                }
                else if ((EffectedObjectList.Count > 0))
                {
                    bool checkIfExists = false;

                    foreach (EffectedObject item in EffectedObjectList)
                    {
                        if (item.TouchedObject == touchedObject)
                        {
                            item.EffectedTime = Time.time;
                            checkIfExists = true;
                            break;
                        }
                    }

                    if (!checkIfExists)
                    {
                        EffectedObject effectedObject = new EffectedObject(Time.time, touchedObject, physicsEffect);
                        EffectedObjectList.Add(effectedObject);

                        CreateAttachedGameObject(touchedObject, attachedGameObject);
                    }
                }
            }

            //Creates the attached gameobject for the effect
            private static void CreateAttachedGameObject(Rigidbody touchedObject, GameObject attachGameObject)
            {
                if (attachGameObject != null)
                {
                    if (touchedObject.transform.FindChild(SpecialEffectGameObject) == null)
                    {
                        GameObject newAttachGameObject = Instantiate(attachGameObject, touchedObject.gameObject.transform.position, attachGameObject.gameObject.transform.rotation) as GameObject;
                        newAttachGameObject.name = SpecialEffectGameObject;
                        newAttachGameObject.transform.parent = touchedObject.gameObject.transform;
                    }
                }
            }

            //Removes the attached gameobject for the effect
            private static void RemoveAttachedGameObject(Rigidbody touchedObject, GameObject attachGameObject)
            {
                if (touchedObject != null && attachGameObject != null)
                {
                    if (touchedObject.transform.FindChild(SpecialEffectGameObject) != null)
                    {
                        Destroy(touchedObject.transform.FindChild(SpecialEffectGameObject).gameObject);
                    }
                }
            }

            //Refreshs the EffectedObjects over a timer
            public void RefreshEffectedObjectListOverTime(float timer, GameObject attachedGameObject)
            {
                if (EffectedObjectList.Count != 0)
                {
                    List<EffectedObject> removeItems = new List<EffectedObject>();

                    foreach (EffectedObject item in EffectedObjectList)
                    {
                        if (item.EffectedTime + timer < Time.time)
                        {
                            switch (item.physicsEffect)
                            {
                                case PhysicsEffect.None:
                                    break;
                                case PhysicsEffect.NoGravity:
                                    if (item.TouchedObject != null)
                                    {
                                        item.TouchedObject.useGravity = true;
                                    }
                                    break;
                            }

                            RemoveAttachedGameObject(item.TouchedObject, attachedGameObject);
                            removeItems.Add(item);
                        }
                    }

                    //Clears effected items out of the list
                    foreach (var item in removeItems)
                    {
                        EffectedObjectList.Remove(item);
                    }
                }
            }
        }

        //Data structure for effected object
        public class EffectedObject
        {
            public EffectedObject()
            {
            }

            public EffectedObject(float effectedTime, Rigidbody touchedObject, PhysicsEffect physicsEffect)
            {
                this.EffectedTime = effectedTime;
                this.TouchedObject = touchedObject;
                this.physicsEffect = physicsEffect;
            }

            //Time Effected
            private float effectedTime;
            public float EffectedTime
            {
                get { return effectedTime; }
                set { effectedTime = value; }
            }

            //Rigidbody of touched object
            private Rigidbody touchedObject;
            public Rigidbody TouchedObject
            {
                get { return touchedObject; }
                set { touchedObject = value; }
            }

            //Type of effect
            public PhysicsEffect physicsEffect;
        }

        #endregion

        #region Properties/Constructor

        public CGF()
        {
            _forceTypeProperties = new ForceTypeProperties();
            _specialEffect = new SpecialEffect();
            _gameobjectFilter = new GameobjectFilter();
            _tagFilter = new TagFilter();
            _layerFilter = new LayerFilter();
            _triggerAreaFilter = new TriggerAreaFilter();
            _constraintProperties = new ConstraintProperties();
            _constraintProperties._gameobjectFilter = new GameobjectFilter();
            _constraintProperties._tagFilter = new TagFilter();
            _constraintProperties._layerFilter = new LayerFilter();
            _drawGravityProperties = new DrawGravityProperties();
            _memoryProperties = new MemoryProperties();
        }

        //Used for when wanting to see the cgf line
        static private string CirularGravityLineName = "CirularGravityForce_LineDisplay";

        //Used for when creating the GameObject on an effected item
        static private string SpecialEffectGameObject = "CirularGravityForce_SpecialEffect";

#if !(UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
        //Warning message for Requiring Unity 5.3
        static public string WarningMessageNonAllocUnity_5_3 = "3D Non-Alloc Physics Requires Upgrading Project to Unity 5.3 or Higher.";
        static public string WarningMessageBoxUnity_5_3 = "3D Box Shape Physics Requires Upgrading Project to Unity 5.3 or Higher.";
#endif

        [SerializeField, Tooltip("Enable/Disable the Circular Gravity Force.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("Shape of the Cirular Gravity Force.")]
        private Shape shape = CGF.Shape.Sphere;
        public Shape _shape
        {
            get { return shape; }
            set { shape = value; }
        }

        [SerializeField, Tooltip("The force type of the Cirular Gravity Force.")]
        private ForceType forceType = ForceType.ForceAtPosition;
        public ForceType _forceType
        {
            get { return forceType; }
            set { forceType = value; }
        }

        [SerializeField, Tooltip("Option for how to apply a force.")]
        private ForceMode forceMode = ForceMode.Force;
        public ForceMode _forceMode
        {
            get { return forceMode; }
            set { forceMode = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private float size = 5f;
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        [SerializeField, Tooltip("Capsule Radius size of the fore.")]
        private float capsuleRadius = 2f;
        public float CapsuleRadius
        {
            get { return capsuleRadius; }
            set { capsuleRadius = value; }
        }

        [SerializeField, Tooltip("Box vector size of the fore.")]
        private Vector3 boxSize = new Vector3(5f, 5f, 5f);
        public Vector3 BoxSize
        {
            get { return boxSize; }
            set { boxSize = value; }
        }

        [SerializeField, Tooltip("Power for the force, can be negative or positive.")]
        private float forcePower = 10f;
        public float ForcePower
        {
            get { return forcePower; }
            set { forcePower = value; }
        }

        [SerializeField, Tooltip("Force type properties.")]
        private ForceTypeProperties forceTypeProperties;
        public ForceTypeProperties _forceTypeProperties
        {
            get { return forceTypeProperties; }
            set { forceTypeProperties = value; }
        }

        [SerializeField, Tooltip("Constraint properties.")]
        private ConstraintProperties constraintProperties;
        public ConstraintProperties _constraintProperties
        {
            get { return constraintProperties; }
            set { constraintProperties = value; }
        }

        [SerializeField, Tooltip("GameObject filter options.")]
        private GameobjectFilter gameobjectFilter;
        public GameobjectFilter _gameobjectFilter
        {
            get { return gameobjectFilter; }
            set { gameobjectFilter = value; }
        }

        [SerializeField, Tooltip("Tag filter options.")]
        private TagFilter tagFilter;
        public TagFilter _tagFilter
        {
            get { return tagFilter; }
            set { tagFilter = value; }
        }

        [SerializeField, Tooltip("Layer filter options.")]
        private LayerFilter layerFilter;
        public LayerFilter _layerFilter
        {
            get { return layerFilter; }
            set { layerFilter = value; }
        }

        [SerializeField, Tooltip("Trigger area filter options.")]
        private TriggerAreaFilter triggerAreaFilter;
        public TriggerAreaFilter _triggerAreaFilter
        {
            get { return triggerAreaFilter; }
            set { triggerAreaFilter = value; }
        }

        [SerializeField, Tooltip("Special effect options.")]
        private SpecialEffect specialEffect;
        public SpecialEffect _specialEffect
        {
            get { return specialEffect; }
            set { specialEffect = value; }
        }

        [SerializeField, Tooltip("Draw gravity properties.")]
        private DrawGravityProperties drawGravityProperties;
        public DrawGravityProperties _drawGravityProperties
        {
            get { return drawGravityProperties; }
            set { drawGravityProperties = value; }
        }

        [SerializeField, Tooltip("Memory Properties.")]
        private MemoryProperties memoryProperties;
        public MemoryProperties _memoryProperties
        {
            get { return memoryProperties; }
            set { memoryProperties = value; }
        }

        [SerializeField]
        private int colliderListCount;
        public int ColliderListCount
        {
            get { return colliderListCount; }
        }
        [SerializeField]
        private int raycastHitListCount;
        public int RaycastHitListCount
        {
            get { return raycastHitListCount; }
        }

        //Selected Flag
        bool isSelected = false;

        //Line Object
        private GameObject cirularGravityLine;

        //Pre-Allocated Objects
        private Collider[] colliderList;
        private RaycastHit[] raycastHitList;

        #endregion

        #region Gizmos

        //Used for draying icons
        void OnDrawGizmos()
        {

#if (UNITY_EDITOR)
            string icon = "CircularGravityForce Icons/";
            icon = SetupIcons(icon);

            isSelected = CheckGameObjects();

            if (isSelected)
            {
                if (!EditorApplication.isPlaying && (_shape != Shape.Box) && (_shape != Shape.Sphere))
                {
                    _drawGravityProperties.DrawGravityForceGizmos = false;
                }
                else
                {
                    _drawGravityProperties.DrawGravityForceGizmos = true;
                }
            }
            else
            {
                Gizmos.DrawIcon(this.transform.position, icon, true);
                _drawGravityProperties.DrawGravityForceGizmos = true;
            }
#endif
            if (_drawGravityProperties.DrawGravityForceGizmos)
            {
                DrawGravityForceGizmos();
            }
        }

#if (UNITY_EDITOR)
        bool CheckGameObjects()
        {
            if (Selection.activeGameObject == this.gameObject)
                return true;

            foreach (var item in Selection.gameObjects)
            {
                if (item == this.gameObject)
                    return true;
            }

            return false;
        }

        string SetupIcons(string icon)
        {
            string cgfDir = string.Format("{0}/ResurgamStudios/CircularGravityForce Package/Gizmos/CircularGravityForce Icons/", Application.dataPath);
            string dir = string.Format("{0}/Gizmos/CircularGravityForce Icons/", Application.dataPath);

            if (!Directory.Exists(dir))
            {
                if (Directory.Exists(cgfDir))
                {
                    CopyIcons(cgfDir, dir);

                    AssetDatabase.Refresh();
                }
            }

            icon = icon + "cgf_icon";

            if (forcePower == 0 || enable == false)
            {
                icon = icon + "0.png";
            }
            else if (forcePower >= 0)
            {
                icon = icon + "1.png";
            }
            else if (forcePower < 0)
            {
                icon = icon + "2.png";
            }

            return icon;
        }

        //Copys all cgf icons
        void CopyIcons(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir).Where(s => s.EndsWith(".png")))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }
#endif

        #endregion

        #region Unity Events

        void Awake()
        {
            if (_memoryProperties.NonAllocPhysics)
            {
                colliderList = new Collider[_memoryProperties.ColliderBuffer];
                raycastHitList = new RaycastHit[_memoryProperties.RaycastHitBuffer];
            }
        }

        //Update is called once per frame
        void Update()
        {
            if (Enable)
            {
                //Sets up the line that gets rendered showing the area of forces
                if (_drawGravityProperties.DrawGravityForce)
                {
                    if (cirularGravityLine == null)
                    {
                        //Creates line for showing the force
                        cirularGravityLine = new GameObject(CirularGravityLineName);
                        cirularGravityLine.transform.SetParent(this.gameObject.transform, false);
                        cirularGravityLine.AddComponent<LineRenderer>();
                    }
                }
                else
                {
                    if (cirularGravityLine != null)
                    {
                        //Destroys line when not using
                        Destroy(cirularGravityLine);
                    }
                }
            }
            else
            {
                if (cirularGravityLine != null)
                {
                    //Destroys line when not using
                    Destroy(cirularGravityLine);
                }
            }
        }

        //Used for when drawing the cgf line with no lag
        void LateUpdate()
        {
            if (Enable)
            {
                //Sets up the line that gets rendered showing the area of forces
                if (_drawGravityProperties.DrawGravityForce)
                {
                    if (cirularGravityLine != null)
                    {
                        DrawGravityForceLineRenderer();
                    }
                }
            }
        }

        //This function is called every fixed frame
        void FixedUpdate()
        {
            if (Enable && forcePower != 0)
            {
                CalculateAndEstimateForce();
            }

            _specialEffect._effectedObjects.RefreshEffectedObjectListOverTime(_specialEffect.TimeEffected, _specialEffect.AttachedGameObject);
        }

        #endregion

        #region Functions

        //Applys the force function
        private void ApplyForce(Rigidbody rigid, Transform trans)
        {
            switch (_forceType)
            {
                case ForceType.ForceAtPosition:
                    rigid.AddForceAtPosition((rigid.gameObject.transform.position - trans.position) * ForcePower, trans.position, _forceMode);
                    break;
                case ForceType.Force:
                    rigid.AddForce(trans.forward * ForcePower, _forceMode);
                    break;
                case ForceType.Torque:
                    rigid.maxAngularVelocity = _forceTypeProperties.TorqueMaxAngularVelocity;
                    rigid.AddTorque(trans.right * ForcePower, _forceMode);
                    break;
                case ForceType.ExplosionForce:
                    rigid.AddExplosionForce(ForcePower, trans.position, Size, _forceTypeProperties.ExplosionForceUpwardsModifier, _forceMode);
                    break;
                case ForceType.GravitationalAttraction:
                    Vector3 appForce = (rigid.gameObject.transform.position - trans.position).normalized * rigid.mass * ForcePower / (rigid.gameObject.transform.position - trans.position).sqrMagnitude;
                    if (float.IsNaN(appForce.x) && float.IsNaN(appForce.y) && float.IsNaN(appForce.z))
                    {
                        appForce = Vector3.zero;
                    }
                    rigid.AddForce(appForce, _forceMode);
                    break;
            }
        }

        //Calculate and Estimate the force
        private void CalculateAndEstimateForce()
        {
            if (_shape == Shape.Sphere || _shape == Shape.Box)
            {
                #region Sphere

                colliderListCount = 0;

                if (_shape == Shape.Sphere)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                        colliderListCount = Physics.OverlapSphereNonAlloc(this.transform.position, Size, colliderList);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        colliderList = Physics.OverlapSphere(this.transform.position, Size);
                        colliderListCount = colliderList.Length;
                    }
                }
                if (_shape == Shape.Box)
                {
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        colliderListCount = Physics.OverlapBoxNonAlloc(this.transform.position, BoxSize, colliderList, this.transform.rotation);
                    }
                    else
                    {
                        colliderList = Physics.OverlapBox(this.transform.position, BoxSize, this.transform.rotation);
                        colliderListCount = colliderList.Length;
                    }
#else
                    Debug.LogWarning(WarningMessageBoxUnity_5_3);
#endif
                }

                for (int i = 0; i < colliderListCount; i++)
                {
                    if (colliderList[i] != null)
                    {
                        var rigid = colliderList[i].attachedRigidbody;

                        if (rigid != null)
                        {
                            if (ValidateFilters(rigid))
                            {
                                ApplyAlignment(rigid);

                                SetupSpecialEffect(rigid);

                                ApplyForce(rigid, this.transform);
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region RayCast / Capsule

                //Circular Gravity Force Transform
                Transform cgfTran = this.transform;

                raycastHitListCount = 0;

                if (_shape == Shape.Raycast)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                        raycastHitListCount = Physics.RaycastNonAlloc(cgfTran.position, cgfTran.rotation * Vector3.forward, raycastHitList, Size);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        raycastHitList = Physics.RaycastAll(cgfTran.position, cgfTran.rotation * Vector3.forward, Size);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }
                else if (_shape == Shape.Capsule)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                        raycastHitListCount = Physics.CapsuleCastNonAlloc(cgfTran.position, cgfTran.position + ((cgfTran.rotation * Vector3.back)), capsuleRadius, cgfTran.position - ((cgfTran.position + (cgfTran.rotation * (Vector3.back)))), raycastHitList, Size);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        raycastHitList = Physics.CapsuleCastAll(cgfTran.position, cgfTran.position + ((cgfTran.rotation * Vector3.back)), capsuleRadius, cgfTran.position - ((cgfTran.position + (cgfTran.rotation * (Vector3.back)))), Size);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }

                for (int i = 0; i < raycastHitListCount; i++)
                {
                    if (raycastHitList[i].collider != null)
                    {
                        var rigid = raycastHitList[i].collider.attachedRigidbody;

                        if (rigid != null)
                        {
                            if (ValidateFilters(rigid))
                            {
                                ApplyAlignment(rigid);

                                SetupSpecialEffect(rigid);

                                ApplyForce(rigid, this.transform);
                            }
                        }
                    }
                }

                #endregion
            }
        }

        //Applys the alignment of the listed game objects
        private void ApplyAlignment(Rigidbody rigid)
        {
            if (_constraintProperties.AlignToForce)
            {
                if (ValidateAlignToForceFilters(rigid))
                {
                    Vector3 up = (rigid.transform.position - this.transform.position).normalized;
                    Vector3 newLocal = Vector3.zero;

                    switch (_constraintProperties._alignDirection)
                    {
                        case ConstraintProperties.AlignDirection.Up:
                            newLocal = rigid.transform.up;
                            break;
                        case ConstraintProperties.AlignDirection.Down:
                            newLocal = -rigid.transform.up;
                            break;
                        case ConstraintProperties.AlignDirection.Left:
                            newLocal = -rigid.transform.right;
                            break;
                        case ConstraintProperties.AlignDirection.Right:
                            newLocal = rigid.transform.right;
                            break;
                        case ConstraintProperties.AlignDirection.Forward:
                            newLocal = rigid.transform.forward;
                            break;
                        case ConstraintProperties.AlignDirection.Backword:
                            newLocal = -rigid.transform.forward;
                            break;
                    }

                    Quaternion targetRotation = Quaternion.FromToRotation(newLocal, up) * rigid.rotation;
                    rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, targetRotation, Time.deltaTime * _constraintProperties.SlerpSpeed));
                }
            }
        }

        //Applies where filters for alignment
        private bool ValidateAlignToForceFilters(Rigidbody rigid)
        {
            bool value = true;

            switch (_constraintProperties._gameobjectFilter._gameobjectFilterOptions)
            {
                case GameobjectFilter.GameObjectFilterOptions.Disabled:
                    break;
                case GameobjectFilter.GameObjectFilterOptions.OnlyAffectListedGameobjects:
                    value = _constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                case GameobjectFilter.GameObjectFilterOptions.DontAffectListedGameobjects:
                    value = !_constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._tagFilter._tagFilterOptions)
            {
                case TagFilter.TagFilterOptions.Disabled:
                    break;
                case TagFilter.TagFilterOptions.OnlyAffectListedTags:
                    value = _constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                case TagFilter.TagFilterOptions.DontAffectListedTags:
                    value = !_constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._layerFilter._layerFilterOptions)
            {
                case LayerFilter.LayerFilterOptions.Disabled:
                    break;
                case LayerFilter.LayerFilterOptions.OnlyAffectListedLayers:
                    value = _constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                case LayerFilter.LayerFilterOptions.DontAffectListedLayers:
                    value = !_constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                default:
                    break;
            }

            return value;
        }

        private bool ValidateFilters(Rigidbody rigid)
        {
            bool value = true;

            switch (_gameobjectFilter._gameobjectFilterOptions)
            {
                case GameobjectFilter.GameObjectFilterOptions.Disabled:
                    break;
                case GameobjectFilter.GameObjectFilterOptions.OnlyAffectListedGameobjects:
                    value = _gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.gameObject);
                    break;
                case GameobjectFilter.GameObjectFilterOptions.DontAffectListedGameobjects:
                    value = !_gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.gameObject);
                    break;
            }

            //Used for Tag filtering
            switch (_tagFilter._tagFilterOptions)
            {
                case TagFilter.TagFilterOptions.Disabled:
                    break;
                case TagFilter.TagFilterOptions.OnlyAffectListedTags:
                    value = _tagFilter.TagsList.Contains<string>(rigid.tag);
                    break;
                case TagFilter.TagFilterOptions.DontAffectListedTags:
                    value = !_tagFilter.TagsList.Contains<string>(rigid.tag);
                    break;
            }

            //Used for Layer filtering
            switch (_layerFilter._layerFilterOptions)
            {
                case LayerFilter.LayerFilterOptions.Disabled:
                    break;
                case LayerFilter.LayerFilterOptions.OnlyAffectListedLayers:
                    value = _layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.gameObject.layer));
                    break;
                case LayerFilter.LayerFilterOptions.DontAffectListedLayers:
                    value = !_layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.gameObject.layer));
                    break;
            }

            //Used for Trigger Area Filtering
            switch (_triggerAreaFilter._triggerAreaFilterOptions)
            {
                case TriggerAreaFilter.TriggerAreaFilterOptions.Disabled:
                    break;
                case TriggerAreaFilter.TriggerAreaFilterOptions.OnlyAffectWithinTigger:
                    value = _triggerAreaFilter.TriggerArea.bounds.Contains(rigid.transform.position);
                    break;
                case TriggerAreaFilter.TriggerAreaFilterOptions.DontAffectWithinTigger:
                    value = !_triggerAreaFilter.TriggerArea.bounds.Contains(rigid.transform.position);
                    break;
                default:
                    break;
            }

            return value;
        }

        //Sets up the special effects
        private void SetupSpecialEffect(Rigidbody rigidbody)
        {
            switch (_specialEffect._physicsEffect)
            {
                case PhysicsEffect.None:
                    break;
                case PhysicsEffect.NoGravity:
                    rigidbody.useGravity = false;
                    break;
                default:
                    break;
            }

            if (_specialEffect.TimeEffected > 0)
            {
                _specialEffect._effectedObjects.AddedEffectedObject(rigidbody, _specialEffect._physicsEffect, _specialEffect.AttachedGameObject);
            }
        }

        #endregion

        #region Static Functions

        //Static function used to create CGFs
        public static GameObject CreateCGF()
        {
            //Creates empty gameobject.
            GameObject cgf = new GameObject();

            //Sets gameojbect Name
            cgf.name = "CGF";

            //Creates Circular Gravity Force component
            cgf.AddComponent<CGF>()._drawGravityProperties.GravityLineMaterial = new Material(Shader.Find("GUI/Text Shader"));

            return cgf;
        }

        #endregion

        #region Draw

        //Draws effected area by forces line renderer
        private void DrawGravityForceLineRenderer()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            Color DebugGravityLineColor;

            if (Enable)
            {
                if (forcePower == 0)
                    DebugGravityLineColor = Color.white;
                else if (forcePower > 0)
                    DebugGravityLineColor = Color.green;
                else
                    DebugGravityLineColor = Color.red;
            }
            else
            {
                DebugGravityLineColor = Color.white;
            }

            //Line setup
            LineRenderer lineRenderer = cirularGravityLine.GetComponent<LineRenderer>();
            lineRenderer.SetWidth(_drawGravityProperties.Thickness, _drawGravityProperties.Thickness);
            lineRenderer.material = _drawGravityProperties.GravityLineMaterial;
            lineRenderer.material.color = DebugGravityLineColor;

            //Renders type outline
            switch (_shape)
            {
                case Shape.Sphere:

                    //Models line
                    lineRenderer.SetVertexCount(12);

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));
                    lineRenderer.SetPosition(7, cgfTran.position);
                    lineRenderer.SetPosition(8, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size));
                    lineRenderer.SetPosition(9, cgfTran.position);
                    lineRenderer.SetPosition(10, cgfTran.position + ((cgfTran.rotation * Vector3.back) * Size));
                    lineRenderer.SetPosition(11, cgfTran.position);

                    break;

                case Shape.Capsule:

                    //Model Capsule
                    lineRenderer.SetVertexCount(17);

                    //Starting Point
                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(7, cgfTran.position);

                    //Middle Line
                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size);
                    lineRenderer.SetPosition(8, endPointLoc);

                    //End Point
                    lineRenderer.SetPosition(9, endPointLoc + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(10, endPointLoc);
                    lineRenderer.SetPosition(11, endPointLoc + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(12, endPointLoc);
                    lineRenderer.SetPosition(13, endPointLoc + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(14, endPointLoc);
                    lineRenderer.SetPosition(15, endPointLoc + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(16, endPointLoc);

                    break;

                case Shape.Raycast:

                    //Model Line
                    lineRenderer.SetVertexCount(2);

                    lineRenderer.SetPosition(0, cgfTran.position);
                    lineRenderer.SetPosition(1, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size));

                    break;

                case Shape.Box:

                    //Models line
                    lineRenderer.SetVertexCount(12);

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x));
                    lineRenderer.SetPosition(7, cgfTran.position);
                    lineRenderer.SetPosition(8, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * BoxSize.z));
                    lineRenderer.SetPosition(9, cgfTran.position);
                    lineRenderer.SetPosition(10, cgfTran.position + ((cgfTran.rotation * Vector3.back) * BoxSize.z));
                    lineRenderer.SetPosition(11, cgfTran.position);

                    break;
            }
        }

        //Draws effected area by forces with debug draw line, so you can see it in Gizmos
        private void DrawGravityForceGizmos()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            Color DebugGravityLineColorA;
            Color DebugGravityLineColorB;

            if (Enable)
            {
                if (forcePower == 0)
                {
                    DebugGravityLineColorA = Color.white;
                    DebugGravityLineColorB = Color.white;
                }
                else if (forcePower > 0)
                {
                    DebugGravityLineColorA = Color.green;
                    DebugGravityLineColorB = Color.green;
                }
                else
                {
                    DebugGravityLineColorA = Color.red;
                    DebugGravityLineColorB = Color.red;
                }
            }
            else
            {
                DebugGravityLineColorA = Color.white;
                DebugGravityLineColorB = Color.white;
            }

            DebugGravityLineColorA.a = .5f;
            DebugGravityLineColorB.a = .1f;

            //Renders type outline
            switch (_shape)
            {
                case CGF.Shape.Sphere:

                    Gizmos.color = DebugGravityLineColorB;
                    Gizmos.DrawSphere(cgfTran.position, Size);

                    if (!isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.back) * Size), cgfTran.position);
                    }
                    else
                    {
                        Gizmos.color = DebugGravityLineColorB;
                        Gizmos.DrawSphere(cgfTran.position, Size);
                    }


                    break;

                case CGF.Shape.Capsule:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * capsuleRadius), cgfTran.position);

                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size);

                    Gizmos.DrawLine(cgfTran.position, endPointLoc);

                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.up) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.down) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.left) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.right) * capsuleRadius), endPointLoc);

                    break;

                case CGF.Shape.Raycast:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size), cgfTran.position);

                    break;

                case CGF.Shape.Box:

                    Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * 2);
                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                    Gizmos.matrix *= cubeTransform;

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawWireCube(Vector3.zero, BoxSize);

                    if (isSelected)
                        DebugGravityLineColorA.a = 1f;

                    if (isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawWireCube(Vector3.zero, BoxSize);
                    }

                    Gizmos.color = DebugGravityLineColorB;
                    Gizmos.DrawCube(Vector3.zero, BoxSize);

                    if (isSelected)
                        Gizmos.DrawCube(Vector3.zero, BoxSize);

                    Gizmos.matrix = oldGizmosMatrix;

                    if (!isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * BoxSize.z), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.back) * BoxSize.z), cgfTran.position);
                    }

                    break;
            }

            if (memoryProperties.SeeColliders)
            {
                if (_shape == Shape.Sphere || _shape == Shape.Box)
                {
                    if (ForcePower != 0)
                    {
                        for (int i = 0; i < ColliderListCount; i++)
                        {
                            if (isSelected)
                            {
                                Gizmos.color = DebugGravityLineColorA;
                                DebugGravityLineColorA.a = 1f;
                            }
                            Gizmos.DrawLine(this.transform.position, colliderList[i].gameObject.transform.position);
                        }
                    }
                }
            }

            if (memoryProperties.SeeRaycastHits)
            {
                if (_shape == Shape.Capsule || _shape == Shape.Raycast)
                {
                    if (ForcePower != 0)
                    {
                        for (int i = 0; i < RaycastHitListCount; i++)
                        {
                            if (isSelected)
                            {
                                Gizmos.color = DebugGravityLineColorA;
                                DebugGravityLineColorA.a = 1f;
                            }
                            Gizmos.DrawLine(this.transform.position, raycastHitList[i].collider.gameObject.transform.position);
                        }
                    }
                }
            }
        }

        #endregion
    }
}