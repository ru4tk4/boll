/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Core logic for Circular Gravity Force for 2D.
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
    [AddComponentMenu("Physics 2D/Circular Gravity Force 2D", -1)]
    public class CGF2D : MonoBehaviour
    {
        #region Enums

        //Force Types
        public enum ForceType2D
        {
            ForceAtPosition,
            Force,
            Torque,
            GravitationalAttraction
        }

        //Force Types
        public enum Shape2D
        {
            Sphere,
            Raycast,
            Box
        }

        #endregion

        #region Classes

        //Manages all effected objects for when using special effect
        public class EffectedObjects2D
        {
            //List of all EffectedObject
            private List<EffectedObject2D> effectedObjectList;
            public List<EffectedObject2D> EffectedObjectList
            {
                get { return effectedObjectList; }
                set { effectedObjectList = value; }
            }

            //Constructor
            public EffectedObjects2D()
            {
                EffectedObjectList = new List<EffectedObject2D>();
            }

            //Used to add to the effectedObjectList
            public void AddedEffectedObject2D(Rigidbody2D touchedObject, CGF.PhysicsEffect physicsEffect, GameObject attachedGameObject)
            {
                if (EffectedObjectList.Count == 0)
                {
                    EffectedObject2D effectedObject = new EffectedObject2D(Time.time, touchedObject, physicsEffect);
                    EffectedObjectList.Add(effectedObject);

                    CreateAttachedGameObject2D(touchedObject, attachedGameObject);
                }
                else if ((EffectedObjectList.Count > 0))
                {
                    bool checkIfExists = false;

                    foreach (EffectedObject2D item in EffectedObjectList)
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
                        EffectedObject2D effectedObject = new EffectedObject2D(Time.time, touchedObject, physicsEffect);
                        EffectedObjectList.Add(effectedObject);

                        CreateAttachedGameObject2D(touchedObject, attachedGameObject);
                    }
                }
            }

            //Creates the attached gameobject for the effect
            private static void CreateAttachedGameObject2D(Rigidbody2D touchedObject, GameObject attachGameObject)
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
            private static void RemoveAttachedGameObject2D(Rigidbody2D touchedObject, GameObject attachGameObject)
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
                    List<EffectedObject2D> removeItems = new List<EffectedObject2D>();

                    foreach (EffectedObject2D item in EffectedObjectList)
                    {
                        if (item.EffectedTime + timer < Time.time)
                        {
                            switch (item.PhysicsEffect)
                            {
                                case CGF.PhysicsEffect.None:
                                    break;
                                case CGF.PhysicsEffect.NoGravity:
                                    if (item.TouchedObject != null)
                                    {
                                        item.TouchedObject.gravityScale = 1f;
                                    }
                                    break;
                            }

                            RemoveAttachedGameObject2D(item.TouchedObject, attachedGameObject);
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

        [SerializeField, Tooltip("Used for toggling filter options.")]
        private bool toggleFilterOptions;
        public bool ToggleFilterOptions
        {
            get { return toggleFilterOptions; }
            set { toggleFilterOptions = value; }
        }

        //Manages all effected objects for when using special effect in 2D
        [System.Serializable]
        public class SpecialEffect2D
        {
            [SerializeField, Tooltip("Physics effect options.")]
            private CGF.PhysicsEffect physicsEffect = CGF.PhysicsEffect.None;
            public CGF.PhysicsEffect _physicsEffect
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
            private EffectedObjects2D effectedObjects;
            public EffectedObjects2D _effectedObjects
            {
                get { return effectedObjects; }
                set { effectedObjects = value; }
            }

            //Constructor
            public SpecialEffect2D()
            {
                _effectedObjects = new EffectedObjects2D();
            }
        }

        //Trigger Area Filter
        [System.Serializable]
        public class TriggerAreaFilter2D
        {
            //Trigger Options
            public enum TriggerAreaFilterOptions2D
            {
                Disabled,
                OnlyAffectWithinTigger,
                DontAffectWithinTigger,
            }

            [SerializeField, Tooltip("Trigger area filter options.")]
            private TriggerAreaFilterOptions2D triggerAreaFilterOptions = TriggerAreaFilterOptions2D.Disabled;
            public TriggerAreaFilterOptions2D _triggerAreaFilterOptions
            {
                get { return triggerAreaFilterOptions; }
                set { triggerAreaFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed triggers used for the filter.")]
            private Collider2D triggerArea;
            public Collider2D TriggerArea
            {
                get { return triggerArea; }
                set { triggerArea = value; }
            }
        }

        //Data structure for effected object
        public class EffectedObject2D
        {
            public EffectedObject2D()
            {
            }

            public EffectedObject2D(float effectedTime, Rigidbody2D touchedObject, CGF.PhysicsEffect physicsEffect)
            {
                this.EffectedTime = effectedTime;
                this.TouchedObject = touchedObject;
                this.PhysicsEffect = physicsEffect;
            }

            //Time Effected
            private float effectedTime;
            public float EffectedTime
            {
                get { return effectedTime; }
                set { effectedTime = value; }
            }

            //Rigidbody of touched object
            private Rigidbody2D touchedObject;
            public Rigidbody2D TouchedObject
            {
                get { return touchedObject; }
                set { touchedObject = value; }
            }

            //Type of effect
            private CGF.PhysicsEffect physicsEffect;
            public CGF.PhysicsEffect PhysicsEffect
            {
                get { return physicsEffect; }
                set { physicsEffect = value; }
            }
        }

        #endregion

        #region Properties/Constructor

        public CGF2D()
        {
            _specialEffect = new SpecialEffect2D();
            _gameobjectFilter = new CGF.GameobjectFilter();
            _tagFilter = new CGF.TagFilter();
            _layerFilter = new CGF.LayerFilter();
            _triggerAreaFilter = new TriggerAreaFilter2D();
            _constraintProperties = new CGF.ConstraintProperties();
            _constraintProperties._gameobjectFilter = new CGF.GameobjectFilter();
            _constraintProperties._tagFilter = new CGF.TagFilter();
            _constraintProperties._layerFilter = new CGF.LayerFilter();
            _drawGravityProperties = new CGF.DrawGravityProperties();
            _memoryProperties = new CGF.MemoryProperties();
        }

        //Used for when wanting to see the cgf line
        static private string CirularGravityLineName = "CirularGravityForce_LineDisplay";

        //Used for when creating the GameObject on an effected item
        static private string SpecialEffectGameObject = "CirularGravityForce_SpecialEffect";

#if !(UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
        //Warning message for Requiring Unity 5.3
        static private string WarningMessageBoxUnity_5_3 = "2D Box Shape Physics Requires Upgrading Project to Unity 5.3 or Higher.";
#endif

        [SerializeField, Tooltip("Enable/Disable the Circular Gravity Force.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("Shape of the Cirular Gravity Force.")]
        private Shape2D shape2D = Shape2D.Sphere;
        public Shape2D _shape2D
        {
            get { return shape2D; }
            set { shape2D = value; }
        }

        [SerializeField, Tooltip("The force type of the Cirular Gravity Force.")]
        private ForceType2D forceType2D = ForceType2D.ForceAtPosition;
        public ForceType2D _forceType2D
        {
            get { return forceType2D; }
            set { forceType2D = value; }
        }

        [SerializeField, Tooltip("Option for how to apply a force.")]
        private ForceMode2D forceMode2D = ForceMode2D.Force;
        public ForceMode2D _forceMode2D
        {
            get { return forceMode2D; }
            set { forceMode2D = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private float size = 5f;
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private Vector2 boxSize = Vector2.one * 5f;
        public Vector2 BoxSize
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

        [SerializeField, Tooltip("Constraint properties.")]
        private CGF.ConstraintProperties constraintProperties;
        public CGF.ConstraintProperties _constraintProperties
        {
            get { return constraintProperties; }
            set { constraintProperties = value; }
        }

        [SerializeField, Tooltip("GameObject filter options.")]
        private CGF.GameobjectFilter gameobjectFilter;
        public CGF.GameobjectFilter _gameobjectFilter
        {
            get { return gameobjectFilter; }
            set { gameobjectFilter = value; }
        }

        [SerializeField, Tooltip("Tag filter options.")]
        private CGF.TagFilter tagFilter;
        public CGF.TagFilter _tagFilter
        {
            get { return tagFilter; }
            set { tagFilter = value; }
        }

        [SerializeField, Tooltip("Layer filter options.")]
        private CGF.LayerFilter layerFilter;
        public CGF.LayerFilter _layerFilter
        {
            get { return layerFilter; }
            set { layerFilter = value; }
        }

        [SerializeField, Tooltip("Trigger area filter options.")]
        private TriggerAreaFilter2D triggerAreaFilter;
        public TriggerAreaFilter2D _triggerAreaFilter
        {
            get { return triggerAreaFilter; }
            set { triggerAreaFilter = value; }
        }

        [SerializeField, Tooltip("Special effect options.")]
        private SpecialEffect2D specialEffect;
        public SpecialEffect2D _specialEffect
        {
            get { return specialEffect; }
            set { specialEffect = value; }
        }

        [SerializeField, Tooltip("Draw gravity properties.")]
        private CGF.DrawGravityProperties drawGravityProperties;
        public CGF.DrawGravityProperties _drawGravityProperties
        {
            get { return drawGravityProperties; }
            set { drawGravityProperties = value; }
        }

        [SerializeField, Tooltip("Memory Properties.")]
        private CGF.MemoryProperties memoryProperties;
        public CGF.MemoryProperties _memoryProperties
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

        //Effected Rigidbodys
        Collider2D[] colliderList;
        RaycastHit2D[] raycastHitList;

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
                if (!EditorApplication.isPlaying && (_shape2D != Shape2D.Box))
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
            else if (ForcePower < 0)
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
                colliderList = new Collider2D[_memoryProperties.ColliderBuffer];
                raycastHitList = new RaycastHit2D[_memoryProperties.RaycastHitBuffer];
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
            if (Enable && ForcePower != 0)
            {
                CalculateAndEstimateForce();
            }

            _specialEffect._effectedObjects.RefreshEffectedObjectListOverTime(_specialEffect.TimeEffected, _specialEffect.AttachedGameObject);
        }

        #endregion

        #region Functions

        //Applys the force function
        private void ApplyForce(Rigidbody2D rigid, Transform trans)
        {
            switch (_forceType2D)
            {
                case ForceType2D.ForceAtPosition:
                    rigid.AddForceAtPosition((rigid.gameObject.transform.position - trans.position) * ForcePower, trans.position, _forceMode2D);
                    break;
                case ForceType2D.Force:
                    rigid.AddForce(trans.right * ForcePower, _forceMode2D);
                    break;
                case ForceType2D.Torque:
                    rigid.AddTorque(-ForcePower, _forceMode2D);
                    break;
                case ForceType2D.GravitationalAttraction:
                    Vector3 appForce = (rigid.gameObject.transform.position - trans.position).normalized * rigid.mass * ForcePower / (rigid.gameObject.transform.position - trans.position).sqrMagnitude;
                    if (float.IsNaN(appForce.x) && float.IsNaN(appForce.y) && float.IsNaN(appForce.z))
                    {
                        appForce = Vector3.zero;
                    }
                    rigid.AddForce(appForce, _forceMode2D);
                    break;
            }
        }

        //Calculate and Estimate the force
        private void CalculateAndEstimateForce()
        {
            if (_shape2D == Shape2D.Sphere)
            {
                #region Sphere

                colliderListCount = 0;

                if (_shape2D == Shape2D.Sphere)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        colliderListCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, Size, colliderList);
                    }
                    else
                    {
                        colliderList = Physics2D.OverlapCircleAll(this.transform.position, Size);
                        colliderListCount = colliderList.Length;
                    }
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
                #region RayCast

                //Circular Gravity Force Transform
                Transform cgfTran = this.transform;

                raycastHitListCount = 0;

                if (_shape2D == Shape2D.Raycast)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        raycastHitListCount = Physics2D.RaycastNonAlloc(cgfTran.position, cgfTran.rotation * Vector3.right, raycastHitList, Size);
                    }
                    else
                    {
                        raycastHitList = Physics2D.RaycastAll(cgfTran.position, cgfTran.rotation * Vector3.right, Size);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }

                if (_shape2D == Shape2D.Box)
                {
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9)
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        raycastHitListCount = Physics2D.BoxCastNonAlloc(this.transform.position, new Vector2(boxSize.x * 2, boxSize.y * 2), this.transform.eulerAngles.z, Vector3.zero, raycastHitList);
                    }
                    else
                    {
                        raycastHitList = Physics2D.BoxCastAll(this.transform.position, new Vector2(boxSize.x * 2, boxSize.y * 2), this.transform.eulerAngles.z, Vector3.zero);
                        raycastHitListCount = raycastHitList.Length;
                    }
#else
                    Debug.LogWarning(WarningMessageBoxUnity_5_3);
#endif
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

        //Applies where filters for alignment
        private bool ValidateAlignToForceFilters(Rigidbody rigid)
        {
            bool value = true;

            switch (_constraintProperties._gameobjectFilter._gameobjectFilterOptions)
            {
                case CGF.GameobjectFilter.GameObjectFilterOptions.Disabled:
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.OnlyAffectListedGameobjects:
                    value = _constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.DontAffectListedGameobjects:
                    value = !_constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._tagFilter._tagFilterOptions)
            {
                case CGF.TagFilter.TagFilterOptions.Disabled:
                    break;
                case CGF.TagFilter.TagFilterOptions.OnlyAffectListedTags:
                    value = _constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                case CGF.TagFilter.TagFilterOptions.DontAffectListedTags:
                    value = !_constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._layerFilter._layerFilterOptions)
            {
                case CGF.LayerFilter.LayerFilterOptions.Disabled:
                    break;
                case CGF.LayerFilter.LayerFilterOptions.OnlyAffectListedLayers:
                    value = _constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                case CGF.LayerFilter.LayerFilterOptions.DontAffectListedLayers:
                    value = !_constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                default:
                    break;
            }

            return value;
        }

        //Applys the alignment of the listed game objects
        private void ApplyAlignment(Rigidbody2D rigid)
        {
            if (_constraintProperties.AlignToForce)
            {
                if (ValidateAlignToForceFilters(rigid))
                {
                    Vector3 newLocal = Vector3.zero;

                    switch (_constraintProperties._alignDirection)
                    {
                        case CGF.ConstraintProperties.AlignDirection.Up:
                            newLocal = -rigid.transform.forward;
                            break;
                        case CGF.ConstraintProperties.AlignDirection.Down:
                            newLocal = rigid.transform.forward;
                            break;
                        case CGF.ConstraintProperties.AlignDirection.Left:
                            newLocal = -rigid.transform.right;
                            break;
                        case CGF.ConstraintProperties.AlignDirection.Right:
                            newLocal = rigid.transform.right;
                            break;
                        case CGF.ConstraintProperties.AlignDirection.Forward:
                            newLocal = -rigid.transform.up;
                            break;
                        case CGF.ConstraintProperties.AlignDirection.Backword:
                            newLocal = rigid.transform.up;
                            break;
                    }

                    Quaternion newRotation = Quaternion.LookRotation((rigid.transform.position - this.transform.position).normalized, newLocal);
                    newRotation.x = 0.0f;
                    newRotation.y = 0.0f;
                    rigid.transform.rotation = Quaternion.Slerp(rigid.transform.rotation, newRotation, Time.deltaTime * _constraintProperties.SlerpSpeed);
                }
            }
        }

        //Applies where filters for alignment
        private bool ValidateAlignToForceFilters(Rigidbody2D rigid)
        {
            bool value = true;

            switch (_constraintProperties._gameobjectFilter._gameobjectFilterOptions)
            {
                case CGF.GameobjectFilter.GameObjectFilterOptions.Disabled:
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.OnlyAffectListedGameobjects:
                    value = _constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.DontAffectListedGameobjects:
                    value = !_constraintProperties._gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.transform.gameObject);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._tagFilter._tagFilterOptions)
            {
                case CGF.TagFilter.TagFilterOptions.Disabled:
                    break;
                case CGF.TagFilter.TagFilterOptions.OnlyAffectListedTags:
                    value = _constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                case CGF.TagFilter.TagFilterOptions.DontAffectListedTags:
                    value = !_constraintProperties._tagFilter.TagsList.Contains<string>(rigid.transform.gameObject.tag);
                    break;
                default:
                    break;
            }

            switch (_constraintProperties._layerFilter._layerFilterOptions)
            {
                case CGF.LayerFilter.LayerFilterOptions.Disabled:
                    break;
                case CGF.LayerFilter.LayerFilterOptions.OnlyAffectListedLayers:
                    value = _constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                case CGF.LayerFilter.LayerFilterOptions.DontAffectListedLayers:
                    value = !_constraintProperties._layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.transform.gameObject.layer));
                    break;
                default:
                    break;
            }

            return value;
        }

        private bool ValidateFilters(Rigidbody2D rigid)
        {
            bool value = true;

            switch (_gameobjectFilter._gameobjectFilterOptions)
            {
                case CGF.GameobjectFilter.GameObjectFilterOptions.Disabled:
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.OnlyAffectListedGameobjects:
                    value = _gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.gameObject);
                    break;
                case CGF.GameobjectFilter.GameObjectFilterOptions.DontAffectListedGameobjects:
                    value = !_gameobjectFilter.GameobjectList.Contains<GameObject>(rigid.gameObject);
                    break;
            }

            //Used for Tag filtering
            switch (_tagFilter._tagFilterOptions)
            {
                case CGF.TagFilter.TagFilterOptions.Disabled:
                    break;
                case CGF.TagFilter.TagFilterOptions.OnlyAffectListedTags:
                    value = _tagFilter.TagsList.Contains<string>(rigid.tag);
                    break;
                case CGF.TagFilter.TagFilterOptions.DontAffectListedTags:
                    value = !_tagFilter.TagsList.Contains<string>(rigid.tag);
                    break;
            }

            //Used for Layer filtering
            switch (_layerFilter._layerFilterOptions)
            {
                case CGF.LayerFilter.LayerFilterOptions.Disabled:
                    break;
                case CGF.LayerFilter.LayerFilterOptions.OnlyAffectListedLayers:
                    value = _layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.gameObject.layer));
                    break;
                case CGF.LayerFilter.LayerFilterOptions.DontAffectListedLayers:
                    value = !_layerFilter.LayerList.Contains<string>(LayerMask.LayerToName(rigid.gameObject.layer));
                    break;
            }

            //Used for Trigger Area Filtering
            switch (_triggerAreaFilter._triggerAreaFilterOptions)
            {
                case TriggerAreaFilter2D.TriggerAreaFilterOptions2D.Disabled:
                    break;
                case TriggerAreaFilter2D.TriggerAreaFilterOptions2D.OnlyAffectWithinTigger:
                    value = _triggerAreaFilter.TriggerArea.bounds.Contains(rigid.transform.position);
                    break;
                case TriggerAreaFilter2D.TriggerAreaFilterOptions2D.DontAffectWithinTigger:
                    value = !_triggerAreaFilter.TriggerArea.bounds.Contains(rigid.transform.position);
                    break;
                default:
                    break;
            }

            return value;
        }

        //Sets up the special effects
        private void SetupSpecialEffect(Rigidbody2D rigidbody)
        {
            switch (_specialEffect._physicsEffect)
            {
                case CGF.PhysicsEffect.None:
                    break;
                case CGF.PhysicsEffect.NoGravity:
                    rigidbody.gravityScale = 0f;
                    break;
                default:
                    break;
            }

            if (_specialEffect.TimeEffected > 0)
            {
                _specialEffect._effectedObjects.AddedEffectedObject2D(rigidbody, _specialEffect._physicsEffect, _specialEffect.AttachedGameObject);
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
            cgf.name = "CGF 2D";

            //Creates Circular Gravity Force component
            cgf.AddComponent<CGF2D>()._drawGravityProperties.GravityLineMaterial = new Material(Shader.Find("GUI/Text Shader"));

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
                if (ForcePower == 0)
                    DebugGravityLineColor = Color.white;
                else if (ForcePower > 0)
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
            switch (_shape2D)
            {
                case Shape2D.Sphere:

                    //Models line
                    lineRenderer.SetVertexCount(8);

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));
                    lineRenderer.SetPosition(7, cgfTran.position);

                    break;

                case Shape2D.Raycast:

                    //Model Line
                    lineRenderer.SetVertexCount(2);

                    lineRenderer.SetPosition(0, cgfTran.position);
                    lineRenderer.SetPosition(1, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));

                    break;


                case Shape2D.Box:

                    //Models line
                    lineRenderer.SetVertexCount(8);

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x));
                    lineRenderer.SetPosition(7, cgfTran.position);

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
            switch (_shape2D)
            {
                case Shape2D.Sphere:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);

                    break;

                case Shape2D.Raycast:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);

                    break;

                case Shape2D.Box:

                    Vector3 BoxSize = new Vector3(boxSize.x, boxSize.y, 0f);

                    Gizmos.color = DebugGravityLineColorA;
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
                    }

                    break;
            }

            if (memoryProperties.SeeColliders)
            {
                if (_shape2D == Shape2D.Sphere)
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
                if (shape2D == Shape2D.Raycast || _shape2D == Shape2D.Box)
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