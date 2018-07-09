using UnityEngine;

namespace MIMIR.Util
{
    /// <summary>
    /// Base class for easy creation of singletons in Unity
    /// </summary>
    /// <typeparam name="T">The type of your script</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        protected virtual void Awake()
        {
            if ( instance != null )
            {
                Debug.LogErrorFormat("Trying to instantiante a second instance of {0} singleton", this.gameObject.name);
            }
            else
            {
                instance = (T) this;
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

       
    }
}
