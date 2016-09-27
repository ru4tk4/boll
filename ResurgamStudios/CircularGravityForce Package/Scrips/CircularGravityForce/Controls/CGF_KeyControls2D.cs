/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for when wanting to setup key controls for the cgf gameobject in 2D.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF2D))]
    public class CGF_KeyControls2D : MonoBehaviour
    {
        #region Enums
        
        public enum CGFControls
		{
			Enable,
		}

        #endregion

        #region Classis

        [System.Serializable]
        public class EnableControls
        {
            [SerializeField, Tooltip("Input keycode.")]
            private KeyCode keyCode;
            public KeyCode _keyCode
            {
                get { return keyCode; }
                set { keyCode = value; }
            }

            [SerializeField, Tooltip("Press keycode value for enable of the circular gravity force.")]
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
            [SerializeField, Tooltip("Input keycode.")]
            private KeyCode keyCode;
            public KeyCode _keyCode
            {
                get { return keyCode; }
                set { keyCode = value; }
            }

            [SerializeField, Tooltip("Press keycode value for size of the circular gravity force.")]
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
            [SerializeField, Tooltip("Input keycode.")]
            private KeyCode keyCode;
            public KeyCode _keyCode
            {
                get { return keyCode; }
                set { keyCode = value; }
            }

            [SerializeField, Tooltip("Press keycode value for box size of the circular gravity force.")]
            private Vector2 pressValue;
            public Vector2 PressValue
            {
                get { return pressValue; }
                set { pressValue = value; }
            }
        }

        [System.Serializable]
        public class ForcePowerControls
        {
            [SerializeField, Tooltip("Input keycode.")]
            private KeyCode keyCode;
            public KeyCode _keyCode
            {
                get { return keyCode; }
                set { keyCode = value; }
            }

            [SerializeField, Tooltip("Press keycode value for force power of the circular gravity force.")]
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
            private Vector2 idleValue = Vector2.zero;
            public Vector2 IdleValue
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

        public CGF_KeyControls2D()
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

        private CGF2D cgf;

		bool flagEnableControl = false;
		bool flagSizeControl = false;
		bool flagPowerControl = false;
        bool flagBoxSizeControl = false;

        #endregion

        #region Unity Functions

        void Awake()
        {
            cgf = this.GetComponent<CGF2D>();

			SetIdleValues();
        }

        void Update()
        {
			flagEnableControl = false;
			foreach (var enableControl in _enableControler._enableControls) 
			{
				if (enableControl._keyCode != KeyCode.None) 
				{
					if (Input.GetKey (enableControl._keyCode)) 
					{
						cgf.Enable = enableControl.PressValue;
						flagEnableControl = true;
					}
				}
			}

			flagSizeControl = false;
			foreach (var sizeControl in _sizeControler._sizeControls) 
			{
				if (sizeControl._keyCode != KeyCode.None) 
				{
					if (Input.GetKey (sizeControl._keyCode)) 
					{
						cgf.Size = sizeControl.PressValue;
						flagSizeControl = true;
					}
				}
			}

			flagPowerControl = false;
			foreach (var forcePowerControl in _forceControler._forcePowerControls) 
			{
				if (forcePowerControl._keyCode != KeyCode.None) 
				{
					if (Input.GetKey (forcePowerControl._keyCode)) 
					{
						cgf.ForcePower = forcePowerControl.PressValue;
						flagPowerControl = true;
					}
				}
			}

            flagBoxSizeControl = false;
            foreach (var boxSizeControl in _boxSizeControler._boxSizeControls)
            {
                if (boxSizeControl._keyCode != KeyCode.None)
                {
                    if (Input.GetKey(boxSizeControl._keyCode))
                    {
                        cgf.BoxSize = boxSizeControl.PressValue;
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
			
			if (!flagSizeControl && _sizeControler._sizeControls.Count != 0)
			{
				cgf.Size = _sizeControler.IdleValue;
			}
			
			if (!flagPowerControl && _forceControler._forcePowerControls.Count != 0)
			{
				cgf.ForcePower = _forceControler.IdleValue;
			}

            if (!flagBoxSizeControl && _boxSizeControler._boxSizeControls.Count != 0)
            {
                cgf.BoxSize = _boxSizeControler.IdleValue;
            }
        }

        #endregion
    }
}