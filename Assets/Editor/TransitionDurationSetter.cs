using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[InitializeOnLoad]
public class TransitionDurationSetter : EditorWindow
{
    static TransitionDurationSetter()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if (Selection.activeObject is AnimatorController controller)
        {
            foreach (var layer in controller.layers)
            {
                foreach (var stateMachine in layer.stateMachine.stateMachines)
                {
                    foreach (var transition in stateMachine.stateMachine.anyStateTransitions)
                    {
                        if (transition.duration != 0)
                        {
                            Undo.RecordObject(controller, "Set Transition Duration to 0");
                            transition.duration = 0;
                            EditorUtility.SetDirty(controller);
                        }
                    }

                    foreach (var state in stateMachine.stateMachine.states)
                    {
                        foreach (var transition in state.state.transitions)
                        {
                            if (transition.duration != 0)
                            {
                                Undo.RecordObject(controller, "Set Transition Duration to 0");
                                transition.duration = 0;
                                EditorUtility.SetDirty(controller);
                            }
                        }
                    }
                }
            }
        }
    }
}