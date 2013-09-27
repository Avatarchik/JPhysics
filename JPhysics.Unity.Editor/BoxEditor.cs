﻿namespace JPhysics.Unity.Editor
{
    using UnityEditor;
    using UnityEngine;

    //WARNING: This code is taken from UnityEditor, only God knows how it works

    internal class BoxEditor
    {
#pragma warning disable 649
        private int m_ControlIdHint;
#pragma warning restore 649
        private bool m_UseLossyScale;

        public BoxEditor(bool useLossyScale)
        {
            this.m_UseLossyScale = useLossyScale;
        }

        private void AdjustEdgeHandleColor(Vector3 handlePos, Vector3 slideDir1, Vector3 slideDir2, Matrix4x4 transform, float alphaFactor)
        {
            bool flag;
            Vector3 inPoint = transform.MultiplyPoint(handlePos);
            Vector3 normalized = transform.MultiplyVector(slideDir1).normalized;
            Vector3 rhs = transform.MultiplyVector(slideDir2).normalized;
            if (Camera.current.isOrthoGraphic)
            {
                flag = (Vector3.Dot(-Camera.current.transform.forward, normalized) < 0f) && (Vector3.Dot(-Camera.current.transform.forward, rhs) < 0f);
            }
            else
            {
                Plane plane = new Plane(normalized, inPoint);
                Plane plane2 = new Plane(rhs, inPoint);
                flag = !plane.GetSide(Camera.current.transform.position) && !plane2.GetSide(Camera.current.transform.position);
            }
            if (flag)
            {
                alphaFactor *= 0.2f;
            }
            if (alphaFactor < 1f)
            {
                Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * alphaFactor);
            }
        }

        private void AdjustMidpointHandleColor(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform, float alphaFactor)
        {
            float num;
            Vector3 vector = transform.MultiplyPoint(localPos);
            Vector3 lhs = transform.MultiplyVector(localTangent);
            Vector3 rhs = transform.MultiplyVector(localBinormal);
            Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
            if (Camera.current.isOrthoGraphic)
            {
                num = Vector3.Dot(-Camera.current.transform.forward, normalized);
            }
            else
            {
                Vector3 vector6 = Camera.current.transform.position - vector;
                num = Vector3.Dot(vector6.normalized, normalized);
            }
            if (num < -0.0001f)
            {
                alphaFactor *= 0.2f;
            }
            if (alphaFactor < 1f)
            {
                Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * alphaFactor);
            }
        }

        public void DrawWireframeBox(Vector3 center, Vector3 siz)
        {
            Vector3 vector = (Vector3)(siz * 0.5f);
            Vector3[] points = new Vector3[] { center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, vector.y, -vector.z), center + new Vector3(vector.x, vector.y, -vector.z), center + new Vector3(vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, vector.y, vector.z), center + new Vector3(vector.x, vector.y, vector.z), center + new Vector3(vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, -vector.y, vector.z) };
            Handles.DrawPolyLine(points);
            Handles.DrawLine(points[1], points[6]);
            Handles.DrawLine(points[2], points[7]);
            Handles.DrawLine(points[3], points[8]);
        }

        private Vector3 EdgeHandle(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, Matrix4x4 transform)
        {
            Color color = Handles.color;
            bool flag = true;
            if (Camera.current != null)
            {
                Vector3 vector = Handles.matrix.inverse.MultiplyPoint(Camera.current.transform.position);
                Vector3 vector4 = handlePos - vector;
                Vector3 normalized = vector4.normalized;
                if (Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.Cross(slideDir1, slideDir2), normalized))) > 1.518436f)
                {
                    flag = false;
                }
            }
            float alphaFactor = !flag ? 0f : 1f;
            this.AdjustEdgeHandleColor(handlePos, slideDir1, slideDir2, transform, alphaFactor);
            int controlID = GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
            if (alphaFactor > 0f)
            {
                handlePos = Slider2D.Do(controlID, handlePos, handleDir, slideDir1, slideDir2, HandleUtility.GetHandleSize(handlePos) * 0.03f, new Handles.DrawCapFunction(Handles.DotCap), 0.1f, true);
            }
            Handles.color = color;
            return handlePos;
        }

        private void EdgeHandles(ref Vector3 minPos, ref Vector3 maxPos, Matrix4x4 transform)
        {
            Vector3 handleDir = new Vector3(1f, 0f, 0f);
            Vector3 vector2 = new Vector3(0f, 1f, 0f);
            Vector3 vector3 = new Vector3(0f, 0f, 1f);
            float z = (minPos.z + maxPos.z) * 0.5f;
            Vector3 handlePos = new Vector3(minPos.x, minPos.y, z);
            Vector3 vector5 = this.EdgeHandle(handlePos, handleDir, -handleDir, -vector2, transform);
            minPos.x = vector5.x;
            minPos.y = vector5.y;
            handlePos = new Vector3(minPos.x, maxPos.y, z);
            vector5 = this.EdgeHandle(handlePos, handleDir, -handleDir, vector2, transform);
            minPos.x = vector5.x;
            maxPos.y = vector5.y;
            handlePos = new Vector3(maxPos.x, maxPos.y, z);
            vector5 = this.EdgeHandle(handlePos, handleDir, handleDir, vector2, transform);
            maxPos.x = vector5.x;
            maxPos.y = vector5.y;
            handlePos = new Vector3(maxPos.x, minPos.y, z);
            vector5 = this.EdgeHandle(handlePos, handleDir, handleDir, -vector2, transform);
            maxPos.x = vector5.x;
            minPos.y = vector5.y;
            float y = (minPos.y + maxPos.y) * 0.5f;
            Vector3 vector6 = new Vector3(minPos.x, y, minPos.z);
            Vector3 vector7 = this.EdgeHandle(vector6, vector2, -handleDir, -vector3, transform);
            minPos.x = vector7.x;
            minPos.z = vector7.z;
            vector6 = new Vector3(minPos.x, y, maxPos.z);
            vector7 = this.EdgeHandle(vector6, vector2, -handleDir, vector3, transform);
            minPos.x = vector7.x;
            maxPos.z = vector7.z;
            vector6 = new Vector3(maxPos.x, y, maxPos.z);
            vector7 = this.EdgeHandle(vector6, vector2, handleDir, vector3, transform);
            maxPos.x = vector7.x;
            maxPos.z = vector7.z;
            vector6 = new Vector3(maxPos.x, y, minPos.z);
            vector7 = this.EdgeHandle(vector6, vector2, handleDir, -vector3, transform);
            maxPos.x = vector7.x;
            minPos.z = vector7.z;
            float x = (minPos.x + maxPos.x) * 0.5f;
            Vector3 vector8 = new Vector3(x, minPos.y, minPos.z);
            Vector3 vector9 = this.EdgeHandle(vector8, vector2, -vector2, -vector3, transform);
            minPos.y = vector9.y;
            minPos.z = vector9.z;
            vector8 = new Vector3(x, minPos.y, maxPos.z);
            vector9 = this.EdgeHandle(vector8, vector2, -vector2, vector3, transform);
            minPos.y = vector9.y;
            maxPos.z = vector9.z;
            vector8 = new Vector3(x, maxPos.y, maxPos.z);
            vector9 = this.EdgeHandle(vector8, vector2, vector2, vector3, transform);
            maxPos.y = vector9.y;
            maxPos.z = vector9.z;
            vector8 = new Vector3(x, maxPos.y, minPos.z);
            vector9 = this.EdgeHandle(vector8, vector2, vector2, -vector3, transform);
            maxPos.y = vector9.y;
            minPos.z = vector9.z;
        }

        private Vector3 MidpointHandle(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform)
        {
            Color color = Handles.color;
            float alphaFactor = 1f;
            this.AdjustMidpointHandleColor(localPos, localTangent, localBinormal, transform, alphaFactor);
            int controlID = GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
            if (alphaFactor > 0f)
            {
                Vector3 normalized = Vector3.Cross(localTangent, localBinormal).normalized;
                localPos = Slider1D.Do(controlID, localPos, normalized, HandleUtility.GetHandleSize(localPos) * 0.03f, (Handles.DotCap), 0.1f);
            }
            Handles.color = color;
            return localPos;
        }

        private void MidpointHandles(ref Vector3 minPos, ref Vector3 maxPos, Matrix4x4 transform)
        {
            Vector3 localTangent = new Vector3(1f, 0f, 0f);
            Vector3 vector2 = new Vector3(0f, 1f, 0f);
            Vector3 localBinormal = new Vector3(0f, 0f, 1f);
            Vector3 vector4 = (Vector3)((maxPos + minPos) * 0.5f);
            Vector3 localPos = new Vector3(maxPos.x, vector4.y, vector4.z);
            Vector3 vector6 = this.MidpointHandle(localPos, vector2, localBinormal, transform);
            maxPos.x = vector6.x;
            localPos = new Vector3(minPos.x, vector4.y, vector4.z);
            vector6 = this.MidpointHandle(localPos, vector2, -localBinormal, transform);
            minPos.x = vector6.x;
            localPos = new Vector3(vector4.x, maxPos.y, vector4.z);
            vector6 = this.MidpointHandle(localPos, localTangent, -localBinormal, transform);
            maxPos.y = vector6.y;
            localPos = new Vector3(vector4.x, minPos.y, vector4.z);
            vector6 = this.MidpointHandle(localPos, localTangent, localBinormal, transform);
            minPos.y = vector6.y;
            localPos = new Vector3(vector4.x, vector4.y, maxPos.z);
            vector6 = this.MidpointHandle(localPos, vector2, -localTangent, transform);
            maxPos.z = vector6.z;
            localPos = new Vector3(vector4.x, vector4.y, minPos.z);
            vector6 = this.MidpointHandle(localPos, vector2, localTangent, transform);
            minPos.z = vector6.z;
        }

        public bool OnSceneGUI(Transform transform, Color color, ref Vector3 center, ref Vector3 size)
        {
            return this.OnSceneGUI(transform, color, true, ref center, ref size);
        }

        public bool OnSceneGUI(Matrix4x4 transform, Color color, bool handlesOnly, ref Vector3 center, ref Vector3 size)
        {
            bool shift = Event.current.shift;
            //if ((!this.m_AlwaysDisplayHandles && !shift) && !flag)
            //{
            //    for (int i = 0; i < 6; i++)
            //    {
            //        GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
            //    }
            //    return false;
            //}
            Color color2 = Handles.color;
            Handles.color = color;
            Vector3 minPos = (Vector3)(center - (size * 0.5f));
            Vector3 maxPos = (Vector3)(center + (size * 0.5f));
            Matrix4x4 matrix = Handles.matrix;
            Handles.matrix = transform;
            int hotControl = GUIUtility.hotControl;
            if (handlesOnly)
            {
                this.DrawWireframeBox(((Vector3)((maxPos - minPos) * 0.5f)) + minPos, maxPos - minPos);
            }
            if (!shift) return false;
            this.MidpointHandles(ref minPos, ref maxPos, Handles.matrix);
            bool changed = GUI.changed;
            if (changed)
            {
                center = (Vector3)((maxPos + minPos) * 0.5f);
                size = maxPos - minPos;
            }
            Handles.color = color2;
            Handles.matrix = matrix;
            return changed;
        }

        public bool OnSceneGUI(Transform transform, Color color, bool handlesOnly, ref Vector3 center, ref Vector3 size)
        {
            if (this.m_UseLossyScale)
            {
                Matrix4x4 matrixx = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                size.Scale(transform.lossyScale);
                center = transform.TransformPoint(center);
                center = matrixx.inverse.MultiplyPoint(center);
                bool flag = this.OnSceneGUI(matrixx, color, handlesOnly, ref center, ref size);
                center = matrixx.MultiplyPoint(center);
                center = transform.InverseTransformPoint(center);
                size.Scale(new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z));
                return flag;
            }
            return this.OnSceneGUI(transform.localToWorldMatrix, color, handlesOnly, ref center, ref size);
        }

    }
}
