#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Foundation.FSM.Editor
{

    /// <summary>
    /// Editor window used to visualize and inspect runtime FSM (Finite State Machine) data.
    /// Provides tools for selecting FSM instances, viewing state history,
    /// and inspecting available state transitions.
    /// </summary>
    public class StateVisualizerWindow : EditorWindow
    {
        [MenuItem("Window/Core/State Visualizer")]
        public static void ShowWindow()
        {
            var window = GetWindow<StateVisualizerWindow>("State Visualizer");
            window.minSize = new Vector2(300, 575);
            window.Show();
        }

        private readonly IReadOnlyDictionary<Type, List<IFsmDebugView>> _contextType2DebugViews = FSMDebugRegistry.ContextType2DebugViews;

        private int _selectedKeyIndex;
        private int _selectedViewIndex;
        private Vector2 _transitionScroll;

        private IFsmDebugView _currentDebugView;


        // Draws all visualization sections and refreshes the view every frame.
        private void OnGUI()
        {
            if (_contextType2DebugViews == null || _contextType2DebugViews.Count == 0)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to register FSM debug views.", MessageType.Info);
                return;
            }

            DrawSection("State Machine", DrawTargetSection);
            DrawSection("State History", DrawStateSection);
            DrawSection("State Transitions", DrawTransitionSection);

            Repaint();
        }


        #region Sections

        // Draws the section responsible for selecting the context type and the specific FSM instance to inspect.
        private void DrawTargetSection()
        {
            // Draw the context type selector
            Type[] keys = _contextType2DebugViews.Keys.ToArray();
            string[] keyNames = keys.Select(k => k.Name).ToArray();

            _selectedKeyIndex = Mathf.Clamp(_selectedKeyIndex, 0, keys.Length - 1);
            _selectedKeyIndex = EditorGUILayout.Popup("Context Type", _selectedKeyIndex, keyNames);


            EditorGUILayout.Space(2.5f);
            

            // Draw the selected FSM instance
            List<IFsmDebugView> views = _contextType2DebugViews[keys[_selectedKeyIndex]];
            if (views == null || views.Count == 0)
            {
                EditorGUILayout.LabelField("No FSM debug views found.");
                return;
            }

            string[] viewNames = views.Select(v => v.Owner.name).ToArray();
            _selectedViewIndex = Mathf.Clamp(_selectedViewIndex, 0, views.Count - 1);
            _selectedViewIndex = EditorGUILayout.Popup("Object Name", _selectedViewIndex, viewNames);

            _currentDebugView = views[_selectedViewIndex];


            EditorGUILayout.Space(2.5f);
            

            // Focus the selected FSM owner in the Scene view
            if (GUILayout.Button("Focus Object"))
            {
                FocusFSMObject(_currentDebugView);
            }
        }

        // Draws the state history section. Displays the current state first, followed by previous states in a tree-like visual format.
        private void DrawStateSection()
        {
            if (_currentDebugView == null)
            {
                EditorGUILayout.HelpBox("Select an FSM instance to inspect.", MessageType.Info);
                return;
            }

            if (_currentDebugView.History == null || _currentDebugView.History.Count == 0)
            {
                EditorGUILayout.LabelField("This FSM has no State History");
                return;
            }

            string[] states = _currentDebugView.History.Select(state => state.Name).Reverse().ToArray();

            bool isCurrenState = true;
            foreach (string state in states)
            {
                if (isCurrenState)
                {
                    EditorGUILayout.LabelField($"▶ {state} (*)", EditorStyles.boldLabel);
                    isCurrenState = false;
                }
                else
                {
                    EditorGUILayout.LabelField($"└─ {state}", EditorStyles.label);
                }
            }
            
        }

        // Draws the state transitions section. Displays transition debug information in a read-only, scrollable text area.
        private void DrawTransitionSection()
        {
            if (_currentDebugView == null)
            {
                EditorGUILayout.HelpBox("Select an FSM instance to inspect.", MessageType.Info);
                return;
            }

            _transitionScroll = EditorGUILayout.BeginScrollView(_transitionScroll, GUILayout.MinHeight(300), GUILayout.MaxHeight(300));

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextArea(_currentDebugView.Transitions, GUILayout.ExpandHeight(true));
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion


        #region Helpers

        // Draws a boxed UI section with a title and custom content.
        private void DrawSection(string title, Action content)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                content.Invoke();
            }
        }

        // Focuses the Scene view camera on the selected FSM owner.
        private void FocusFSMObject(IFsmDebugView fsmDebugView)
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

            Selection.activeGameObject = fsmDebugView.Owner;
            EditorGUIUtility.PingObject(fsmDebugView.Owner);

            SceneView.lastActiveSceneView.LookAt(fsmDebugView.Owner.transform.position, SceneView.lastActiveSceneView.rotation, 5f, true);
        }

        #endregion
    }

    #endif
}
