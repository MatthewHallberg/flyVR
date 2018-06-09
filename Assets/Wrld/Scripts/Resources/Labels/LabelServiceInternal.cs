using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Wrld.Interop;
using Wrld.Utilities;

namespace Wrld.Resources.Labels
{
    internal class LabelServiceInternal
    {
        private Dictionary<string, UnityEngine.UI.Text> m_screenTextObjects = new Dictionary<string, UnityEngine.UI.Text>();
        private Canvas m_unityCanvas = null;

        private IntPtr m_handleToSelf;

        private bool m_enableLabels;

        public LabelServiceInternal(GameObject unityCanvas, bool enabled)
        {
            if (unityCanvas == null)
            {
                if (enabled)
                {
                    Debug.LogWarning(
                        "No canvas was supplied to the label service.  Labels will not be displayed.\n" + 
                        "If you require labels, please add a canvas called \"Canvas\" to the scene, using \n" + 
                        "the \"Screen Space - Overlay\" render mode.");
                }
            }
            else
            {
                m_unityCanvas = unityCanvas.GetComponent<Canvas>();
            }

            m_handleToSelf = NativeInteropHelpers.AllocateNativeHandleForObject(this);
            m_enableLabels = enabled && m_unityCanvas != null;
        }

        public IntPtr GetHandle()
        {
            return m_handleToSelf;
        }

        public void AddLabel(string labelId, string labelText, Color haloColor, int baseFontSize, double fontScale)
        {
            if (m_enableLabels)
            {
                UnityEngine.UI.Text newScreenText = GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>("Labels/ScreenTextPrefab")).GetComponent<UnityEngine.UI.Text>();

                if (m_screenTextObjects.ContainsKey(labelId))
                {
                    DestroyLabel(labelId);
                }

                newScreenText.fontSize = baseFontSize;
                newScreenText.text = labelText;
                newScreenText.transform.SetParent(m_unityCanvas.transform, false);

                float fontScaleFactor = (float)fontScale / m_unityCanvas.scaleFactor;
                newScreenText.transform.localScale = new Vector3(fontScaleFactor, fontScaleFactor, fontScaleFactor);

                var outline = newScreenText.gameObject.GetComponent<UnityEngine.UI.Outline>();

                if (outline != null)
                {
                    outline.effectColor = haloColor;
                }

                // Unity UI - send to back
                newScreenText.transform.SetAsFirstSibling();

                m_screenTextObjects.Add(labelId, newScreenText);
            }
        }

        void UpdateLabel(string labelId, Color textColor, Vector2 position, float rotationAngleDegrees)
        {
            UnityEngine.UI.Text label;

            if (m_screenTextObjects.TryGetValue(labelId, out label))
            {
                float newX = position.x - (m_unityCanvas.pixelRect.width / 2);
                float newY = (m_unityCanvas.pixelRect.height - position.y) - (m_unityCanvas.pixelRect.height / 2);

                newX /= m_unityCanvas.scaleFactor;
                newY /= m_unityCanvas.scaleFactor;

                label.color = textColor;

                label.rectTransform.localPosition = new Vector3(newX, newY, 0);
                label.rectTransform.localRotation = Quaternion.Euler(0, 0, rotationAngleDegrees);
            }
        }

        public void DestroyLabel(string labelId)
        {
            if (m_screenTextObjects.ContainsKey(labelId))
            {
                if (!m_screenTextObjects[labelId].IsDestroyed())
                {
                    UnityEngine.Object.Destroy(m_screenTextObjects[labelId].gameObject);
                }
                m_screenTextObjects.Remove(labelId);
            }
        }

        internal void Destroy()
        {
            var keys = new List<string>(m_screenTextObjects.Keys);

            foreach (string labelId in keys)
            {
                DestroyLabel(labelId);
            }

            NativeInteropHelpers.FreeNativeHandle(m_handleToSelf);
        }

        public delegate void AddLabelDelegate(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId, [MarshalAs(UnmanagedType.LPStr)] string labelText, ref ColorInterop color, int baseFontSize, double fontScale);

        [MonoPInvokeCallback(typeof(AddLabelDelegate))]
        public static void AddLabel(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId, [MarshalAs(UnmanagedType.LPStr)] string labelText, ref ColorInterop color, int baseFontSize, double fontScale)
        {
            var labelService = labelServiceHandle.NativeHandleToObject<LabelServiceInternal>();
            labelService.AddLabel(labelId, labelText, color.ToColor(), baseFontSize, fontScale);
        }

        public delegate void UpdateLabelDelegate(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId, ref ColorInterop textColor, ref Vector2 position, float rotationAngleDegrees);

        [MonoPInvokeCallback(typeof(UpdateLabelDelegate))]
        public static void UpdateLabel(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId, ref ColorInterop textColor, ref Vector2 position, float rotationAngleDegrees)
        {
            var labelService = labelServiceHandle.NativeHandleToObject<LabelServiceInternal>();
            labelService.UpdateLabel(labelId, textColor.ToColor(), position, rotationAngleDegrees);
        }
        
        public delegate void RemoveLabelDelegate(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId);

        [MonoPInvokeCallback(typeof(RemoveLabelDelegate))]
        public static void RemoveLabel(IntPtr labelServiceHandle, [MarshalAs(UnmanagedType.LPStr)] string labelId)
        {
            var labelService = labelServiceHandle.NativeHandleToObject<LabelServiceInternal>();
            labelService.DestroyLabel(labelId);
        }
    }
}
