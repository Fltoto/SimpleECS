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
using System;

namespace SimpleECS
{
    public interface ISComponent : ISComponentBase
    {
        public Action<ISComponent> OnRemoveFromEntity { get; set; }
        public void SetEntity(SEntity Entity);
        public SEntity GetEntity();
    }
}
