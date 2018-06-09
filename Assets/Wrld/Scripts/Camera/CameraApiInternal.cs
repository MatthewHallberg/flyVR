using AOT;
using System;
using Wrld.Utilities;

namespace Wrld.MapCamera
{
    internal class CameraApiInternal
    {
        internal enum CameraEventType
        {
            Move,
            MoveStart,
            MoveEnd,
            Drag,
            DragStart,
            DragEnd,
            Pan,
            PanStart,
            PanEnd,
            Rotate,
            RotateStart,
            RotateEnd,
            Tilt,
            TiltStart,
            TiltEnd,
            Zoom,
            ZoomStart,
            ZoomEnd,
            TransitionStart,
            TransitionEnd
        };

        internal event Action OnTransitionStartInternal;
        internal event Action OnTransitionEndInternal;

        internal CameraApiInternal()
        {
            m_handleToSelf = NativeInteropHelpers.AllocateNativeHandleForObject(this);
        }

        internal delegate void CameraEventCallback(IntPtr cameraApiInternalHandle, CameraEventType eventId);

        [MonoPInvokeCallback(typeof(CameraEventCallback))]
        internal static void OnCameraEvent(IntPtr cameraApiInternalHandle, CameraEventType eventID)
        {
            var cameraApiInternal = cameraApiInternalHandle.NativeHandleToObject<CameraApiInternal>();

            if (eventID == CameraEventType.TransitionStart)
            {
                var startEvent = cameraApiInternal.OnTransitionStartInternal;

                if (startEvent != null)
                {
                    startEvent();
                }
            }
            else if (eventID == CameraEventType.TransitionEnd)
            {
                var endEvent = cameraApiInternal.OnTransitionEndInternal;

                if (endEvent != null)
                {
                    endEvent();
                }
            }
            // :TODO: handle other events
        }

        internal IntPtr GetHandle()
        {
            return m_handleToSelf;
        }

        internal void Destroy()
        {
            NativeInteropHelpers.FreeNativeHandle(m_handleToSelf);
        }

        private IntPtr m_handleToSelf;
    }
}