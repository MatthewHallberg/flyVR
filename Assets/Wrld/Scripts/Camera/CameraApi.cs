using Wrld.Space;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Wrld.Helpers;

namespace Wrld.MapCamera
{
    /// <summary>
    /// Contains endpoints to interact with the map.
    /// </summary>
    public class CameraApi
    {
        private const string m_nonNullCameraMessage = "A non-null camera must be supplied, or have been set via SetControlledCamera.";

        public delegate void TransitionStartHandler(CameraApi cameraApi, UnityEngine.Camera camera);
        public delegate void TransitionEndHandler(CameraApi cameraApi, UnityEngine.Camera camera);

        /// <summary>
        /// Raised when a camera transition begins.
        /// </summary>
        public event TransitionStartHandler OnTransitionStart;

        /// <summary>
        /// Raised when a camera transition ends.
        /// </summary>
        public event TransitionEndHandler OnTransitionEnd;

        /// <summary>
        ///  Checks to see if the Wrld map controlled camera is currently undergoing a transition.
        /// </summary>
        public bool IsTransitioning { get; private set; }

        internal CameraApi(ApiImplementation apiImplementation, CameraApiInternal cameraApiInternal)
        {
            m_apiImplementation = apiImplementation;
            m_inputHandler = new CameraInputHandler();
            OnTransitionStart += TransitionStarted;
            OnTransitionEnd += TransitionEnded;
            m_cameraApiInternal = cameraApiInternal;
            m_cameraApiInternal.OnTransitionStartInternal += () => OnTransitionStart(this, m_controlledCamera);
            m_cameraApiInternal.OnTransitionEndInternal += () => OnTransitionEnd(this, m_controlledCamera);
            IsCameraDrivenFromInput = true;
        }

        /// <summary>
        /// Sets the camera that is then controlled by the Wrld map.
        /// </summary>
        /// <param name="camera">A Unity camera that can provide the frustum for streaming and is controlled by the Wrld map.</param>
        public void SetControlledCamera(UnityEngine.Camera camera)
        {
            m_controlledCamera = camera;
        }

        /// <summary>
        /// Controls whether or not to update the controlled camera in response to user mouse & touch events.
        /// </summary>
        public bool IsCameraDrivenFromInput { get; set; }

        /// <summary>
        /// Returns the camera that is currently being controlled by the map.
        /// </summary>
        public UnityEngine.Camera GetControlledCamera()
        {
            return m_controlledCamera;
        }

        /// <summary>
        /// Removes any Unity camera under control of the Wrld Map.
        /// </summary>
        public void ClearControlledCamera()
        {
            m_controlledCamera = null;
        }


        /// <summary>
        /// Transforms a point from local Unity space to a geographic coordinate using the supplied camera. If no camera is specified, the currently controlled camera will be used.
        /// Note: If using the ECEF coordinate system, the returned position will only be valid until the camera is moved. To robustly position an object on the map, use the GeographicTransform component.
        /// </summary>
        /// <param name="position">The geographical coordinates of the position to transform to local space.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed geographic LatLongAltitude in local space.</returns>
        [Obsolete("Please use SpacesApi.GeographicToWorldPoint() in the future.")]
        public Vector3 GeographicToWorldPoint(LatLongAltitude position, Camera camera = null)
        {
            return m_apiImplementation.SpacesApi.GeographicToWorldPoint(position);
        }

        /// <summary>
        /// Transforms a point from local Unity space to a geographic coordinate using the supplied camera. If no camera is specified, the currently controlled camera will be used.
        /// Note: If using the ECEF coordinate system, the returned position will only be valid until the camera is moved. To robustly position an object on the map, use the GeographicTransform component.
        /// </summary>
        /// <param name="position">The world position to transform into a geographic coordinate.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed world position as a LatLongAltitude.</returns>
        [Obsolete("Please use SpacesApi.WorldToGeographicPoint() in the future.")]
        public LatLongAltitude WorldToGeographicPoint(Vector3 position, Camera camera = null)
        {
            return m_apiImplementation.SpacesApi.WorldToGeographicPoint(position);
        }


        /// <summary>
        /// Transforms the supplied geographical coordinates into viewport space, using the supplied camera (if specified) or the camera previously set via SetControlledCamera if not.
        /// </summary>
        /// <param name="position">The geographical coordinates of the position to transform to viewport space.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed geographic LatLongAltitude in viewport space.</returns>
        public Vector3 GeographicToViewportPoint(LatLongAltitude position, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return m_apiImplementation.GeographicToViewportPoint(position, camera);
        }

        /// <summary>
        /// Transforms the supplied viewport space coordinates into LatLongAltitude geographical coordinates, using the supplied camera (if specified) or the camera previously set via SetControlledCamera if not.
        /// </summary>
        /// <param name="viewportSpacePosition">The viewport-space coordinates to transform to geographical coordinates.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed viewport space coordinates as a LatLongAltitude.</returns>
        public LatLongAltitude ViewportToGeographicPoint(Vector3 viewportSpacePosition, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return m_apiImplementation.ViewportToGeographicPoint(viewportSpacePosition, camera);
        }


        /// <summary>
        /// Transforms the supplied screen space coordinates into LatLongAltitude geographical coordinates, using the supplied camera (if specified) or the camera previously set via SetControlledCamera if not.
        /// </summary>
        /// <param name="screenSpacePosition">The screen space coordinates of the position to transform to geographical coordinates.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed screen space coordinates as a LatLongAltitude.</returns>
        public LatLongAltitude ScreenToGeographicPoint(Vector3 screenSpacePosition, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return ViewportToGeographicPoint(camera.ScreenToViewportPoint(screenSpacePosition), camera);
        }

        /// <summary>
        /// Transforms the supplied geographical coordinates into screen space, using the supplied camera (if specified) or the camera previously set via SetControlledCamera if not.
        /// </summary>
        /// <param name="position">The geographical coordinates of the position to transform to screen space.</param>
        /// <param name="camera">The camera for which to perform the transform. If this is left out, any camera which has previously been passed to SetControlledCamera will be used.</param>
        /// <returns>The transformed geographic LatLongAltitude in screen space.</returns>
        public Vector3 GeographicToScreenPoint(LatLongAltitude position, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return camera.ViewportToScreenPoint(GeographicToViewportPoint(position, camera));
        }

        /// <summary>
        /// Sets a custom render camera to be used with API points requiring screen space projection & unprojection.
        /// </summary>
        /// <param name="camera"></param>
        public void SetCustomRenderCamera(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera", "A non-null camera must be supplied.");
            }

            var cameraState = m_apiImplementation.GetStateForCustomCamera(camera);
            NativeCameraApi_SetCustomRenderCameraState(NativePluginRunner.API, cameraState);
        }

        /// <summary>
        /// Unsets the custom render camera to revert screen space API points to their default behaviour
        /// </summary>
        public void ClearCustomRenderCamera()
        {
            NativeCameraApi_ClearCustomRenderCamera(NativePluginRunner.API);
        }

        public void UpdateInput()
        {
            if (m_controlledCamera != null && IsCameraDrivenFromInput)
            {
                m_inputHandler.Update();
            }
        }


        public void Update(float deltaTime)
        {
            if (m_controlledCamera != null)
            {
                var cameraState = GetCurrentCameraState(NativePluginRunner.API);
                m_apiImplementation.ApplyNativeCameraState(cameraState, m_controlledCamera);
            }
        }

        /// <summary>
        /// Moves the camera to view the supplied interest point instantaneously, without any animation.
        /// Requires that a camera has been set using SetControlledCamera.
        /// </summary>
        /// <param name="interestPoint">The latitude and longitude of the point on the ground which the camera should look at.</param>
        /// <param name="distanceFromInterest">Optional. The distance in metres from the interest point at which the camera should sit. If unspecified/null the altitude is set to 0.0.</param>
        /// <param name="headingDegrees">Optional. The heading in degrees (0, 360) with which to view the target point, with 0 facing north, 90 east, etc. If unspecified/null the heading with which the camera&apos;s previous interest point was viewed will be maintained.</param>
        /// <param name="tiltDegrees">Optional. The camera tilt in degrees, where a value of 0 represents a camera looking straight down at the interest point, along the direction of gravity.</param>
        /// <returns>Whether the camera successfully moved or not.</returns>
        public bool MoveTo(
            LatLong interestPoint,
            double? distanceFromInterest = null,
            double? headingDegrees = null,
            double? tiltDegrees = null)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            var cameraUpdate = new CameraUpdate.Builder()
                .Target(interestPoint)
                .Distance(distanceFromInterest)
                .Bearing(headingDegrees)
                .Tilt(tiltDegrees)
                .Build();

            var cameraUpdateInterop = cameraUpdate.ToCameraUpdateInterop();

            NativeCameraApi_MoveCamera(
                NativePluginRunner.API,
                ref cameraUpdateInterop);

            return true;
        }

        /// <summary>
        /// Moves the camera to view the supplied interest point instantaneously, without any animation. Requires that a camera has been set using SetControlledCamera.
        /// </summary>
        /// <param name="interestPoint">The latitude and longitude of the point on the ground which the camera should look at.</param>
        /// <param name="cameraPosition">The latitude, longitude and altitude from which the camera will look at the interest point.</param>
        /// <returns>Whether the camera successfully moved or not.</returns>
        public bool MoveTo(
            LatLong interestPoint,
            LatLongAltitude cameraPosition)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            double distance;
            double headingDegrees;
            double tiltDegrees;
            GetTiltHeadingAndDistanceFromCameraAndTargetPosition(interestPoint, cameraPosition, out tiltDegrees, out headingDegrees, out distance);

            return MoveTo(interestPoint, distance, headingDegrees, tiltDegrees);
        }

        /// <summary>
        /// Smoothly animates the camera to view the supplied interest point. Requires that a camera has been set using SetControlledCamera.
        /// </summary>
        /// <param name="interestPoint">The latitude and longitude of the point on the ground which the camera should be looking at once the transition is complete.</param>
        /// <param name="distanceFromInterest">Optional. The distance in metres from the interest point at which the camera should sit. If unspecified/null the distance to the previous interest point is maintained.</param>
        /// <param name="headingDegrees">Optional. The heading in degrees (0, 360) with which to view the target point, with 0 facing north, 90 east, etc. If unspecified/null the heading with which the camera&apos;s previous interest point was viewed will be maintained.</param>
        /// <param name="tiltDegrees">Optional. The camera tilt in degrees, where a value of 0 represents a camera looking straight down at the interest point, along the direction of gravity.</param>
        /// <param name="transitionDuration">Optional. The total duration of the transition, in seconds. If not specified the duration will be calculated from the distance to be travelled and the camera&apos;s maximum speed.</param>
        /// <param name="jumpIfFarAway">Optional. By default AnimateTo will provide a smooth transition for short distances, but an instantaneous transition if there is a large distance to be covered (rather than waiting for a lengthy animation to play). If you want to override this behaviour and force an animation (even over large distances), you can set this to false.</param>
        /// <returns>Whether the camera successfully animated or not.</returns>
        public bool AnimateTo(
            LatLong interestPoint,
            double? distanceFromInterest = null,
            double? headingDegrees = null,
            double? tiltDegrees = null,
            double? transitionDuration = null,
            bool jumpIfFarAway = true)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            var cameraUpdate = new CameraUpdate.Builder()
                .Target(interestPoint)
                .Distance(distanceFromInterest)
                .Bearing(headingDegrees)
                .Tilt(tiltDegrees)
                .Build();

            var cameraAnimationOptions = new CameraAnimationOptions.Builder()
                .InterruptByGestureAllowed(true)
                .SnapIfDistanceExceedsThreshold(jumpIfFarAway)
                .Duration(transitionDuration)
                .Build();

            var cameraUpdateInterop = cameraUpdate.ToCameraUpdateInterop();
            var cameraAnimationOptionsInterop = cameraAnimationOptions.ToCameraAnimationOptionsInterop();

            NativeCameraApi_AnimateCamera(NativePluginRunner.API, ref cameraUpdateInterop, ref cameraAnimationOptionsInterop);

            return true;
        }

        /// <summary>
        /// Smoothly animates the camera to view the supplied interest point. Requires that a camera has been set using SetControlledCamera.
        /// </summary>
        /// <param name="interestPoint">The latitude and longitude of the point on the ground which the camera should be looking at once the transition is complete.</param>
        /// <param name="cameraPosition">The latitude, longitude and altitude from which the camera will look at the interest point when the transition is complete.</param>
        /// <param name="transitionDuration">Optional. The total duration of the transition, in seconds. If not specified the duration will be calculated from the distance to be travelled and the camera&apos;s maximum speed.</param>
        /// <param name="jumpIfFarAway">Optional. By default AnimateTo will provide a smooth transition for short distances, but an instantaneous transition if there is a large distance to be covered (rather than waiting for a lengthy animation to play). If you want to override this behaviour and force an animation (even over large distances), you can set this to false.</param>
        /// <returns>Whether the camera successfully animated or not.</returns>
        public bool AnimateTo(
            LatLong interestPoint,
            LatLongAltitude cameraPosition,
            double? transitionDuration = null,
            bool jumpIfFarAway = true)
        {
            double distance;
            double headingDegrees;
            double tiltDegrees;
            GetTiltHeadingAndDistanceFromCameraAndTargetPosition(interestPoint, cameraPosition, out tiltDegrees, out headingDegrees, out distance);

            return AnimateTo(interestPoint, distance, headingDegrees, tiltDegrees, transitionDuration, jumpIfFarAway);
        }

        /// <summary>
        /// Whether or not a camera has been set via SetControlledCamera.
        /// </summary>
        public bool HasControlledCamera { get { return m_controlledCamera != null; } }
        
        /// <summary>
        /// Registers a delegate which can control whether or not our built-in camera controls should respond. It is called every frame.
        /// </summary>        
        /// <param name="function">The delegate function to use. Should return a boolean value.</param>
        [Obsolete("Deprecated, please use SetShouldConsumeInputDelegate instead")]
        public void RegisterShouldConsumeInputDelegate(Func<bool> function)
        {
            m_inputHandler.RegisterShouldConsumeInputDelegate(function);
        }

        /// <summary>
        /// Unregisters the delegate which controls whether or not our built-in camera controls should respond.
        /// </summary>        
        /// <param name="function">The delegate function to unregister.</param>
        [Obsolete("Deprecated, please use ClearShouldConsumeInputDelegate instead")]
        public void UnregisterShouldConsumeInputDelegate(Func<bool> function)
        {
            m_inputHandler.UnregisterShouldConsumeInputDelegate(function);
        }

        /// <summary>
        /// Sets a delegate to can control whether or not our built-in camera controls should respond to input. It is called once a frame for
        /// mouse interfaces, and once for every active touch when using touch controls.  The pointer id associated with the input event is
        /// passed as a parameter.
        /// 
        /// If a delegate is supplied here, it will take precedence over anything supplied to the deprecated RegisterShouldConsumeInputDelegate.
        /// </summary>
        /// <param name="function">a method returning true if the camera should respond to the supplied input event and false otherwise</param>
        public void SetShouldConsumeInputDelegate(Func<int, bool> function)
        {
            m_inputHandler.SetShouldConsumeInputDelegate(function);
        }

        /// <summary>
        /// Clears any delegate previously supplied to SetShouldConsumeInputDelegate, with the result that the camera will always respond to input events
        /// if one has been supplied to SetControlledCamera.
        /// </summary>
        public void ClearShouldConsumeInputDelegate()
        {
            m_inputHandler.ClearShouldConsumeInputDelegate();
        }

        [DllImport(NativePluginRunner.DLL, CallingConvention = CallingConvention.StdCall)]
        private static extern void NativeCameraApi_MoveCamera(
            IntPtr ptr,
            ref CameraUpdateInterop cameraUpdate);

        [DllImport(NativePluginRunner.DLL, CallingConvention = CallingConvention.StdCall)]
        private static extern void NativeCameraApi_AnimateCamera(
            IntPtr ptr,
            ref CameraUpdateInterop cameraUpdate,
            ref CameraAnimationOptionsInterop cameraAnimationOptions);

        [DllImport(NativePluginRunner.DLL)]
        private static extern NativeCameraState GetCurrentCameraState(IntPtr ptr);

        [DllImport(NativePluginRunner.DLL)]
        private static extern void NativeCameraApi_SetCustomRenderCameraState(IntPtr ptr, CameraState cameraState);

        [DllImport(NativePluginRunner.DLL)]
        private static extern void NativeCameraApi_ClearCustomRenderCamera(IntPtr ptr);

        private void TransitionStarted(CameraApi controller, UnityEngine.Camera camera)
        {
            IsTransitioning = true;
        }

        private void TransitionEnded(CameraApi controller, UnityEngine.Camera camera)
        {
            IsTransitioning = false;
        }

        private static void GetTiltHeadingAndDistanceFromCameraAndTargetPosition(LatLong interestPoint, LatLongAltitude cameraPosition, out double tiltDegrees, out double headingDegrees, out double distance)
        {
            double distanceAlongGround = LatLong.EstimateGreatCircleDistance(interestPoint, cameraPosition.GetLatLong());
            double cameraAltitude = cameraPosition.GetAltitude();
            distance = Math.Sqrt(distanceAlongGround * distanceAlongGround + cameraAltitude * cameraAltitude);
            headingDegrees = cameraPosition.BearingTo(interestPoint);
            tiltDegrees = MathsHelpers.Rad2Deg(Math.PI * 0.5 - Math.Atan2(cameraAltitude, distanceAlongGround));
        }

        private UnityEngine.Camera m_controlledCamera;
        private ApiImplementation m_apiImplementation;
        private CameraInputHandler m_inputHandler;
        private CameraApiInternal m_cameraApiInternal;
    }
}
