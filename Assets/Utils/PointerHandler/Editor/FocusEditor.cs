namespace Utils.PointerHandler

{
    using System;
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(FocusHandler), true)]
    public class FocusEditor : Editor
    {
        public override void OnInspectorGUI ( )
        {
            base.OnInspectorGUI ( );
            EditorGUILayout.Space ( );

            serializedObject.Update ( );
            serializedObject.ApplyModifiedProperties ( );
        }
    }
}