using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.onlineobject.objectnet {
#if UNITY_EDITOR
    /// <summary>
    /// This suppli extra methods used during Editor Window Draw.
    /// </summary>
    public static class EditorWindowExtensions {
        /// <summary>
        /// Gets all derived types.
        /// </summary>
        /// <param name="aAppDomain">a application domain.</param>
        /// <param name="aType">a type.</param>
        /// <returns>System.Type[].</returns>
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType) {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.IsSubclassOf(aType)) {
                        result.Add(type);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the editor main window position.
        /// </summary>
        /// <returns>Rect.</returns>
        /// <exception cref="System.MissingMemberException">Can't find internal type ContainerWindow. Maybe something has changed inside Unity</exception>
        /// <exception cref="System.MissingFieldException">Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity</exception>
        /// <exception cref="System.NotSupportedException">Can't find internal main window. Maybe something has changed inside Unity</exception>
        public static Rect GetEditorMainWindowPos() {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null) {
                throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            }
            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows) {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }
            throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        /// <summary>
        /// Centers the on main win.
        /// </summary>
        /// <param name="targetWindow">The target window.</param>
        public static void CenterOnMainWin(this UnityEditor.EditorWindow targetWindow) {
            if (targetWindow != null) {
                var main = GetEditorMainWindowPos();
                var pos = targetWindow.position;
                float w = (main.width - pos.width) * 0.5f;
                float h = (main.height - pos.height) * 0.5f;
                pos.x = main.x + w;
                pos.y = main.y + h;
                targetWindow.position = pos;
            }
        }
    }
#endif
}