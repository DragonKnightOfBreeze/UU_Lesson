//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR

#endif

/// <summary>
///     This script should be attached to each camera that's used to draw the objects with UI components on them. This
///     may mean only one camera (main camera or your UI camera), or multiple cameras if you happen to have multiple
///     viewports. Failing to attach this script simply means that objects drawn by this camera won't receive UI
///     notifications: * OnHover (isOver) is sent when the mouse hovers over a collider or moves away. * OnPress (isDown)
///     is sent when a mouse button gets pressed on the collider. * OnSelect (selected) is sent when a mouse button is
///     first pressed on an object. Repeated presses won't result in an OnSelect(true). * OnClick () is sent when a mouse
///     is pressed and released on the same object. UICamera.currentTouchID tells you which button was clicked. *
///     OnDoubleClick () is sent when the click happens twice within a fourth of a second. UICamera.currentTouchID tells
///     you which button was clicked. * OnDragStart () is sent to a game object under the touch just before the OnDrag()
///     notifications begin. * OnDrag (delta) is sent to an object that's being dragged. * OnDragOver (draggedObject) is
///     sent to a game object when another object is dragged over its area. * OnDragOut (draggedObject) is sent to a game
///     object when another object is dragged out of its area. * OnDragEnd () is sent to a dragged object when the drag
///     event finishes. * OnTooltip (show) is sent when the mouse hovers over a collider for some time without moving. *
///     OnScroll (float delta) is sent out when the mouse scroll wheel is moved. * OnNavigate (KeyCode key) is sent when
///     horizontal or vertical navigation axes are moved. * OnPan (Vector2 delta) is sent when when horizontal or vertical
///     panning axes are moved. * OnKey (KeyCode key) is sent when keyboard or controller input is used.
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour {
	public enum ControlScheme {
		Mouse,
		Touch,
		Controller
	}

	/// <summary>Whether the touch event will be sending out the OnClick notification at the end.</summary>
	public enum ClickNotification {
		None,
		Always,
		BasedOnDelta
	}

	/// <summary>Ambiguous mouse, touch, or controller event.</summary>
	public class MouseOrTouch {
		public KeyCode key = KeyCode.None;
		public Vector2 pos;        // Current position of the mouse or touch event
		public Vector2 lastPos;    // Previous position of the mouse or touch event
		public Vector2 delta;      // Delta since last update
		public Vector2 totalDelta; // Delta since the event started being tracked

		public Camera pressedCam; // Camera that the OnPress(true) was fired with

		public GameObject last;    // Last object under the touch or mouse
		public GameObject current; // Current game object under the touch or mouse
		public GameObject pressed; // Last game object to receive OnPress
		public GameObject dragged; // Game object that's being dragged

		public float pressTime; // When the touch event started
		public float clickTime; // The last time a click event was sent out

		public ClickNotification clickNotification = ClickNotification.Always;
		public bool touchBegan = true;
		public bool pressStarted;
		public bool dragStarted;
		public int ignoreDelta;

		/// <summary>Delta time since the touch operation started.</summary>

		public float deltaTime => RealTime.time - pressTime;

		/// <summary>Returns whether this touch is currently over a UI element.</summary>

		public bool isOverUI => current != null && current != fallThrough && NGUITools.FindInParents<UIRoot>(current) != null;
	}

	/// <summary>Camera type controls how raycasts are handled by the UICamera.</summary>
	public enum EventType {
		World_3D, // Perform a Physics.Raycast and sort by distance to the point that was hit.
		UI_3D,    // Perform a Physics.Raycast and sort by widget depth.
		World_2D, // Perform a Physics2D.OverlapPoint
		UI_2D     // Physics2D.OverlapPoint then sort by widget depth
	}

	/// <summary>List of all active cameras in the scene.</summary>
	public static BetterList<UICamera> list = new BetterList<UICamera>();

	public delegate bool GetKeyStateFunc(KeyCode key);

	public delegate float GetAxisFunc(string name);

	public delegate bool GetAnyKeyFunc();

	public delegate MouseOrTouch GetMouseDelegate(int button);

	public delegate MouseOrTouch GetTouchDelegate(int id, bool createIfMissing);

	public delegate void RemoveTouchDelegate(int id);

	/// <summary>GetKeyDown function -- return whether the specified key was pressed this Update().</summary>
	public static GetKeyStateFunc GetKeyDown = delegate(KeyCode key) {
		if(key >= KeyCode.JoystickButton0 && ignoreControllerInput) return false;
		return Input.GetKeyDown(key);
	};

	/// <summary>GetKeyDown function -- return whether the specified key was released this Update().</summary>
	public static GetKeyStateFunc GetKeyUp = delegate(KeyCode key) {
		if(key >= KeyCode.JoystickButton0 && ignoreControllerInput) return false;
		return Input.GetKeyUp(key);
	};

	/// <summary>GetKey function -- return whether the specified key is currently held.</summary>
	public static GetKeyStateFunc GetKey = delegate(KeyCode key) {
		if(key >= KeyCode.JoystickButton0 && ignoreControllerInput) return false;
		return Input.GetKey(key);
	};

	/// <summary>GetAxis function -- return the state of the specified axis.</summary>
	public static GetAxisFunc GetAxis = delegate(string axis) {
		if(ignoreControllerInput) return 0f;
		return Input.GetAxis(axis);
	};

	/// <summary>User-settable Input.anyKeyDown</summary>
	public static GetAnyKeyFunc GetAnyKeyDown;

	/// <summary>Get the details of the specified mouse button.</summary>
	public static GetMouseDelegate GetMouse = delegate(int button) { return mMouse[button]; };

	/// <summary>
	///     Get or create a touch event. If you are trying to iterate through a list of active touches, use activeTouches
	///     instead.
	/// </summary>
	public static GetTouchDelegate GetTouch = delegate(int id, bool createIfMissing) {
		if(id < 0) return GetMouse(-id - 1);

		for(int i = 0, imax = mTouchIDs.Count; i < imax; ++i)
			if(mTouchIDs[i] == id)
				return activeTouches[i];

		if(createIfMissing) {
			var touch = new MouseOrTouch();
			touch.pressTime = RealTime.time;
			touch.touchBegan = true;
			activeTouches.Add(touch);
			mTouchIDs.Add(id);
			return touch;
		}
		return null;
	};

	/// <summary>Remove a touch event from the list.</summary>
	public static RemoveTouchDelegate RemoveTouch = delegate(int id) {
		for(int i = 0, imax = mTouchIDs.Count; i < imax; ++i)
			if(mTouchIDs[i] == id) {
				mTouchIDs.RemoveAt(i);
				activeTouches.RemoveAt(i);
				return;
			}
	};

	/// <summary>
	///     Delegate triggered when the screen size changes for any reason. Subscribe to it if you don't want to compare
	///     Screen.width and Screen.height each frame.
	/// </summary>
	public static OnScreenResize onScreenResize;

	public delegate void OnScreenResize();

	/// <summary>
	///     Event type -- use "UI" for your user interfaces, and "World" for your game camera. This setting changes how
	///     raycasts are handled. Raycasts have to be more complicated for UI cameras.
	/// </summary>
	public EventType eventType = EventType.UI_3D;

	/// <summary>
	///     By default, events will go to rigidbodies when the Event Type is not UI. You can change this behaviour back to
	///     how it was pre-3.7.0 using this flag.
	/// </summary>
	public bool eventsGoToColliders;

	/// <summary>Which layers will receive events.</summary>
	public LayerMask eventReceiverMask = -1;

	public enum ProcessEventsIn {
		Update,
		LateUpdate
	}

	/// <summary>When events will be processed.</summary>
	public ProcessEventsIn processEventsIn = ProcessEventsIn.Update;

	/// <summary>If 'true', currently hovered object will be shown in the top left corner.</summary>
	public bool debug;

	/// <summary>Whether the mouse input is used.</summary>
	public bool useMouse = true;

	/// <summary>Whether the touch-based input is used.</summary>
	public bool useTouch = true;

	/// <summary>Whether multi-touch is allowed.</summary>
	public bool allowMultiTouch = true;

	/// <summary>Whether the keyboard events will be processed.</summary>
	public bool useKeyboard = true;

	/// <summary>Whether the joystick and controller events will be processed.</summary>
	public bool useController = true;

	[System.Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public bool stickyPress => true;

	/// <summary>
	///     Whether the tooltip will disappear as soon as the mouse moves (false) or only if the mouse moves outside of
	///     the widget's area (true).
	/// </summary>
	public bool stickyTooltip = true;

	/// <summary>How long of a delay to expect before showing the tooltip.</summary>
	public float tooltipDelay = 1f;

	/// <summary>
	///     If enabled, a tooltip will be shown after touch gets pressed on something and held for more than
	///     "tooltipDelay" seconds.
	/// </summary>
	public bool longPressTooltip;

	/// <summary>How much the mouse has to be moved after pressing a button before it starts to send out drag events.</summary>
	public float mouseDragThreshold = 4f;

	/// <summary>
	///     How far the mouse is allowed to move in pixels before it's no longer considered for click events, if the click
	///     notification is based on delta.
	/// </summary>
	public float mouseClickThreshold = 10f;

	/// <summary>How much the mouse has to be moved after pressing a button before it starts to send out drag events.</summary>
	public float touchDragThreshold = 40f;

	/// <summary>
	///     How far the touch is allowed to move in pixels before it's no longer considered for click events, if the click
	///     notification is based on delta.
	/// </summary>
	public float touchClickThreshold = 40f;

	/// <summary>Raycast range distance. By default it's as far as the camera can see.</summary>
	public float rangeDistance = -1f;

	/// <summary>Name of the axis used to send left and right key events.</summary>
	public string horizontalAxisName = "Horizontal";

	/// <summary>Name of the axis used to send up and down key events.</summary>
	public string verticalAxisName = "Vertical";

	/// <summary>Name of the horizontal axis used to move scroll views and sliders around.</summary>
	public string horizontalPanAxisName;

	/// <summary>Name of the vertical axis used to move scroll views and sliders around.</summary>
	public string verticalPanAxisName;

	/// <summary>Name of the axis used for scrolling.</summary>
	public string scrollAxisName = "Mouse ScrollWheel";

	/// <summary>Simulate a right-click on OSX when the Command key is held and a left-click is used (for trackpad).</summary>
	[Tooltip("If enabled, command-click will result in a right-click event on OSX")]
	public bool commandClick = true;

	/// <summary>Various keys used by the camera.</summary>
	public KeyCode submitKey0 = KeyCode.Return;
	public KeyCode submitKey1 = KeyCode.JoystickButton0;
	public KeyCode cancelKey0 = KeyCode.Escape;
	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	/// <summary>Whether NGUI will automatically hide the mouse cursor when controller or touch input is detected.</summary>
	public bool autoHideCursor = true;

	public delegate void OnCustomInput();

	/// <summary>
	///     Custom input processing logic, if desired. For example: WP7 touches. Use UICamera.current to get the current
	///     camera.
	/// </summary>
	public static OnCustomInput onCustomInput;

	/// <summary>Whether tooltips will be shown or not.</summary>
	public static bool showTooltips = true;

	/// <summary>
	///     Whether controller input will be temporarily disabled or not. It's useful to be able to turn off controller
	///     interaction and only turn it on when the UI is actually visible.
	/// </summary>

	public static bool disableController {
		get { return mDisableController && !UIPopupList.isOpen; }
		set { mDisableController = value; }
	}

	/// <summary>If set to 'true', all events will be ignored until set to 'true'.</summary>
	public static bool ignoreAllEvents = false;

	/// <summary>If set to 'true', controller input will be flat-out ignored. Permanently, for all cameras.</summary>
	public static bool ignoreControllerInput;

	private static bool mDisableController;
	private static Vector2 mLastPos = Vector2.zero;

	/// <summary>Position of the last touch (or mouse) event.</summary>

	[System.Obsolete("Use lastEventPosition instead. It handles controller input properly.")]
	public static Vector2 lastTouchPosition {
		get { return mLastPos; }
		set { mLastPos = value; }
	}

	/// <summary>Position of the last touch (or mouse) event.</summary>

	public static Vector2 lastEventPosition {
		get {
			var scheme = currentScheme;

			if(scheme == ControlScheme.Controller) {
				var go = hoveredObject;

				if(go != null) {
					var b = NGUIMath.CalculateAbsoluteWidgetBounds(go.transform);
					var cam = NGUITools.FindCameraForLayer(go.layer);
					return cam.WorldToScreenPoint(b.center);
				}
			}
			return mLastPos;
		}
		set { mLastPos = value; }
	}

	/// <summary>Position of the last touch (or mouse) event in the world.</summary>
	public static Vector3 lastWorldPosition = Vector3.zero;

	/// <summary>Last raycast into the world space.</summary>
	public static Ray lastWorldRay;

	/// <summary>
	///     Last raycast hit prior to sending out the event. This is useful if you want detailed information about what
	///     was actually hit in your OnClick, OnHover, and other event functions. Note that this is not going to be valid if
	///     you're using 2D colliders.
	/// </summary>
	public static RaycastHit lastHit;

	/// <summary>UICamera that sent out the event.</summary>
	public static UICamera current;

	/// <summary>NGUI event system that will be handling all events.</summary>

	public static UICamera first {
		get {
			if(list == null || list.size == 0) return null;
			return list[0];
		}
	}

	/// <summary>
	///     Last camera active prior to sending out the event. This will always be the camera that actually sent out the
	///     event.
	/// </summary>
	public static Camera currentCamera;

	public delegate void OnSchemeChange();

	/// <summary>Delegate called when the control scheme changes.</summary>
	public static OnSchemeChange onSchemeChange;
	private static ControlScheme mLastScheme = ControlScheme.Mouse;

	/// <summary>Current control scheme. Derived from the last event to arrive.</summary>

	public static ControlScheme currentScheme {
		get {
			if(mCurrentKey == KeyCode.None) return ControlScheme.Touch;
			if(mCurrentKey >= KeyCode.JoystickButton0) return ControlScheme.Controller;

			if(current != null) {
				if(mLastScheme == ControlScheme.Controller && (mCurrentKey == current.submitKey0 || mCurrentKey == current.submitKey1))
					return ControlScheme.Controller;

				if(current.useMouse) return ControlScheme.Mouse;
				if(current.useTouch) return ControlScheme.Touch;
				return ControlScheme.Controller;
			}
			return ControlScheme.Mouse;
		}
		set {
			if(mLastScheme != value) {
				if(value == ControlScheme.Mouse)
					currentKey = KeyCode.Mouse0;
				else if(value == ControlScheme.Controller)
					currentKey = KeyCode.JoystickButton0;
				else if(value == ControlScheme.Touch)
					currentKey = KeyCode.None;
				else currentKey = KeyCode.Alpha0;

				mLastScheme = value;
			}
		}
	}

	/// <summary>
	///     ID of the touch or mouse operation prior to sending out the event. Mouse ID is '-1' for left, '-2' for right
	///     mouse button, '-3' for middle.
	/// </summary>
	public static int currentTouchID = -100;

	private static KeyCode mCurrentKey = KeyCode.Alpha0;

	/// <summary>Key that triggered the event, if any.</summary>

	public static KeyCode currentKey {
		get { return mCurrentKey; }
		set {
			if(mCurrentKey != value) {
				var before = mLastScheme;
				mCurrentKey = value;
				mLastScheme = currentScheme;

				if(before != mLastScheme) {
					HideTooltip();

					if(mLastScheme == ControlScheme.Mouse) {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
						Screen.lockCursor = false;
						Screen.showCursor = true;
#else
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
#endif
					}
#if UNITY_EDITOR
					else if(mLastScheme == ControlScheme.Controller)
#else
					else
#endif 
					{
						if(current != null && current.autoHideCursor) {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
							Screen.showCursor = false;
							Screen.lockCursor = true;
#else
							Cursor.visible = false;
							Cursor.lockState = CursorLockMode.Locked;
#endif

							// Skip the next 2 frames worth of mouse movement
							mMouse[0].ignoreDelta = 2;
						}
					}

					if(onSchemeChange != null) onSchemeChange();
				}
			}
		}
	}

	/// <summary>Ray projected into the screen underneath the current touch.</summary>

	public static Ray currentRay => currentCamera != null && currentTouch != null ? currentCamera.ScreenPointToRay(currentTouch.pos) : new Ray();

	/// <summary>Current touch, set before any event function gets called.</summary>
	public static MouseOrTouch currentTouch;

	private static bool mInputFocus;

	/// <summary>Whether an input field currently has focus.</summary>

	public static bool inputHasFocus {
		get {
			if(mInputFocus && mSelected && mSelected.activeInHierarchy) return true;
			return false;
		}
	}

	// Obsolete, kept for backwards compatibility.
	private static GameObject mGenericHandler;

	/// <summary>If set, this game object will receive all events regardless of whether they were handled or not.</summary>

	[System.Obsolete("Use delegates instead such as UICamera.onClick, UICamera.onHover, etc.")]
	public static GameObject genericEventHandler {
		get { return mGenericHandler; }
		set { mGenericHandler = value; }
	}

	/// <summary>If events don't get handled, they will be forwarded to this game object.</summary>
	public static GameObject fallThrough;

	public delegate void MoveDelegate(Vector2 delta);

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	/// <summary>These notifications are sent out prior to the actual event going out.</summary>
	public static VoidDelegate onClick;
	public static VoidDelegate onDoubleClick;
	public static BoolDelegate onHover;
	public static BoolDelegate onPress;
	public static BoolDelegate onSelect;
	public static FloatDelegate onScroll;
	public static VectorDelegate onDrag;
	public static VoidDelegate onDragStart;
	public static ObjectDelegate onDragOver;
	public static ObjectDelegate onDragOut;
	public static VoidDelegate onDragEnd;
	public static ObjectDelegate onDrop;
	public static KeyCodeDelegate onKey;
	public static KeyCodeDelegate onNavigate;
	public static VectorDelegate onPan;
	public static BoolDelegate onTooltip;
	public static MoveDelegate onMouseMove;

	// Mouse events
	private static readonly MouseOrTouch[] mMouse = {new MouseOrTouch(), new MouseOrTouch(), new MouseOrTouch()};

	/// <summary>Access to the mouse-related data. This is intended to be read-only.</summary>

	public static MouseOrTouch mouse0 => mMouse[0];
	public static MouseOrTouch mouse1 => mMouse[1];
	public static MouseOrTouch mouse2 => mMouse[2];

	// Joystick/controller/keyboard event
	public static MouseOrTouch controller = new MouseOrTouch();

	/// <summary>List of all the active touches.</summary>
	public static List<MouseOrTouch> activeTouches = new List<MouseOrTouch>();

	// Used internally to store IDs of active touches
	private static readonly List<int> mTouchIDs = new List<int>();

	// Used to detect screen dimension changes
	private static int mWidth;
	private static int mHeight;

	// Tooltip widget (mouse only)
	private static GameObject mTooltip;

	// Mouse input is turned off on iOS
	private Camera mCam;
	private static float mTooltipTime;
	private float mNextRaycast;

	/// <summary>Helper function that determines if this script should be handling the events.</summary>

	private bool handlesEvents => eventHandler == this;

	/// <summary>Caching is always preferable for performance.</summary>

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	public Camera cachedCamera { get { if (mCam == null) mCam = camera; return mCam; } }
#else
	public Camera cachedCamera {
		get {
			if(mCam == null) mCam = GetComponent<Camera>();
			return mCam;
		}
	}
#endif

	/// <summary>
	///     Set to 'true' just before OnDrag-related events are sent. No longer needed, but kept for backwards
	///     compatibility.
	/// </summary>
	public static bool isDragging;

	/// <summary>Object that should be showing the tooltip.</summary>

	public static GameObject tooltipObject => mTooltip;

#if !W2
	/// <summary>Whether this object is a part of the UI or not.</summary>
	public static bool IsPartOfUI(GameObject go) {
		if(go == null || go == fallThrough) return false;
		return NGUITools.FindInParents<UIRoot>(go) != null;
	}
#else
// This is a simplified path I use in the Sightseer project. You are welcome to do the same if your UI is only on one layer.
	static public bool IsPartOfUI (GameObject go)
	{
		if (go == null || go == fallThrough) return false;
		if (mUILayer == -1) mUILayer = LayerMask.NameToLayer("UI");
		return go.layer == mUILayer;
	}
	static int mUILayer = -1;
#endif

	/// <summary>Whether the last raycast was over the UI.</summary>

	public static bool isOverUI {
		get {
			var frame = Time.frameCount;

			if(mLastOverCheck != frame) {
				mLastOverCheck = frame;

				if(currentTouch != null) {
					mLastOverResult = currentTouch.isOverUI;
					return mLastOverResult;
				}

				for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
					var touch = activeTouches[i];

					if(IsPartOfUI(touch.pressed)) {
						mLastOverResult = true;
						return mLastOverResult;
					}
				}

				for(var i = 0; i < 3; ++i) {
					var m = mMouse[i];

					if(IsPartOfUI(m.current)) {
						mLastOverResult = true;
						return mLastOverResult;
					}
				}

				mLastOverResult = IsPartOfUI(controller.pressed);
			}
			return mLastOverResult;
		}
	}

	/// <summary>
	///     Much like 'isOverUI', but also returns 'true' if there is currently an active mouse press on a UI element, or
	///     if a UI input has focus.
	/// </summary>

	public static bool uiHasFocus {
		get {
			var frame = Time.frameCount;

			if(mLastFocusCheck != frame) {
				mLastFocusCheck = frame;

				if(inputHasFocus) {
					mLastFocusResult = true;
					return mLastFocusResult;
				}

				if(currentTouch != null) {
					mLastFocusResult = currentTouch.isOverUI;
					return mLastFocusResult;
				}

				for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
					var touch = activeTouches[i];

					if(IsPartOfUI(touch.pressed)) {
						mLastFocusResult = true;
						return mLastFocusResult;
					}
				}

				for(var i = 0; i < 3; ++i) {
					var m = mMouse[i];

					if(IsPartOfUI(m.pressed) || IsPartOfUI(m.current)) {
						mLastFocusResult = true;
						return mLastFocusResult;
					}
				}

				mLastFocusResult = IsPartOfUI(controller.pressed);
			}
			return mLastFocusResult;
		}
	}

	/// <summary>Whether there is a active current focus on the UI -- either input, or an active touch.</summary>

	public static bool interactingWithUI {
		get {
			var frame = Time.frameCount;

			if(mLastInteractionCheck != frame) {
				mLastInteractionCheck = frame;

				if(inputHasFocus) {
					mLastInteractionResult = true;
					return mLastInteractionResult;
				}

				for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
					var touch = activeTouches[i];

					if(IsPartOfUI(touch.pressed)) {
						mLastInteractionResult = true;
						return mLastInteractionResult;
					}
				}

				for(var i = 0; i < 3; ++i) {
					var m = mMouse[i];

					if(IsPartOfUI(m.pressed)) {
						mLastInteractionResult = true;
						return mLastInteractionResult;
					}
				}

				mLastInteractionResult = IsPartOfUI(controller.pressed);
			}
			return mLastInteractionResult;
		}
	}

	private static int mLastInteractionCheck = -1;
	private static bool mLastInteractionResult;
	private static int mLastFocusCheck = -1;
	private static bool mLastFocusResult;
	private static int mLastOverCheck = -1;
	private static bool mLastOverResult;

	private static GameObject mRayHitObject;
	private static GameObject mHover;
	private static GameObject mSelected;

	/// <summary>
	///     The object over which the mouse is hovering over, or the object currently selected by the controller input.
	///     Mouse and controller input share the same hovered object, while touches have no hovered object at all. Checking
	///     this value from within a touch-based event will simply return the current touched object.
	/// </summary>

	public static GameObject hoveredObject {
		get {
			if(currentTouch != null && (currentScheme != ControlScheme.Mouse || currentTouch.dragStarted)) return currentTouch.current;
			if(mHover && mHover.activeInHierarchy) return mHover;
			mHover = null;
			return null;
		}
		set {
			// We already have this object highlighted
			if(mHover == value) return;

			var statesDiffer = false;
			var prevCamera = current;

			if(currentTouch == null) {
				statesDiffer = true;
				currentTouchID = -100;
				currentTouch = controller;
			}

			// Hide the tooltip
			ShowTooltip(null);

			// Remove the selection
			if(mSelected && currentScheme == ControlScheme.Controller) {
				Notify(mSelected, "OnSelect", false);
				if(onSelect != null) onSelect(mSelected, false);
				mSelected = null;
			}

			// Remove the previous hover state
			if(mHover) {
				Notify(mHover, "OnHover", false);
				if(onHover != null) onHover(mHover, false);
			}

			mHover = value;
			currentTouch.clickNotification = ClickNotification.None;

			if(mHover) {
				if(mHover != controller.current) {
#if UNITY_5_5_OR_NEWER
					UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					if(mHover.GetComponent<UIKeyNavigation>() != null) controller.current = mHover;
					UnityEngine.Profiling.Profiler.EndSample();
#else
					Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					if (mHover.GetComponent<UIKeyNavigation>() != null) controller.current = mHover;
					Profiler.EndSample();
#endif
				}

				// Locate the appropriate camera for the new object
				if(statesDiffer) {
					var cam = mHover != null ? FindCameraForLayer(mHover.layer) : list[0];

					if(cam != null) {
						current = cam;
						currentCamera = cam.cachedCamera;
					}
				}

				if(onHover != null) onHover(mHover, true);
				Notify(mHover, "OnHover", true);
			}

			// Restore previous states
			if(statesDiffer) {
				current = prevCamera;
				currentCamera = prevCamera != null ? prevCamera.cachedCamera : null;
				currentTouch = null;
				currentTouchID = -100;
			}
		}
	}

	/// <summary>Currently chosen object for controller-based navigation.</summary>

	public static GameObject controllerNavigationObject {
		get {
			if(controller.current && controller.current.activeInHierarchy)
				return controller.current;

			// Automatically update the object chosen by the controller
			if(currentScheme == ControlScheme.Controller &&
			   current != null && current.useController && !ignoreControllerInput && UIKeyNavigation.list.size > 0) {
				for(var i = 0; i < UIKeyNavigation.list.size; ++i) {
					var nav = UIKeyNavigation.list[i];

					if(nav && nav.constraint != UIKeyNavigation.Constraint.Explicit && nav.startsSelected) {
						hoveredObject = nav.gameObject;
						controller.current = mHover;
						return mHover;
					}
				}

				if(mHover == null)
					for(var i = 0; i < UIKeyNavigation.list.size; ++i) {
						var nav = UIKeyNavigation.list[i];

						if(nav && nav.constraint != UIKeyNavigation.Constraint.Explicit) {
							hoveredObject = nav.gameObject;
							controller.current = mHover;
							return mHover;
						}
					}
			}

			controller.current = null;
			return null;
		}
		set {
			if(controller.current != value && controller.current) {
				Notify(controller.current, "OnHover", false);
				if(onHover != null) onHover(controller.current, false);
				controller.current = null;
			}

			hoveredObject = value;
		}
	}

	/// <summary>
	///     Selected object receives exclusive focus. An input field requires exclusive focus in order to type, for
	///     example. Any object is capable of grabbing the selection just by clicking on that object, but only one object can
	///     be selected at a time.
	/// </summary>

	public static GameObject selectedObject {
		get {
			if(mSelected && mSelected.activeInHierarchy) return mSelected;
			mSelected = null;
			return null;
		}
		set {
			if(mSelected == value) {
				hoveredObject = value;
				controller.current = value;
				return;
			}

			// Hide the tooltip
			ShowTooltip(null);

			var statesDiffer = false;
			var prevCamera = current;
			//ControlScheme scheme = currentScheme;

			if(currentTouch == null) {
				statesDiffer = true;
				currentTouchID = -100;
				currentTouch = controller;
			}

			// Input no longer has selection, even if it did
			mInputFocus = false;

			// Remove the selection
			if(mSelected) {
				Notify(mSelected, "OnSelect", false);
				if(onSelect != null) onSelect(mSelected, false);
			}

			// Remove the hovered state
			//if (mHover && scheme < ControlScheme.Controller)
			//{
			//    Notify(mHover, "OnHover", false);
			//    if (onHover != null) onHover(mHover, false);
			//    mHover = null;
			//}

			// Change the selection and hover
			mSelected = value;
			//if (scheme >= ControlScheme.Controller) mHover = value;
			currentTouch.clickNotification = ClickNotification.None;

			if(value != null) {
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#else
				Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#endif
				var nav = value.GetComponent<UIKeyNavigation>();
				if(nav != null) controller.current = value;
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.EndSample();
#else
				Profiler.EndSample();
#endif
			}

			// Set the camera for events
			if(mSelected && statesDiffer) {
				var cam = mSelected != null ? FindCameraForLayer(mSelected.layer) : list[0];

				if(cam != null) {
					current = cam;
					currentCamera = cam.cachedCamera;
				}
			}

			// Set the hovered state first
			//if (mHover && currentScheme >= ControlScheme.Controller)
			//{
			//    if (onHover != null) onHover(mHover, true);
			//    Notify(mHover, "OnHover", true);
			//}

			// Set the selection
			if(mSelected) {
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
				mInputFocus = mSelected.activeInHierarchy && mSelected.GetComponent<UIInput>() != null;
				UnityEngine.Profiling.Profiler.EndSample();
#else
				Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
				mInputFocus = (mSelected.activeInHierarchy && mSelected.GetComponent<UIInput>() != null);
				Profiler.EndSample();
#endif
				if(onSelect != null) onSelect(mSelected, true);
				Notify(mSelected, "OnSelect", true);
			}

			// Restore the states
			if(statesDiffer) {
				current = prevCamera;
				currentCamera = prevCamera != null ? prevCamera.cachedCamera : null;
				currentTouch = null;
				currentTouchID = -100;
			}
		}
	}

	/// <summary>Returns 'true' if any of the active touch, mouse or controller is currently holding the specified object.</summary>
	public static bool IsPressed(GameObject go) {
		for(var i = 0; i < 3; ++i)
			if(mMouse[i].pressed == go)
				return true;
		for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
			var touch = activeTouches[i];
			if(touch.pressed == go) return true;
		}
		if(controller.pressed == go) return true;
		return false;
	}

	[System.Obsolete("Use either 'CountInputSources()' or 'activeTouches.Count'")]
	public static int touchCount => CountInputSources();

	/// <summary>
	///     Number of active touches from all sources. Note that this will include the sum of touch, mouse and controller
	///     events. If you want only touch events, use activeTouches.Count.
	/// </summary>
	public static int CountInputSources() {
		var count = 0;

		for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
			var touch = activeTouches[i];
			if(touch.pressed != null)
				++count;
		}

		for(var i = 0; i < mMouse.Length; ++i)
			if(mMouse[i].pressed != null)
				++count;

		if(controller.pressed != null)
			++count;

		return count;
	}

	/// <summary>Number of active drag events from all sources.</summary>

	public static int dragCount {
		get {
			var count = 0;

			for(int i = 0, imax = activeTouches.Count; i < imax; ++i) {
				var touch = activeTouches[i];
				if(touch.dragged != null)
					++count;
			}

			for(var i = 0; i < mMouse.Length; ++i)
				if(mMouse[i].dragged != null)
					++count;

			if(controller.dragged != null)
				++count;

			return count;
		}
	}

	/// <summary>Convenience function that returns the main HUD camera.</summary>

	public static Camera mainCamera {
		get {
			var mouse = eventHandler;
			return mouse != null ? mouse.cachedCamera : null;
		}
	}

	/// <summary>Event handler for all types of events.</summary>

	public static UICamera eventHandler {
		get {
			for(var i = 0; i < list.size; ++i) {
				// Invalid or inactive entry -- keep going
				var cam = list.buffer[i];
				if(cam == null || !cam.enabled || !NGUITools.GetActive(cam.gameObject)) continue;
				return cam;
			}
			return null;
		}
	}

	/// <summary>Static comparison function used for sorting.</summary>
	private static int CompareFunc(UICamera a, UICamera b) {
		if(a.cachedCamera.depth < b.cachedCamera.depth) return 1;
		if(a.cachedCamera.depth > b.cachedCamera.depth) return -1;
		return 0;
	}

	private struct DepthEntry {
		public int depth;
		public RaycastHit hit;
		public Vector3 point;
		public GameObject go;
	}

	private static DepthEntry mHit;
	private static readonly BetterList<DepthEntry> mHits = new BetterList<DepthEntry>();

	/// <summary>
	///     Find the rigidbody on the parent, but return 'null' if a UIPanel is found instead. The idea is: send events to
	///     the rigidbody in the world, but to colliders in the UI.
	/// </summary>
	private static Rigidbody FindRootRigidbody(Transform trans) {
#if UNITY_5_5_OR_NEWER
		UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#else
		Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#endif

		while(trans != null) {
			if(trans.GetComponent<UIPanel>() != null) break;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Rigidbody rb = trans.rigidbody;
#else
			var rb = trans.GetComponent<Rigidbody>();
#endif
			if(rb != null) {
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.EndSample();
#else
				Profiler.EndSample();
#endif
				return rb;
			}
			trans = trans.parent;
		}
#if UNITY_5_5_OR_NEWER
		UnityEngine.Profiling.Profiler.EndSample();
#else
		Profiler.EndSample();
#endif
		return null;
	}

	/// <summary>Find the 2D rigidbody on the parent, but return 'null' if a UIPanel is found instead.</summary>
	private static Rigidbody2D FindRootRigidbody2D(Transform trans) {
#if UNITY_5_5_OR_NEWER
		UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#else
		Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
#endif

		while(trans != null) {
			if(trans.GetComponent<UIPanel>() != null) break;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Rigidbody2D rb = trans.rigidbody2D;
#else
			var rb = trans.GetComponent<Rigidbody2D>();
#endif
			if(rb != null) {
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.EndSample();
#else
				Profiler.EndSample();
#endif
				return rb;
			}
			trans = trans.parent;
		}
#if UNITY_5_5_OR_NEWER
		UnityEngine.Profiling.Profiler.EndSample();
#else
		Profiler.EndSample();
#endif
		return null;
	}

	/// <summary>Raycast into the screen underneath the touch and update its 'current' value.</summary>
	public static void Raycast(MouseOrTouch touch) {
		if(!Raycast(touch.pos)) mRayHitObject = fallThrough;
		if(mRayHitObject == null) mRayHitObject = mGenericHandler;
		touch.last = touch.current;
		touch.current = mRayHitObject;
		mLastPos = touch.pos;
	}

#if !UNITY_4_7
	private static RaycastHit[] mRayHits;
	private static Collider2D[] mOverlap;
#endif

	/// <summary>Returns the object under the specified position.</summary>
	public static bool Raycast(Vector3 inPos) {
		for(var i = 0; i < list.size; ++i) {
			var cam = list.buffer[i];

			// Skip inactive scripts
			if(!cam.enabled || !NGUITools.GetActive(cam.gameObject)) continue;

			// Convert to view space
			currentCamera = cam.cachedCamera;
#if !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
			if(currentCamera.targetDisplay != 0) continue;
#endif
			var pos = currentCamera.ScreenToViewportPoint(inPos);
			if(float.IsNaN(pos.x) || float.IsNaN(pos.y)) continue;

			// If it's outside the camera's viewport, do nothing
			if(pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f) continue;

			// Cast a ray into the screen
			var ray = currentCamera.ScreenPointToRay(inPos);

			// Raycast into the screen
			var mask = currentCamera.cullingMask & cam.eventReceiverMask;
			var dist = cam.rangeDistance > 0f ? cam.rangeDistance : currentCamera.farClipPlane - currentCamera.nearClipPlane;

			if(cam.eventType == EventType.World_3D) {
				lastWorldRay = ray;

#if UNITY_4_7
				if (Physics.Raycast(ray, out lastHit, dist, mask))
#else
				if(Physics.Raycast(ray, out lastHit, dist, mask, QueryTriggerInteraction.Ignore))
#endif 
				{
					lastWorldPosition = lastHit.point;
					mRayHitObject = lastHit.collider.gameObject;

					if(!cam.eventsGoToColliders) {
						var rb = mRayHitObject.gameObject.GetComponentInParent<Rigidbody>();
						if(rb != null) mRayHitObject = rb.gameObject;
					}
					return true;
				}
			}
			else if(cam.eventType == EventType.UI_3D) {
#if UNITY_4_7
				RaycastHit[] mRayHits = Physics.RaycastAll(ray, dist, mask);
				var hitCount = mRayHits.Length;
#else
				if(mRayHits == null) mRayHits = new RaycastHit[50];
				var hitCount = Physics.RaycastNonAlloc(ray, mRayHits, dist, mask, QueryTriggerInteraction.Collide);
#endif
				if(hitCount > 1) {
					for(var b = 0; b < hitCount; ++b) {
						var go = mRayHits[b].collider.gameObject;
#if UNITY_5_5_OR_NEWER
						UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
						var w = go.GetComponent<UIWidget>();
						UnityEngine.Profiling.Profiler.EndSample();
#else
						Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
						UIWidget w = go.GetComponent<UIWidget>();
						Profiler.EndSample();
#endif

						if(w != null) {
							if(!w.isVisible) continue;
							if(w.hitCheck != null && !w.hitCheck(mRayHits[b].point)) continue;
						}
						else {
							var rect = NGUITools.FindInParents<UIRect>(go);
							if(rect != null && rect.finalAlpha < 0.001f) continue;
						}

						mHit.depth = NGUITools.CalculateRaycastDepth(go);

						if(mHit.depth != int.MaxValue) {
							mHit.hit = mRayHits[b];
							mHit.point = mRayHits[b].point;
							mHit.go = mRayHits[b].collider.gameObject;
							mHits.Add(mHit);
						}
					}

					mHits.Sort(delegate(DepthEntry r1, DepthEntry r2) { return r2.depth.CompareTo(r1.depth); });

					for(var b = 0; b < mHits.size; ++b) {
#if UNITY_FLASH
						if (IsVisible(mHits.buffer[b]))
#else
						if(IsVisible(ref mHits.buffer[b]))
#endif 
						{
							lastHit = mHits[b].hit;
							mRayHitObject = mHits[b].go;
							lastWorldRay = ray;
							lastWorldPosition = mHits[b].point;
							mHits.Clear();
							return true;
						}
					}
					mHits.Clear();
				}
				else if(hitCount == 1) {
					var go = mRayHits[0].collider.gameObject;
#if UNITY_5_5_OR_NEWER
					UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					var w = go.GetComponent<UIWidget>();
					UnityEngine.Profiling.Profiler.EndSample();
#else
					Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					UIWidget w = go.GetComponent<UIWidget>();
					Profiler.EndSample();
#endif

					if(w != null) {
						if(!w.isVisible) continue;
						if(w.hitCheck != null && !w.hitCheck(mRayHits[0].point)) continue;
					}
					else {
						var rect = NGUITools.FindInParents<UIRect>(go);
						if(rect != null && rect.finalAlpha < 0.001f) continue;
					}

					if(IsVisible(mRayHits[0].point, mRayHits[0].collider.gameObject)) {
						lastHit = mRayHits[0];
						lastWorldRay = ray;
						lastWorldPosition = mRayHits[0].point;
						mRayHitObject = lastHit.collider.gameObject;
						return true;
					}
				}
			}
			else if(cam.eventType == EventType.World_2D) {
				if(m2DPlane.Raycast(ray, out dist)) {
					var point = ray.GetPoint(dist);
					var c2d = Physics2D.OverlapPoint(point, mask);

					if(c2d) {
						lastWorldPosition = point;
						mRayHitObject = c2d.gameObject;

						if(!cam.eventsGoToColliders) {
							var rb = FindRootRigidbody2D(mRayHitObject.transform);
							if(rb != null) mRayHitObject = rb.gameObject;
						}
						return true;
					}
				}
			}
			else if(cam.eventType == EventType.UI_2D) {
				if(m2DPlane.Raycast(ray, out dist)) {
					lastWorldPosition = ray.GetPoint(dist);
#if UNITY_4_7
					Collider2D[] mOverlap = Physics2D.OverlapPointAll(lastWorldPosition, mask);
					var hitCount = mOverlap.Length;
#else
					if(mOverlap == null) mOverlap = new Collider2D[50];
					var hitCount = Physics2D.OverlapPointNonAlloc(lastWorldPosition, mOverlap, mask);
#endif
					if(hitCount > 1) {
						for(var b = 0; b < hitCount; ++b) {
							var go = mOverlap[b].gameObject;
#if UNITY_5_5_OR_NEWER
							UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
							var w = go.GetComponent<UIWidget>();
							UnityEngine.Profiling.Profiler.EndSample();
#else
							Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
							UIWidget w = go.GetComponent<UIWidget>();
							Profiler.EndSample();
#endif

							if(w != null) {
								if(!w.isVisible) continue;
								if(w.hitCheck != null && !w.hitCheck(lastWorldPosition)) continue;
							}
							else {
								var rect = NGUITools.FindInParents<UIRect>(go);
								if(rect != null && rect.finalAlpha < 0.001f) continue;
							}

							mHit.depth = NGUITools.CalculateRaycastDepth(go);

							if(mHit.depth != int.MaxValue) {
								mHit.go = go;
								mHit.point = lastWorldPosition;
								mHits.Add(mHit);
							}
						}

						mHits.Sort(delegate(DepthEntry r1, DepthEntry r2) { return r2.depth.CompareTo(r1.depth); });

						for(var b = 0; b < mHits.size; ++b) {
#if UNITY_FLASH
							if (IsVisible(mHits.buffer[b]))
#else
							if(IsVisible(ref mHits.buffer[b]))
#endif 
							{
								mRayHitObject = mHits[b].go;
								mHits.Clear();
								return true;
							}
						}
						mHits.Clear();
					}
					else if(hitCount == 1) {
						var go = mOverlap[0].gameObject;
#if UNITY_5_5_OR_NEWER
						UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
						var w = go.GetComponent<UIWidget>();
						UnityEngine.Profiling.Profiler.EndSample();
#else
						Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
						var w = go.GetComponent<UIWidget>();
						Profiler.EndSample();
#endif

						if(w != null) {
							if(!w.isVisible) continue;
							if(w.hitCheck != null && !w.hitCheck(lastWorldPosition)) continue;
						}
						else {
							var rect = NGUITools.FindInParents<UIRect>(go);
							if(rect != null && rect.finalAlpha < 0.001f) continue;
						}

						if(IsVisible(lastWorldPosition, go)) {
							mRayHitObject = go;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	/// <summary>Helper function to check if the specified hit is visible by the panel.</summary>
	private static bool IsVisible(Vector3 worldPoint, GameObject go) {
		var panel = NGUITools.FindInParents<UIPanel>(go);

		while(panel != null) {
			if(!panel.IsVisible(worldPoint)) return false;
			panel = panel.parentPanel;
		}
		return true;
	}

	/// <summary>Helper function to check if the specified hit is visible by the panel.</summary>
#if UNITY_FLASH
	static bool IsVisible (DepthEntry de)
#else
	private static bool IsVisible(ref DepthEntry de)
#endif 
	{
		var panel = NGUITools.FindInParents<UIPanel>(de.go);

		while(panel != null) {
			if(!panel.IsVisible(de.point)) return false;
			panel = panel.parentPanel;
		}
		return true;
	}

	/// <summary>Whether the specified object should be highlighted.</summary>
	public static bool IsHighlighted(GameObject go) {
		return hoveredObject == go;
	}

	/// <summary>Find the camera responsible for handling events on objects of the specified layer.</summary>
	public static UICamera FindCameraForLayer(int layer) {
		var layerMask = 1 << layer;

		for(var i = 0; i < list.size; ++i) {
			var cam = list.buffer[i];
			var uc = cam.cachedCamera;
			if(uc != null && (uc.cullingMask & layerMask) != 0) return cam;
		}
		return null;
	}

	/// <summary>Using the keyboard will result in 1 or -1, depending on whether up or down keys have been pressed.</summary>
	private static int GetDirection(KeyCode up, KeyCode down) {
		if(GetKeyDown(up)) {
			currentKey = up;
			return 1;
		}
		if(GetKeyDown(down)) {
			currentKey = down;
			return -1;
		}
		return 0;
	}

	/// <summary>Using the keyboard will result in 1 or -1, depending on whether up or down keys have been pressed.</summary>
	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1) {
		if(GetKeyDown(up0)) {
			currentKey = up0;
			return 1;
		}
		if(GetKeyDown(up1)) {
			currentKey = up1;
			return 1;
		}
		if(GetKeyDown(down0)) {
			currentKey = down0;
			return -1;
		}
		if(GetKeyDown(down1)) {
			currentKey = down1;
			return -1;
		}
		return 0;
	}

	// Used to ensure that joystick-based controls don't trigger that often
	private static float mNextEvent;

	/// <summary>Using the joystick to move the UI results in 1 or -1 if the threshold has been passed, mimicking up/down keys.</summary>
	private static int GetDirection(string axis) {
		var time = RealTime.time;

		if(mNextEvent < time && !string.IsNullOrEmpty(axis)) {
			var val = GetAxis(axis);

			if(val > 0.75f) {
				currentKey = KeyCode.JoystickButton0;
				mNextEvent = time + 0.25f;
				return 1;
			}

			if(val < -0.75f) {
				currentKey = KeyCode.JoystickButton0;
				mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	private static int mNotifying;

	/// <summary>
	///     Generic notification function. Used in place of SendMessage to shorten the code and allow for more than one
	///     receiver.
	/// </summary>
	public static void Notify(GameObject go, string funcName, object obj) {
		if(mNotifying > 10) return;

		// Automatically forward events to the currently open popup list
		if(currentScheme == ControlScheme.Controller && UIPopupList.isOpen &&
		   UIPopupList.current.source == go && UIPopupList.isOpen)
			go = UIPopupList.current.gameObject;

		if(go && go.activeInHierarchy) {
			++mNotifying;
			//if (currentScheme == ControlScheme.Controller)
			//	Debug.Log((go != null ? "[" + go.name + "]." : "[global].") + funcName + "(" + obj + ");", go);
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if(mGenericHandler != null && mGenericHandler != go)
				mGenericHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			--mNotifying;
		}
	}

	/// <summary>Add this camera to the list.</summary>
	private void Awake() {
		mWidth = Screen.width;
		mHeight = Screen.height;

#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY || UNITY_WINRT || UNITY_METRO)
		currentScheme = ControlScheme.Touch;
#else
#if !UNITY_5_5_OR_NEWER
		if (Application.platform == RuntimePlatform.PS3 || Application.platform == RuntimePlatform.XBOX360)
#else
		if(Application.platform == RuntimePlatform.PS4 || Application.platform == RuntimePlatform.XboxOne)
#endif
			currentScheme = ControlScheme.Controller;
#endif

		// Save the starting mouse position
		mMouse[0].pos = Input.mousePosition;

		for(var i = 1; i < 3; ++i) {
			mMouse[i].pos = mMouse[0].pos;
			mMouse[i].lastPos = mMouse[0].pos;
		}
		mLastPos = mMouse[0].pos;

#if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX)
		string[] args = System.Environment.GetCommandLineArgs();

		if (args != null)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				string s = args[i];
				if (s == "-noMouse") useMouse = false;
				else if (s == "-noTouch") useTouch = false;
				else if (s == "-noController") { useController = false; ignoreControllerInput = true; }
				else if (s == "-noJoystick") { useController = false; ignoreControllerInput = true; }
				else if (s == "-useMouse") useMouse = true;
				else if (s == "-useTouch") useTouch = true;
				else if (s == "-useController") useController = true;
				else if (s == "-useJoystick") useController = true;
			}
		}
#endif
	}

	/// <summary>Sort the list when enabled.</summary>
	private void OnEnable() {
		list.Add(this);
		list.Sort(CompareFunc);
	}

	/// <summary>Remove this camera from the list.</summary>
	private void OnDisable() {
		list.Remove(this);
	}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
	private static bool disableControllerCheck = true;
#endif

	/// <summary>We don't want the camera to send out any kind of mouse events.</summary>
	private void Start() {
		list.Sort(CompareFunc);

		if(eventType != EventType.World_3D && cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
			cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;

		if(Application.isPlaying) {
			// Always set a fall-through object
			if(fallThrough == null) {
				var root = NGUITools.FindInParents<UIRoot>(gameObject);
				fallThrough = root != null ? root.gameObject : gameObject;
			}
			cachedCamera.eventMask = 0;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
			// Automatically disable controller-based input if the game starts with a non-zero controller input.
			// This most commonly happens with Thrustmaster and other similar joystick types.
			if(!ignoreControllerInput && disableControllerCheck && useController && handlesEvents) {
				disableControllerCheck = false;
				if(!string.IsNullOrEmpty(horizontalAxisName) && Mathf.Abs(GetAxis(horizontalAxisName)) > 0.1f) ignoreControllerInput = true;
				else if(!string.IsNullOrEmpty(verticalAxisName) && Mathf.Abs(GetAxis(verticalAxisName)) > 0.1f) ignoreControllerInput = true;
				else if(!string.IsNullOrEmpty(horizontalPanAxisName) && Mathf.Abs(GetAxis(horizontalPanAxisName)) > 0.1f) ignoreControllerInput = true;
				else if(!string.IsNullOrEmpty(verticalPanAxisName) && Mathf.Abs(GetAxis(verticalPanAxisName)) > 0.1f) ignoreControllerInput = true;
			}
#endif
		}
	}

#if UNITY_EDITOR
	private void OnValidate() {
		Start();
	}
#endif

	/// <summary>Check the input and send out appropriate events.</summary>
	private void Update() {
		// Ignore events if asked for
		if(ignoreAllEvents) return;

		// Only the first UI layer should be processing events
#if UNITY_EDITOR
		if(!Application.isPlaying || !handlesEvents) return;
#else
        if (!handlesEvents) return;
#endif
		if(processEventsIn == ProcessEventsIn.Update) ProcessEvents();
	}

	/// <summary>Keep an eye on screen size changes.</summary>
	private void LateUpdate() {
#if UNITY_EDITOR
		if(!Application.isPlaying || !handlesEvents) return;
#else
		if (!handlesEvents) return;
#endif
		if(processEventsIn == ProcessEventsIn.LateUpdate) ProcessEvents();

		var w = Screen.width;
		var h = Screen.height;

		if(w != mWidth || h != mHeight) {
			mWidth = w;
			mHeight = h;

			UIRoot.Broadcast("UpdateAnchors");

			if(onScreenResize != null)
				onScreenResize();
		}
	}

	/// <summary>Process all events.</summary>
	private void ProcessEvents() {
		current = this;
		NGUIDebug.debugRaycast = debug;

		// Process touch events first
		if(useTouch) ProcessTouches();
		else if(useMouse) ProcessMouse();

		// Custom input processing
		if(onCustomInput != null) onCustomInput();

		// Update the keyboard and joystick events
		if((useKeyboard || useController) && !disableController && !ignoreControllerInput) ProcessOthers();

		// If it's time to show a tooltip, inform the object we're hovering over
		if(useMouse && mHover != null) {
			var scroll = !string.IsNullOrEmpty(scrollAxisName) ? GetAxis(scrollAxisName) : 0f;

			if(scroll != 0f) {
				if(onScroll != null) onScroll(mHover, scroll);
				Notify(mHover, "OnScroll", scroll);
			}

			if(currentScheme == ControlScheme.Mouse && showTooltips && mTooltipTime != 0f && !UIPopupList.isOpen && mMouse[0].dragged == null &&
			   (mTooltipTime < RealTime.time || GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift))) {
				currentTouch = mMouse[0];
				currentTouchID = -1;
				ShowTooltip(mHover);
			}
		}

		if(mTooltip != null && !NGUITools.GetActive(mTooltip))
			ShowTooltip(null);

		current = null;
		currentTouchID = -100;
	}

	/// <summary>Update mouse input.</summary>
	public void ProcessMouse() {
		// Is any button currently pressed?
		var isPressed = false;
		var justPressed = false;

		for(var i = 0; i < 3; ++i)
			if(Input.GetMouseButtonDown(i)) {
				currentKey = KeyCode.Mouse0 + i;
				justPressed = true;
				isPressed = true;
			}
			else if(Input.GetMouseButton(i)) {
				currentKey = KeyCode.Mouse0 + i;
				isPressed = true;
			}

		// We're currently using touches -- do nothing
		if(currentScheme == ControlScheme.Touch && activeTouches.Count > 0) return;

		currentTouch = mMouse[0];

		// Update the position and delta
		Vector2 pos = Input.mousePosition;

		if(currentTouch.ignoreDelta == 0) {
			currentTouch.delta = pos - currentTouch.pos;
		}
		else {
			--currentTouch.ignoreDelta;
			currentTouch.delta.x = 0f;
			currentTouch.delta.y = 0f;
		}

		var sqrMag = currentTouch.delta.sqrMagnitude;
		currentTouch.pos = pos;
		mLastPos = pos;

		var posChanged = false;

		if(currentScheme != ControlScheme.Mouse) {
			if(sqrMag < 0.001f) return; // Nothing changed and we are not using the mouse -- exit
			currentKey = KeyCode.Mouse0;
			posChanged = true;
		}
		else if(sqrMag > 0.001f) {
			posChanged = true;
		}

		// Propagate the updates to the other mouse buttons
		for(var i = 1; i < 3; ++i) {
			mMouse[i].pos = currentTouch.pos;
			mMouse[i].delta = currentTouch.delta;
		}

		// No need to perform raycasts every frame
		if(isPressed || posChanged || mNextRaycast < RealTime.time) {
			mNextRaycast = RealTime.time + 0.02f;
			Raycast(currentTouch);
			for(var i = 0; i < 3; ++i) mMouse[i].current = currentTouch.current;
		}

		var highlightChanged = currentTouch.last != currentTouch.current;
		var wasPressed = currentTouch.pressed != null;

		if(!wasPressed)
			hoveredObject = currentTouch.current;

		currentTouchID = -1;
		if(highlightChanged) currentKey = KeyCode.Mouse0;

		if(!isPressed && posChanged && (!stickyTooltip || highlightChanged)) {
			if(mTooltipTime != 0f)
				mTooltipTime = Time.unscaledTime + tooltipDelay;
			else if(mTooltip != null)
				ShowTooltip(null);
		}

		// Generic mouse move notifications
		if(posChanged && onMouseMove != null) {
			onMouseMove(currentTouch.delta);
			currentTouch = null;
		}

		// The button was released over a different object -- remove the highlight from the previous
		if(highlightChanged && (justPressed || wasPressed && !isPressed))
			hoveredObject = null;

		// Process all 3 mouse buttons as individual touches
		for(var i = 0; i < 3; ++i) {
			var pressed = Input.GetMouseButtonDown(i);
			var unpressed = Input.GetMouseButtonUp(i);
			if(pressed || unpressed) currentKey = KeyCode.Mouse0 + i;
			currentTouch = mMouse[i];

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			if (commandClick && i == 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				currentTouchID = -2;
				currentKey = KeyCode.Mouse1;
			}
			else
#endif
			{
				currentTouchID = -1 - i;
				currentKey = KeyCode.Mouse0 + i;
			}

			// We don't want to update the last camera while there is a touch happening
			if(pressed) {
				currentTouch.pressedCam = currentCamera;
				currentTouch.pressTime = RealTime.time;
			}
			else if(currentTouch.pressed != null) {
				currentCamera = currentTouch.pressedCam;
			}

			// Process the mouse events
			ProcessTouch(pressed, unpressed);
		}

		// If nothing is pressed and there is an object under the touch, highlight it
		if(!isPressed && highlightChanged) {
			currentTouch = mMouse[0];
			mTooltipTime = Time.unscaledTime + tooltipDelay;
			currentTouchID = -1;
			currentKey = KeyCode.Mouse0;
			hoveredObject = currentTouch.current;
		}

		currentTouch = null;

		// Update the last value
		mMouse[0].last = mMouse[0].current;
		for(var i = 1; i < 3; ++i) mMouse[i].last = mMouse[0].last;
	}

	private static bool mUsingTouchEvents = true;

	public class Touch {
		public int fingerId;
		public TouchPhase phase = TouchPhase.Began;
		public Vector2 position;
		public int tapCount = 0;
	}

	public delegate int GetTouchCountCallback();

	public delegate Touch GetTouchCallback(int index);

	public static GetTouchCountCallback GetInputTouchCount;
	public static GetTouchCallback GetInputTouch;

	/// <summary>Update touch-based events.</summary>
	public void ProcessTouches() {
		var count = GetInputTouchCount == null ? Input.touchCount : GetInputTouchCount();

		for(var i = 0; i < count; ++i) {
			int fingerId;
			TouchPhase phase;
			Vector2 position;
			int tapCount;

			if(GetInputTouch == null) {
				var touch = Input.GetTouch(i);
				phase = touch.phase;
				fingerId = touch.fingerId;
				position = touch.position;
				tapCount = touch.tapCount;
#if UNITY_WIIU && !UNITY_EDITOR
// Unity bug: http://www.tasharen.com/forum/index.php?topic=5821.0
				position.y = Screen.height - position.y;
#endif
			}
			else {
				var touch = GetInputTouch(i);
				phase = touch.phase;
				fingerId = touch.fingerId;
				position = touch.position;
				tapCount = touch.tapCount;
			}

			currentTouchID = allowMultiTouch ? fingerId : 1;
			currentTouch = GetTouch(currentTouchID, true);

			var pressed = phase == TouchPhase.Began || currentTouch.touchBegan;
			var unpressed = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			currentTouch.delta = position - currentTouch.pos;
			currentTouch.pos = position;
			currentKey = KeyCode.None;

			// Raycast into the screen
			Raycast(currentTouch);

			// We don't want to update the last camera while there is a touch happening
			if(pressed) currentTouch.pressedCam = currentCamera;
			else if(currentTouch.pressed != null) currentCamera = currentTouch.pressedCam;

			// Double-tap support
			if(tapCount > 1) currentTouch.clickTime = RealTime.time;

			// Process the events from this touch
			ProcessTouch(pressed, unpressed);

			// If the touch has ended, remove it from the list
			if(unpressed) RemoveTouch(currentTouchID);

			currentTouch.touchBegan = false;
			currentTouch.last = null;
			currentTouch = null;

			// Don't consider other touches
			if(!allowMultiTouch) break;
		}

		if(count == 0) {
			// Skip the first frame after using touch events
			if(mUsingTouchEvents) {
				mUsingTouchEvents = false;
				return;
			}

			if(useMouse) ProcessMouse();
#if UNITY_EDITOR
			else if(GetInputTouch == null) ProcessFakeTouches();
#endif
		}
		else {
			mUsingTouchEvents = true;
		}
	}

	/// <summary>
	///     Process fake touch events where the mouse acts as a touch device. Useful for testing mobile functionality in
	///     the editor.
	/// </summary>
	private void ProcessFakeTouches() {
		var pressed = Input.GetMouseButtonDown(0);
		var unpressed = Input.GetMouseButtonUp(0);
		var held = Input.GetMouseButton(0);

		if(pressed || unpressed || held) {
			currentTouchID = 1;
			currentTouch = mMouse[0];
			currentTouch.touchBegan = pressed;

			if(pressed) {
				currentTouch.pressTime = RealTime.time;
				activeTouches.Add(currentTouch);
			}

			Vector2 pos = Input.mousePosition;
			currentTouch.delta = pos - currentTouch.pos;
			currentTouch.pos = pos;

			// Raycast into the screen
			Raycast(currentTouch);

			// We don't want to update the last camera while there is a touch happening
			if(pressed) currentTouch.pressedCam = currentCamera;
			else if(currentTouch.pressed != null) currentCamera = currentTouch.pressedCam;

			// Process the events from this touch
			currentKey = KeyCode.None;
			ProcessTouch(pressed, unpressed);

			// If the touch has ended, remove it from the list
			if(unpressed) activeTouches.Remove(currentTouch);
			currentTouch.last = null;
			currentTouch = null;
		}
	}

	/// <summary>Process keyboard and joystick events.</summary>
	public void ProcessOthers() {
		currentTouchID = -100;
		currentTouch = controller;

		var submitKeyDown = false;
		var submitKeyUp = false;

		if(submitKey0 != KeyCode.None && GetKeyDown(submitKey0)) {
			currentKey = submitKey0;
			submitKeyDown = true;
		}
		else if(submitKey1 != KeyCode.None && GetKeyDown(submitKey1)) {
			currentKey = submitKey1;
			submitKeyDown = true;
		}
		else if((submitKey0 == KeyCode.Return || submitKey1 == KeyCode.Return) && GetKeyDown(KeyCode.KeypadEnter)) {
			currentKey = submitKey0;
			submitKeyDown = true;
		}

		if(submitKey0 != KeyCode.None && GetKeyUp(submitKey0)) {
			currentKey = submitKey0;
			submitKeyUp = true;
		}
		else if(submitKey1 != KeyCode.None && GetKeyUp(submitKey1)) {
			currentKey = submitKey1;
			submitKeyUp = true;
		}
		else if((submitKey0 == KeyCode.Return || submitKey1 == KeyCode.Return) && GetKeyUp(KeyCode.KeypadEnter)) {
			currentKey = submitKey0;
			submitKeyUp = true;
		}

		if(submitKeyDown) currentTouch.pressTime = RealTime.time;

		if((submitKeyDown || submitKeyUp) && currentScheme == ControlScheme.Controller) {
			currentTouch.current = controllerNavigationObject;
			ProcessTouch(submitKeyDown, submitKeyUp);
			currentTouch.last = currentTouch.current;
		}

		var lastKey = KeyCode.None;

		// Handle controller events
		if(useController && !ignoreControllerInput) {
			// Automatically choose the first available selection object
			if(!disableController && currentScheme == ControlScheme.Controller && (currentTouch.current == null || !currentTouch.current.activeInHierarchy))
				currentTouch.current = controllerNavigationObject;

			if(!string.IsNullOrEmpty(verticalAxisName)) {
				var vertical = GetDirection(verticalAxisName);

				if(vertical != 0) {
					ShowTooltip(null);
					currentScheme = ControlScheme.Controller;
					currentTouch.current = controllerNavigationObject;

					if(currentTouch.current != null) {
						lastKey = vertical > 0 ? KeyCode.UpArrow : KeyCode.DownArrow;
						if(onNavigate != null) onNavigate(currentTouch.current, lastKey);
						Notify(currentTouch.current, "OnNavigate", lastKey);
					}
				}
			}

			if(!string.IsNullOrEmpty(horizontalAxisName)) {
				var horizontal = GetDirection(horizontalAxisName);

				if(horizontal != 0) {
					ShowTooltip(null);
					currentScheme = ControlScheme.Controller;
					currentTouch.current = controllerNavigationObject;

					if(currentTouch.current != null) {
						lastKey = horizontal > 0 ? KeyCode.RightArrow : KeyCode.LeftArrow;
						if(onNavigate != null) onNavigate(currentTouch.current, lastKey);
						Notify(currentTouch.current, "OnNavigate", lastKey);
					}
				}
			}

			var x = !string.IsNullOrEmpty(horizontalPanAxisName) ? GetAxis(horizontalPanAxisName) : 0f;
			var y = !string.IsNullOrEmpty(verticalPanAxisName) ? GetAxis(verticalPanAxisName) : 0f;

			if(x != 0f || y != 0f) {
				ShowTooltip(null);
				currentScheme = ControlScheme.Controller;
				currentTouch.current = controllerNavigationObject;

				if(currentTouch.current != null) {
					var delta = new Vector2(x, y);
					delta *= Time.unscaledDeltaTime;
					if(onPan != null) onPan(currentTouch.current, delta);
					Notify(currentTouch.current, "OnPan", delta);
				}
			}
		}

		// Send out all key events
		if(GetAnyKeyDown != null ? GetAnyKeyDown() : Input.anyKeyDown)
			for(int i = 0, imax = NGUITools.keys.Length; i < imax; ++i) {
				var key = NGUITools.keys[i];
				if(lastKey == key) continue;
				if(!GetKeyDown(key)) continue;

				if(!useKeyboard && key < KeyCode.Mouse0) continue;
				if((!useController || ignoreControllerInput) && key >= KeyCode.JoystickButton0) continue;
				if(!useMouse && key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) continue;

				currentKey = key;
				if(onKey != null) onKey(currentTouch.current, key);
				Notify(currentTouch.current, "OnKey", key);
			}

		currentTouch = null;
	}

	/// <summary>Process the press part of a touch.</summary>
	private void ProcessPress(bool pressed, float click, float drag) {
		// Send out the press message
		if(pressed) {
			if(mTooltip != null) ShowTooltip(null);
			mTooltipTime = Time.unscaledTime + tooltipDelay;
			currentTouch.pressStarted = true;
			if(onPress != null && currentTouch.pressed)
				onPress(currentTouch.pressed, false);

			Notify(currentTouch.pressed, "OnPress", false);

			if(currentScheme == ControlScheme.Mouse && hoveredObject == null && currentTouch.current != null)
				hoveredObject = currentTouch.current;

			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ClickNotification.BasedOnDelta;
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;

			if(onPress != null && currentTouch.pressed)
				onPress(currentTouch.pressed, true);

			Notify(currentTouch.pressed, "OnPress", true);

			// Change the selection
			if(mSelected != currentTouch.pressed) {
				// Input no longer has selection, even if it did
				mInputFocus = false;

				// Remove the selection
				if(mSelected) {
					Notify(mSelected, "OnSelect", false);
					if(onSelect != null) onSelect(mSelected, false);
				}

				// Change the selection
				mSelected = currentTouch.pressed;

				if(currentTouch.pressed != null) {
#if UNITY_5_5_OR_NEWER
					UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					var nav = currentTouch.pressed.GetComponent<UIKeyNavigation>();
					UnityEngine.Profiling.Profiler.EndSample();
#else
					Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					UIKeyNavigation nav = currentTouch.pressed.GetComponent<UIKeyNavigation>();
					Profiler.EndSample();
#endif
					if(nav != null) controller.current = currentTouch.pressed;
				}

				// Set the selection
				if(mSelected) {
#if UNITY_5_5_OR_NEWER
					UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					mInputFocus = mSelected.activeInHierarchy && mSelected.GetComponent<UIInput>() != null;
					UnityEngine.Profiling.Profiler.EndSample();
#else
					Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
					mInputFocus = (mSelected.activeInHierarchy && mSelected.GetComponent<UIInput>() != null);
					Profiler.EndSample();
#endif
					if(onSelect != null) onSelect(mSelected, true);
					Notify(mSelected, "OnSelect", true);
				}
			}
		}
		else if(currentTouch.pressed != null && (currentTouch.delta.sqrMagnitude != 0f || currentTouch.current != currentTouch.last)) {
			// Keep track of the total movement
			currentTouch.totalDelta += currentTouch.delta;
			var mag = currentTouch.totalDelta.sqrMagnitude;
			var justStarted = false;

			// If the drag process hasn't started yet but we've already moved off the object, start it immediately
			if(!currentTouch.dragStarted && currentTouch.last != currentTouch.current) {
				currentTouch.dragStarted = true;
				currentTouch.delta = currentTouch.totalDelta;

				// OnDragOver is sent for consistency, so that OnDragOut is always preceded by OnDragOver
				isDragging = true;

				if(onDragStart != null) onDragStart(currentTouch.dragged);
				Notify(currentTouch.dragged, "OnDragStart", null);

				if(onDragOver != null) onDragOver(currentTouch.last, currentTouch.dragged);
				Notify(currentTouch.last, "OnDragOver", currentTouch.dragged);

				isDragging = false;
			}
			else if(!currentTouch.dragStarted && drag < mag) {
				// If the drag event has not yet started, see if we've dragged the touch far enough to start it
				justStarted = true;
				currentTouch.dragStarted = true;
				currentTouch.delta = currentTouch.totalDelta;
			}

			// If we're dragging the touch, send out drag events
			if(currentTouch.dragStarted) {
				if(mTooltip != null) ShowTooltip(null);

				isDragging = true;
				var isDisabled = currentTouch.clickNotification == ClickNotification.None;

				if(justStarted) {
					if(onDragStart != null) onDragStart(currentTouch.dragged);
					Notify(currentTouch.dragged, "OnDragStart", null);

					if(onDragOver != null) onDragOver(currentTouch.last, currentTouch.dragged);
					Notify(currentTouch.current, "OnDragOver", currentTouch.dragged);
				}
				else if(currentTouch.last != currentTouch.current) {
					if(onDragOut != null) onDragOut(currentTouch.last, currentTouch.dragged);
					Notify(currentTouch.last, "OnDragOut", currentTouch.dragged);

					if(onDragOver != null) onDragOver(currentTouch.last, currentTouch.dragged);
					Notify(currentTouch.current, "OnDragOver", currentTouch.dragged);
				}

				if(onDrag != null) onDrag(currentTouch.dragged, currentTouch.delta);
				Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);

				currentTouch.last = currentTouch.current;
				isDragging = false;

				if(isDisabled)
					currentTouch.clickNotification = ClickNotification.None;
				else if(currentTouch.clickNotification == ClickNotification.BasedOnDelta && click < mag)
					currentTouch.clickNotification = ClickNotification.None;
			}
		}
	}

	/// <summary>Process the release part of a touch.</summary>
	private void ProcessRelease(bool isMouse, float drag) {
		// Send out the unpress message
		if(currentTouch == null) return;
		currentTouch.pressStarted = false;

		if(currentTouch.pressed != null) {
			// If there was a drag event in progress, make sure OnDragOut gets sent
			if(currentTouch.dragStarted) {
				if(onDragOut != null) onDragOut(currentTouch.last, currentTouch.dragged);
				Notify(currentTouch.last, "OnDragOut", currentTouch.dragged);

				if(onDragEnd != null) onDragEnd(currentTouch.dragged);
				Notify(currentTouch.dragged, "OnDragEnd", null);
			}

			// Send the notification of a touch ending
			if(onPress != null) onPress(currentTouch.pressed, false);
			Notify(currentTouch.pressed, "OnPress", false);

			// Send a hover message to the object
			if(isMouse) {
#if UNITY_5_5_OR_NEWER
				UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
				var hasCollider = HasCollider(currentTouch.pressed);
				UnityEngine.Profiling.Profiler.EndSample();
#else
				Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
				var hasCollider = HasCollider(currentTouch.pressed);
				Profiler.EndSample();
#endif

				if(hasCollider) {
					// OnHover is sent to restore the visual state
					if(mHover == currentTouch.current) {
						if(onHover != null) onHover(currentTouch.current, true);
						Notify(currentTouch.current, "OnHover", true);
					}
					else {
						hoveredObject = currentTouch.current;
					}
				}
			}

			// If the button/touch was released on the same object, consider it a click and select it
			if(currentTouch.dragged == currentTouch.current ||
			   currentScheme != ControlScheme.Controller &&
			   currentTouch.clickNotification != ClickNotification.None &&
			   currentTouch.totalDelta.sqrMagnitude < drag) {
				// If the touch should consider clicks, send out an OnClick notification
				if(currentTouch.clickNotification != ClickNotification.None && currentTouch.pressed == currentTouch.current) {
					ShowTooltip(null);
					var time = RealTime.time;

					if(onClick != null) onClick(currentTouch.pressed);
					Notify(currentTouch.pressed, "OnClick", null);

					if(currentTouch.clickTime + 0.35f > time) {
						if(onDoubleClick != null) onDoubleClick(currentTouch.pressed);
						Notify(currentTouch.pressed, "OnDoubleClick", null);
					}
					currentTouch.clickTime = time;
				}
			}
			else if(currentTouch.dragStarted) // The button/touch was released on a different object
			{
				// Send a drop notification (for drag & drop)
				if(onDrop != null) onDrop(currentTouch.current, currentTouch.dragged);
				Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
			}
		}
		currentTouch.dragStarted = false;
		currentTouch.pressed = null;
		currentTouch.dragged = null;
	}

	private bool HasCollider(GameObject go) {
		if(go == null) return false;
		var c = go.GetComponent<Collider>();
		if(c != null) return c.enabled;
		var b = go.GetComponent<Collider2D>();
		return b != null && b.enabled;
	}

	/// <summary>Process the events of the specified touch.</summary>
	public void ProcessTouch(bool pressed, bool released) {
		if(released) mTooltipTime = 0f;

		// Whether we're using the mouse
		var isMouse = currentScheme == ControlScheme.Mouse;
		var drag = isMouse ? mouseDragThreshold : touchDragThreshold;
		var click = isMouse ? mouseClickThreshold : touchClickThreshold;

		// So we can use sqrMagnitude below
		drag *= drag;
		click *= click;

		if(currentTouch.pressed != null) {
			if(released) ProcessRelease(isMouse, drag);
			ProcessPress(pressed, click, drag);

			// Hold event = show tooltip
			if(tooltipDelay != 0f && currentTouch.deltaTime > tooltipDelay)
				if(currentTouch.pressed == currentTouch.current && mTooltipTime != 0f && !currentTouch.dragStarted) {
					mTooltipTime = 0f;
					currentTouch.clickNotification = ClickNotification.None;
					if(longPressTooltip) ShowTooltip(currentTouch.pressed);
					Notify(currentTouch.current, "OnLongPress", null);
				}
		}
		else if(isMouse || pressed || released) {
			ProcessPress(pressed, click, drag);
			if(released) ProcessRelease(isMouse, drag);
		}
	}

	/// <summary>Cancel the next tooltip, preventing it from being shown. Moving the mouse again will reset this counter.</summary>
	public static void CancelNextTooltip() {
		mTooltipTime = 0f;
	}

	/// <summary>Show or hide the tooltip.</summary>
	public static bool ShowTooltip(GameObject go) {
		if(mTooltip != go) {
			if(mTooltip != null) {
				if(onTooltip != null) onTooltip(mTooltip, false);
				Notify(mTooltip, "OnTooltip", false);
			}

			mTooltip = go;
			mTooltipTime = 0f;

			if(mTooltip != null) {
				if(onTooltip != null) onTooltip(mTooltip, true);
				Notify(mTooltip, "OnTooltip", true);
			}
			return true;
		}
		return false;
	}

	/// <summary>Hide the tooltip, if one is visible.</summary>
	public static bool HideTooltip() {
		return ShowTooltip(null);
	}
}