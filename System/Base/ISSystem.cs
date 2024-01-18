using System;

namespace SimpleECS
{
    public interface ISSystem
    {
        public void Update(ISComponent Component);
        public Type GetAimComponet();
    }
}
