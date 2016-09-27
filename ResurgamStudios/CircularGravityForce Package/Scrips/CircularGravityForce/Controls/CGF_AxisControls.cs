/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for when wanting to setup axis controls for the cgf gameobject.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF))]
    public class CGF_AxisControls : MonoBehaviour
    {
        #region Classis

        [System.Serializable]
        public class EnableControls
        {
            [SerializeField, Tooltip("Input manager button name.")]
            private string buttonName;
            public string ButtonName
            {
                get { return buttonName; }
                set { buttonName = value; }
            }

            [SerializeField, Tooltip("Press button value for enable of the circular gravity force.")]
            private bool pressValue;
            public bool PressValue
            {
                get { return pressValue; }
                set { pressValue = value; }
            }
        }

        [System.Serializable]
        public class SizeControls
        {
            [SerializeField, Tooltip("Input manager button name.")]
            private string buttonName;
            public string ButtonName
            {
                get { return buttonName; }
                set { buttonName = value; }
            }

            [SerializeField, Tooltip("Press button value for size of the circular gravity force.")]
            private float pressValue;
            public float PressValue
            {
                get { return pressValue; }
                set { pressValue = value; }
            }
        }

        [System.Serializable]
        public class BoxSizeControls
        {
            [SerializeField, Tooltip("Input manager button name.")]
            private string buttonName;
            public string ButtonName
            {
                get { return buttonName; }
                set { buttonName = value; }
            }

            [SerializeField, Tooltip("Press button value for box size of the circular gravity force.")]
            private Vector3 pressValue;
            public Vector3 PressValue
            {
                get { return pressValue; }
                set { pressValue = value; }
            }
        }

        [System.Serializable]
        public class ForcePowerControls
        {
            [SerializeField, Tooltip("Input manager button name.")]
            private string buttonName;
            public string ButtonName
            {
                get { return buttonName; }
                set { buttonName = value; }
            }

            [SerializeField, Tooltip("Press button value for force power of the circular gravity force.")]
            private float pressValue;
            public float PressValue
            {
                get { return pressValue; }
                set { pressValue = value; }
            }
        }

        [System.Serializable]
        public class EnableControler
        {
            [SerializeField, Tooltip("Idle value for enable of the circular gravity force.")]
            private bool idleValue = false;
            public bool IdleValue
            {
                get { return idleValue; }
                set { idleValue = value; }
            }

            [SerializeField, Tooltip("Enable controls.")]
            private List<EnableControls> enableControls;
            public List<EnableControls> _enableControls
            {
                get { return enableControls; }
                set { enableControls = value; }
            }
        }
        [System.Serializable]
        public class ForceControler
        {
            [SerializeField, Tooltip("Idle value for force power of the circular gravity force.")]
            private float idleValue = 0f;
            public float IdleValue
            {
                get { return idleValue; }
                set { idleValue = value; }
            }

            [SerializeField, Tooltip("Force power controls.")]
            private List<ForcePowerControls> forcePowerControls;
            public List<ForcePowerControls> _forcePowerControls
            {
                get { return forcePowerControls; }
                set { forcePowerControls = value; }
            }
        }
        [System.Serializable]
        public class SizeControler
        {
            [SerializeField, Tooltip("Idle value for size of the circular gravity force.")]
            private float idleValue = 0f;
            public float IdleValue
            {
                get { return idleValue; }
                set { idleValue = value; }
            }

            [SerializeField, Tooltip("Size controls.")]
            private List<SizeControls> sizeControls;
            public List<SizeControls> _sizeControls
            {
                get { return sizeControls; }
                set { sizeControls = value; }
            }
        }
        [System.Serializable]
        public class BoxSizeControler
        {
            [SerializeField, Tooltip("Idle value for size of the circular gravity force.")]
            private Vector3 idleValue = Vector3.zero;
            public Vector3 IdleValue
            {
                get { return idleValue; }
                set { idleValue = value; }
            }

            [SerializeField, Tooltip("Size controls.")]
            private List<BoxSizeControls> boxSizeControls;
            public List<BoxSizeControls> _boxSizeControls
            {
                get { return boxSizeControls; }
                set { boxSizeControls = value; }
            }
        }

        #endregion

        #region Properties/Constructor

        public CGF_AxisControls()
        {
            _enableControler = new EnableControler();
            _enableControler._enableControls = new List<EnableControls>();
            _forceControler = new ForceControler();
            _forceControler._forcePowerControls = new List<ForcePowerControls>();
            _sizeControler = new SizeControler();
            _sizeControler._sizeControls = new List<SizeControls>();
            _boxSizeControler = new BoxSizeControler();
            _boxSizeControler._boxSizeControls = new List<BoxSizeControls>();
        }

        [SerializeField, Tooltip("Input contros that enable the circular gravity force.")]
        private EnableControler enableControler;
        public EnableControler _enableControler
        {
            get { return enableControler; }
            set { enableControler = value; }
        }
        
        [SerializeField, Tooltip("Input contros that force power the circular gravity force.")]
        private ForceControler forceControler;
        public ForceControler _forceControler
        {
            get { return forceControler; }
            set { forceControler = value; }
        }
        [SerializeField, Tooltip("Input contros that size the circular gravity force.")]
        private SizeControler sizeControler;
        public SizeControler _sizeControler
        {
            get { return sizeControler; }
            set { sizeControler = value; }
        }
        [SerializeField, Tooltip("Input contros that box size the circular gravity force.")]
        private BoxSizeControler boxSizeControler;
        public BoxSizeControler _boxSizeControler
        {
            get { return boxSizeControler; }
            set { boxSizeControler = value; }
        }

        private CGF cgf;

        bool flagEnableControl = false;
        bool flagPowerControl = false;
        bool flagSizeControl = false;
        bool flagBoxSizeControl = false;

        #endregion

        #region Unity Functions

        void Awake()
        {
            cgf = this.GetComponent<CGF>();

            SetIdleValues();
        }

        void Update()
        {
			flagEnableControl = false;
			foreach (var enableControl in _enableControler._enableControls) 
			{
                if (enableControl.ButtonName != string.Empty) 
				{
                    if (Input.GetButton(enableControl.ButtonName)) 
					{
						cgf.Enable = enableControl.PressValue;
						flagEnableControl = true;
					}
				}
			}

            flagPowerControl = false;
            foreach (var forcePowerControl in _forceControler._forcePowerControls)
            {
                if (forcePowerControl.ButtonName != string.Empty)
                {
                    if (Input.GetButton(forcePowerControl.ButtonName))
                    {
                        cgf.ForcePower = Input.GetAxis(forcePowerControl.ButtonName) * forcePowerControl.PressValue;
                        flagPowerControl = true;
                    }
                }
            }

            flagSizeControl = false;
			foreach (var sizeControl in _sizeControler._sizeControls) 
			{
                if (sizeControl.ButtonName != string.Empty) 
				{
                    if (Input.GetButton(sizeControl.ButtonName)) 
					{
                        cgf.Size = Input.GetAxis(sizeControl.ButtonName) * sizeControl.PressValue;
						flagSizeControl = true;
					}
				}
			}

            flagBoxSizeControl = false;
            foreach (var boxSizeControl in _boxSizeControler._boxSizeControls)
            {
                if (boxSizeControl.ButtonName != string.Empty)
                {
                    if (Input.GetButton(boxSizeControl.ButtonName))
                    {
                        cgf.BoxSize = Input.GetAxis(boxSizeControl.ButtonName) * boxSizeControl.PressValue;
                        flagBoxSizeControl = true;
                    }
                }
            }

            SetIdleValues();
        }

        #endregion

		#region Functions

        void SetIdleValues()
        {
            if (!flagEnableControl && _enableControler._enableControls.Count != 0)
            {
                cgf.Enable = _enableControler.IdleValue;
            }

            if (!flagPowerControl && _forceControler._forcePowerControls.Count != 0)
            {
                cgf.ForcePower = _forceControler.IdleValue;
            }

            if (!flagSizeControl && _sizeControler._sizeControls.Count != 0)
            {
                cgf.Size = _sizeControler.IdleValue;
            }

            if (!flagBoxSizeControl && _boxSizeControler._boxSizeControls.Count != 0)
            {
                cgf.BoxSize = _boxSizeControler.IdleValue;
            }
        }

		#endregion
    }
}