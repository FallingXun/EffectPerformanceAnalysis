using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class Pools
    {
        private static Dictionary<Type, Queue<object>> m_ObjectPool = new Dictionary<Type, Queue<object>>();

        public static T Get<T>() where T : new()
        {
            var type = typeof(T);
            if (m_ObjectPool.TryGetValue(type, out Queue<object> queue))
            {
                if (queue.Count > 0)
                {
                    bool init = false;
                    var obj = (T)queue.Dequeue();
                    if (type.IsGenericType)
                    {
                        var interfaces = type.GetInterfaces();
                        foreach (var item in interfaces)
                        {
                            if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(ICollection<>))
                            {
                                var method = type.GetMethod("Clear");
                                method.Invoke(obj, null);
                                init = true;
                            }
                        }
                    }
                    if (init == false)
                    {
                        Debug.LogErrorFormat("当前类型 {0} 未设置初始化方法！", type.ToString());
                    }
                    return (T)queue.Dequeue();
                }
            }
            return new T();
        }

        public static bool Release<T>(T obj) where T : new()
        {
            var type = typeof(T);
            if (m_ObjectPool.TryGetValue(type, out Queue<object> queue) == false)
            {
                queue = new Queue<object>();
            }
            queue.Enqueue(obj);
            return true;
        }
    }

}