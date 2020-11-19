using UnityEngine;
using UnityEditor;

// public class AnimUnhideTool : ScriptableObject
// {
//     //[MenuItem("Assets/Unhide Fix")]
//     private static void unhide()
//     {
//         UnityEditor.Animations.AnimatorController ac = Selection.activeObject as UnityEditor.Animations.AnimatorController;

//         foreach (UnityEditor.Animations.AnimatorControllerLayer layer in ac.layers)
//         {

//             foreach (UnityEditor.Animations.ChildAnimatorState curState in layer.stateMachine.states)
//             {

//                 if (curState.state.hideFlags != 0) curState.state.hideFlags = (HideFlags)1;
//             }
//         }
//         EditorUtility.SetDirty(ac);
//     }
// }