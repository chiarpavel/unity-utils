using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public class MethodUtils : MonoBehaviour {
        private static MethodUtils instance;
        public static MethodUtils Singleton {
            get {
                if (instance == null) {
                    Debug.LogError("Singleton accessed before it was initialized. Make sure that the component is enabled.");
                }
                return instance;
            }
            private set {
                if (instance != null) {
                    Debug.LogWarning("Multiple instances detected. This may cause performance issues.");
                }
                instance = value;
            }
        }

        void Awake() {
            Singleton = this;
        }

        void Update() {
            ProcessDelays();
        }

        /// <summary>
        /// Creates a method that delays the invocation of the input method until a certain amount of time has passed since it was last called.
        /// </summary>
        /// <param name="func">The method to invoke</param>
        /// <param name="delay">The time in seconds to wait to invoke the method</param>
        /// <returns>The debounced method</returns>
        public Action Debounce(Action func, float delay) {
            bool pending = false;

            Action delayed = () => {
                func();
                pending = false;
            };

            int delayIndex = 0;

            Action debounced = () => {
                if (pending) {
                    RefreshDelay(delayIndex, delay);
                } else {
                    delayIndex = Delay(delayed, delay);
                    pending = true;
                }
            };

            return debounced;
        }

        private static int currentDelayID = 1;
        private static List<int> delayIDs = new List<int>();
        private static List<float> delayTimes = new List<float>();
        private static List<Action> delayActions = new List<Action>();
        private static void ProcessDelays() {
            if (delayIDs.Count == 0) return;
            for (int i = delayIDs.Count - 1; i >= 0; i--) {
                if (Time.time > delayTimes[i]) {
                    delayActions[i]();
                    delayIDs.RemoveAt(i);
                    delayTimes.RemoveAt(i);
                    delayActions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Invoke the input method after a delay.
        /// </summary>
        /// <param name="func">The method to invoke</param>
        /// <param name="delay">The delay in seconds</param>
        /// <returns>The ID of the delayed method</returns>
        public int Delay(Action func, float delay) {
            delayIDs.Add(currentDelayID);
            delayTimes.Add(Time.time + delay);
            delayActions.Add(func);
            return currentDelayID++;
        }

        /// <summary>
        /// Cancels a delayed method that has not been invoked yet.
        /// </summary>
        /// <param name="ID">The ID of the delayed method</param>
        /// <returns>Whether or not the delayed method was found and had not been invoked yet</returns>
        public bool CancelDelay(int ID) {
            int index = delayIDs.IndexOf(ID);
            if (index == -1) return false;
            delayIDs.RemoveAt(index);
            delayTimes.RemoveAt(index);
            delayActions.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Refreshes the invocation time of a delayed method.
        /// </summary>
        /// <param name="ID">The ID of the delayed method</param>
        /// <param name="delay">The new invocation time, in seconds from now</param>
        /// <returns>Whether or not the delayed method was found and had not been invoked yet</returns>
        public bool RefreshDelay(int ID, float delay) {
            int index = delayIDs.IndexOf(ID);
            if (index == -1) return false;
            delayTimes[index] = Time.time + delay;
            return true;
        }

        // ------------ static methods ----------------------------------------

        /// <summary>
        /// Creates a method that invokes the input method at most once per a specified amount of time.
        /// </summary>
        /// <param name="func">The method to invoke</param>
        /// <param name="delay">The minimum time in seconds between invocations</param>
        /// <returns>The throttled method</returns>
        public static Action Throttle(Action func, float delay) {
            float nextPossibleInvocation = Time.time;

            Action throttled = () => {
                if (Time.time > nextPossibleInvocation) {
                    func();
                    nextPossibleInvocation = Time.time + delay;
                }
            };

            return throttled;
        }

        /// <summary>
        /// Creates a method that caches the input method's computed result.
        /// </summary>
        /// <param name="func">The method to memoize</param>
        /// <param name="cacheSize">The maximum number of cached output values</param>
        /// <typeparam name="T0">The input type</typeparam>
        /// <typeparam name="T1">The output type</typeparam>
        /// <returns>The memoized method</returns>
        public static Func<T0, T1> Memoize<T0, T1>(Func<T0, T1> func, int cacheSize = 0) {
            Dictionary<T0, T1> cache = new Dictionary<T0, T1>();
            List<T0> accessList = new List<T0>();

            Func<T0, T1> memoized = (T0 input) => {
                if (cache.ContainsKey(input)) {
                    accessList.Remove(input);
                    accessList.Add(input);
                } else {
                    if (cacheSize != 0 && cache.Count > 0 && cache.Count >= cacheSize) {
                        cache.Remove(accessList[0]);
                        accessList.RemoveAt(0);
                    }
                    T1 output = func(input);
                    cache.Add(input, output);
                    accessList.Add(input);
                }
                return cache[input];
            };

            return memoized;
        }
    }
}
