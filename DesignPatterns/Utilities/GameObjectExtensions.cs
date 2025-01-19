using UnityEngine;
using System.Linq;

namespace HIEU_NL.Utilities {
    public static class GameObjectExtensions {
        /// <summary>
        /// This method is used to hide the GameObject in the Hierarchy view.
        /// </summary>
        public static void HideInHierarchy(this GameObject gameObject) {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
        /// </summary>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Returns the object itself if it exists, null otherwise.
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
        
        /// <summary>
        /// Check game object is not valid
        /// </summary>
        public static bool IsValid<T>(this T obj) where T : Object => obj != null;
        
        /// <summary>
        /// Check game object is not null
        /// </summary>
        public static bool IsNull<T>(this T obj) where T : Object => obj == null;

        /// <summary>
        /// Destroys all children of the game object
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildren(this GameObject gameObject) {
            gameObject.transform.DestroyChildren();
        }

        /// <summary>
        /// Immediately destroys all children of the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject) {
            gameObject.transform.DestroyChildrenImmediate();
        }

        /// <summary>
        /// Enables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
        public static void EnableChildren(this GameObject gameObject) {
            gameObject.transform.EnableChildren();
        }

        /// <summary>
        /// Disables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
        public static void DisableChildren(this GameObject gameObject) {
            gameObject.transform.DisableChildren();
        }

        /// <summary>
        /// Resets the GameObject's transform's position, rotation, and scale to their default values.
        /// </summary>
        /// <param name="gameObject">GameObject whose transformation is to be reset.</param>
        public static void ResetTransformation(this GameObject gameObject) {
            gameObject.transform.Reset();
        }

        /// <summary>
        /// Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObjects parent.</returns>
        public static string Path(this GameObject gameObject) {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }

        /// <summary>
        /// Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObject itself.</returns>
        public static string PathFull(this GameObject gameObject) {
            return gameObject.Path() + "/" + gameObject.name;
        }

        /// <summary>
        /// Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
        /// </summary>
        /// <param name="gameObject">The GameObject to set layers for.</param>
        /// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer) {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }
    }
}
