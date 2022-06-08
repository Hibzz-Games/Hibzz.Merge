using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hibzz.Merge.Singleton
{
    public class Simple<T> where T : new()
    {
        /// <summary>
        /// An instance of the singleton
        /// </summary>
        protected static T instance;

        /// <summary>
        /// Get an existing instance, or create a new one
        /// </summary>
        /// <returns>The singleton instance</returns>
        public static T GetOrCreateInstance()
		{
            instance ??= new T();
            return instance;
		}
    }
}
