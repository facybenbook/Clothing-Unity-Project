using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.PointerHandler
{
    using UnityEditor;

    [CustomEditor(typeof(DoubleClickHandler), true)]
    public class DoubleClickEditor : Editor
    {
        SerializedProperty m_OnClickProperty;
        protected void OnEnable ( )
        {
            m_OnClickProperty = serializedObject.FindProperty ( "m_OnDoubleClick" );
        }

        public override void OnInspectorGUI ( )
        {
           // base.OnInspectorGUI ( );
            EditorGUILayout.Space ( );

            serializedObject.Update ( );
            EditorGUILayout.PropertyField ( m_OnClickProperty );
            serializedObject.ApplyModifiedProperties ( );
        }

    }
}
