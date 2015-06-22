using UnityEngine;

namespace Pacman
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _i;
        private static object syncRoot = new object();

        public static T I
        {
            get
            {
                if (_i == null)
                {
                    lock (syncRoot)
                    {
                        if (_i == null)
                        {
                            var gameObject = new GameObject();
                            _i = gameObject.AddComponent<T>();
                            return _i;
                        }
                    }
                }

                return _i;
            }
        }
    }
}
