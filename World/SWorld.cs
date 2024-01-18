using System;
using System.Collections.Generic;
using System.Threading;
/*
# THIS FILE IS PART OF SimpleECS
# 
# THIS PROGRAM IS FREE SOFTWARE, WE USED APACHE2.0 LICENSE.
# YOU SHOULD HAVE RECEIVED A COPY OF APACHE2.0 LICENSE.
#
# THIS STATEMENT APPLIES TO THE ENTIRE PROJECT 
#
# Copyright (c) 2024 Fltoto
*/
namespace SimpleECS
{
    public class SWorld : SComponentContainer<ISGlobalComponent>
    {
        /// <summary>
        /// System执行FixedUpdate的间隔
        /// </summary>
        public static float FixedTime = 0.03f;

        public bool Running { get; private set; }
        public Action<SEntity> OnAddEntity, OnRemoveEntity;
        public int FPS { get; private set; }
        private int fps;
        private DateTime LastFPS;
        private DateTime LastFixed;
        private Thread main;

        private Dictionary<Type, List<ISComponent>> TDic = new Dictionary<Type, List<ISComponent>>();
        private Dictionary<uint, SEntity> IDEntities = new Dictionary<uint, SEntity>();
        private List<ISSystem> Systems = new List<ISSystem>();
        private List<ISGlobalSystem> GlobalSystems = new List<ISGlobalSystem>();

        /// <summary>
        /// 如果不启用async你需要在每一帧自己调用Update
        /// </summary>
        public void Start(bool async = true)
        {
            if (Running)
            {
                return;
            }
            Running = true;
            if (async)
            {
                main = new Thread(() =>
                {
                    Loop();
                });
            }
            Init();
            main?.Start();
        }
        public void Shutdown()
        {
            if (!Running)
            {
                return;
            }
            Running = false;
        }
        private void Loop()
        {
            while (Running)
            {
                Update();
            }
        }
        public virtual void Update()
        {
            fps++;
            if ((DateTime.Now - LastFPS).TotalSeconds > 1)
            {
                FPS = fps;
                fps = 0;
                LastFPS = DateTime.Now;
            }
            if ((DateTime.Now - LastFixed).TotalSeconds >= FixedTime)
            {
                LastFixed = DateTime.Now;
                FixedUpdateSystem();
            }
            UpdateSystem();
        }
        private void UpdateSystem()
        {
            lock (Systems)
            {
                lock (GlobalSystems)
                {
                    lock (TDic)
                    {
                        for (int i = 0; i < GlobalSystems.Count; i++)
                        {
                            var s = GlobalSystems[i];
                            var t = s.GetAimComponet();
                            var c = GetComponent(t);
                            if (c != null)
                            {
                                s.Update(c);
                            }
                        }
                        for (int i = 0; i < Systems.Count; i++)
                        {
                            var s = Systems[i];
                            var t = s.GetAimComponet();
                            if (TDic.ContainsKey(t))
                            {
                                lock (TDic[t])
                                {
                                    for (int j = 0; j < TDic[t].Count; j++)
                                    {
                                        if (j < TDic[t].Count)
                                        {
                                            s.Update(TDic[t][j]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void FixedUpdateSystem()
        {
            lock (Systems)
            {
                lock (GlobalSystems)
                {
                    lock (TDic)
                    {
                        for (int i = 0; i < GlobalSystems.Count; i++)
                        {
                            var s = GlobalSystems[i];
                            var t = s.GetAimComponet();
                            var c = GetComponent(t);
                            if (c != null)
                            {
                                s.Update(c);
                            }
                        }
                        for (int i = 0; i < Systems.Count; i++)
                        {
                            var s = Systems[i];
                            var t = s.GetAimComponet();
                            if (TDic.ContainsKey(t))
                            {
                                lock (TDic[t])
                                {
                                    for (int j = 0; j < TDic[t].Count; j++)
                                    {
                                        if (j < TDic[t].Count)
                                        {
                                            s.FixedUpdate(TDic[t][j]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public SEntity GetEntity(uint UID)
        {
            lock (IDEntities)
            {
                if (IDEntities.ContainsKey(UID))
                {
                    return IDEntities[UID];
                }
            }
            return null;
        }
        public void AddEntity(SEntity Entity)
        {
            lock (TDic)
            {
                lock (IDEntities)
                {
                    if (IDEntities.ContainsKey(Entity.UID))
                    {
                        return;
                    }
                    Entity.OnAddCom += (c) =>
                    {
                        if (!TDic.ContainsKey(c.GetType()))
                        {
                            TDic.Add(c.GetType(), new List<ISComponent>());
                        }
                        TDic[c.GetType()].Add(c);
                    };
                    Entity.OnRemoveCom += (c) =>
                    {
                        if (TDic.ContainsKey(c.GetType()))
                        {
                            TDic[c.GetType()].Remove(c);
                            if (TDic[c.GetType()].Count == 0)
                            {
                                TDic.Remove(c.GetType());
                            }
                        }
                    };
                    Entity.Init();
                    IDEntities.Add(Entity.UID, Entity);
                    Entity.OnAddToWorld?.Invoke(Entity);
                    OnAddEntity?.Invoke(Entity);
                }
            }
        }
        public void RemoveEntity(SEntity Entity)
        {
            RemoveEntity(Entity.UID);
        }
        public void RemoveEntity(uint UID)
        {
            lock (TDic)
            {
                lock (IDEntities)
                {
                    if (IDEntities.ContainsKey(UID))
                    {
                        var Entity = IDEntities[UID];
                        Entity.OnRemoveFromWorld?.Invoke(Entity);
                        IDEntities.Remove(UID);
                        lock (Entity.Components)
                        {
                            foreach (var c in Entity.Components)
                            {
                                if (TDic.ContainsKey(c.GetType()))
                                {
                                    lock (TDic[c.GetType()])
                                    {
                                        TDic[c.GetType()].Remove(c);
                                        if (TDic[c.GetType()].Count == 0)
                                        {
                                            TDic.Remove(c.GetType());
                                        }
                                    }
                                }
                            }
                            OnRemoveEntity?.Invoke(Entity);
                        }
                    }
                }
            }
        }

        public ISSystem AddSystem<T>() where T : ISSystem
        {
            return AddSystem(typeof(T));
        }
        public ISSystem AddSystem(Type type)
        {
            return AddSystem((ISSystem)Activator.CreateInstance(type)); ;
        }
        public ISSystem AddSystem(ISSystem sys)
        {
            lock (Systems)
            {
                Systems.Add(sys);
            }
            return sys;
        }
        public void RemoveSystem(ISSystem sys)
        {
            lock (Systems)
            {
                Systems.Remove(sys);
            }
        }
        public void RemoveSystem<T>() where T : ISSystem
        {
            lock (Systems)
            {
                var sys = Systems.Find(x => x.GetType() == typeof(T));
                if (sys == null)
                {
                    return;
                }
                Systems.Remove(sys);
            }
        }
        public ISSystem GetSystem<T>() where T : ISSystem
        {
            lock (Systems)
            {
                return Systems.Find(x => x.GetType() == typeof(T));
            }
        }

        public ISGlobalSystem AddGlobalSystem<T>() where T : ISGlobalSystem
        {
            return AddGlobalSystem(typeof(T));
        }
        public ISGlobalSystem AddGlobalSystem(Type type)
        {
            return AddGlobalSystem((ISGlobalSystem)Activator.CreateInstance(type)); ;
        }
        public ISGlobalSystem AddGlobalSystem(ISGlobalSystem sys)
        {
            lock (GlobalSystems)
            {
                GlobalSystems.Add(sys);
            }
            return sys;
        }
        public void RemoveGlobalSystem(ISGlobalSystem sys)
        {
            lock (GlobalSystems)
            {
                GlobalSystems.Remove(sys);
            }
        }
        public void RemoveGlobalSystem<T>() where T : ISGlobalSystem
        {
            lock (GlobalSystems)
            {
                var sys = GlobalSystems.Find(x => x.GetType() == typeof(T));
                if (sys == null)
                {
                    return;
                }
                GlobalSystems.Remove(sys);
            }
        }
        public ISGlobalSystem GetGlobalSystem<T>() where T : ISGlobalSystem
        {
            lock (GlobalSystems)
            {
                return GlobalSystems.Find(x => x.GetType() == typeof(T));
            }
        }

    }
}
