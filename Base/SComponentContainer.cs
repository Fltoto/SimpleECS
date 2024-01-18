using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleECS
{
    public class SComponentContainer<T> : ISComponentContainer<T> where T : ISComponentBase
    {
        public List<T> Components = new List<T>();
        public Action<T> OnAddCom { get; set; }
        public Action<T> OnRemoveCom { get; set; }
        private Dictionary<Type, List<T>> Coms = new Dictionary<Type, List<T>>();
        private Dictionary<ushort, T> IDComs = new Dictionary<ushort, T>();


        public void Init()
        {
            lock (Coms)
            {
                lock (IDComs)
                {
                    Coms.Clear();
                    IDComs.Clear();
                    var l = Components.ToArray();
                    Components.Clear();
                    foreach (var c in l)
                    {
                        AddComponent(c);
                    }
                }
            }
        }
        /// <summary>
        /// 当这个容器触发Init之前你添加了Component，你的Component会触发两次Init.
        /// </summary>
        protected virtual void InitCom(T c)
        {
        }

        public Com AddComponent<Com>() where Com : T
        {
            return (Com)AddComponent(typeof(Com));
        }

        public T AddComponent(T Component)
        {
            lock (Coms)
            {
                lock (IDComs)
                {
                    if (IDComs.ContainsKey(Component.UID))
                    {
                    ID: Random rand = new Random();
                        Component.SetUID((ushort)rand.Next(0, 65534));
                        if (IDComs.ContainsKey(Component.UID))
                        {
                            goto ID;
                        }
                    }
                    IDComs.Add(Component.UID, Component);
                    if (!Coms.ContainsKey(Component.GetType()))
                    {
                        Coms.Add(Component.GetType(), new List<T>());
                    }
                    Coms[Component.GetType()].Add(Component);
                    Components.Add(Component);
                    OnAddCom?.Invoke(Component);
                    InitCom(Component);
                    return Component;
                }
            }
        }

        public T AddComponent(Type type)
        {
            return AddComponent((T)Activator.CreateInstance(type));
        }

        public T[] GetAllComponents()
        {
            lock (IDComs)
            {
                return IDComs.Values.ToArray();
            }
        }

        public T GetComponent(ushort UID)
        {
            lock (IDComs)
            {
                if (IDComs.ContainsKey(UID))
                {
                    return IDComs[UID];
                }
                return default(T);
            }
        }

        public Com GetComponent<Com>() where Com : T
        {
            return (Com)GetComponent(typeof(Com));
        }
        public T[] GetComponents<C>() where C : ISComponent {
            return GetComponents(typeof(C));
        }
        public T[] GetComponents(Type type)
        {
            if (Coms.ContainsKey(type))
            {
                return Coms[type].ToArray();
            }
            return null;
        }
        public T GetComponent(Type type)
        {
            lock (Coms)
            {
                if (Coms.ContainsKey(type))
                {
                    return Coms[type][0];
                }
                return default(T);
            }
        }

        public bool HasComponent(ushort UID)
        {
            return GetComponent(UID) != null;
        }

        public bool HasComponent<Com>() where Com : T
        {
            return GetComponent<Com>() != null;
        }

        public bool HasComponent(Type type)
        {
            return GetComponent(type) != null;
        }

        public void RemoveComponent(ushort UID)
        {
            lock (Coms)
            {
                lock (IDComs)
                {
                    if (IDComs.ContainsKey(UID))
                    {
                        var com = IDComs[UID];
                        IDComs.Remove(UID);
                        Coms[com.GetType()].Remove(com);
                        if (Coms[com.GetType()].Count == 0)
                        {
                            Coms.Remove(com.GetType());
                        }
                        Components.Remove(com);
                        OnRemoveCom?.Invoke(com);
                        OnRemove(com);
                    }
                }
            }
        }
        protected virtual void OnRemove(T Com) { 
        }
        public void RemoveComponent<Com>() where Com : T
        {
            RemoveComponent(typeof(Com));
        }

        public void RemoveComponent(Type type)
        {
            var c = GetComponent(type);
            if (c==null) {
                return;
            }
            RemoveComponent(GetComponent(type).UID);
        }
    }
}
