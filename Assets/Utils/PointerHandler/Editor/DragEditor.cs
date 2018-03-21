using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.PointerHandler
{
    using UnityEditor;

    [CustomEditor(typeof(DragHandler), true)]
    public class DragEditor : Editor
    {
        SerializedProperty m_OnDragProperty;
        protected void OnEnable ( )
        {
            m_OnDragProperty = serializedObject.FindProperty ( "m_OnDrag" );
        }

        public override void OnInspectorGUI ( )
        {
            base.OnInspectorGUI ( );
            EditorGUILayout.Space ( );

            serializedObject.Update ( );
            //EditorGUILayout.PropertyField ( m_OnDragProperty );
            serializedObject.ApplyModifiedProperties ( );
        }
    }
}
