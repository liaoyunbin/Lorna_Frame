using System.Collections.Generic;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public struct SignalArgs
    {
        public readonly PropertyName signal;
        public readonly GameObject invoker;
        public readonly GameObject Receiver;
        public List<NameVariable> Variables;

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public SignalArgs(PropertyName signal, GameObject invoker,GameObject receiver = null, List<NameVariable> variables = null)
        {
            this.signal = signal;
            this.invoker = invoker;
            this.Receiver = receiver;
            this.Variables = variables;
        }
    }
}