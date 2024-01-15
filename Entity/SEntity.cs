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
    [Serializable]
    public sealed class SEntity : SComponentContainer<ISComponent>
    {
        public uint UID;

        public Action<SEntity> OnAddToWorld { get; set; }
        public Action<SEntity> OnRemoveFromWorld { get; set; }


        protected override void InitCom(ISComponent c)
        {
            c.SetEntity(this);
            base.InitCom(c);
        }
        protected override void OnRemove(ISComponent Com)
        {
            Com.OnRemoveFromEntity?.Invoke(Com);
            base.OnRemove(Com);
        }
    }
}
