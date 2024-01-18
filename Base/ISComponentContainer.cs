using System;
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
    public interface ISComponentContainer<T> where T : ISComponentBase
    {
        public Com AddComponent<Com>() where Com : T;
        public T AddComponent(T Component);
        public T AddComponent(Type type);
        public void RemoveComponent(ushort UID);
        public void RemoveComponent<Com>() where Com : T;
        public void RemoveComponent(Type type);
        public T GetComponent(ushort UID);
        public T[] GetComponents<C>() where C : ISComponent;
        public T[] GetComponents(Type type);
        public Com GetComponent<Com>() where Com : T;
        public T GetComponent(Type type);
        public bool HasComponent(ushort UID);
        public bool HasComponent<Com>() where Com : T;
        public bool HasComponent(Type type);
        public T[] GetAllComponents();
    }
}
