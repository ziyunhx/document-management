using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine
{
    using System.Activities;

 
    sealed class TransitionData
    {
  
        public Activity<bool> Condition
        {
            get;
            set;
        }


        public Activity Action
        {
            get;
            set;
        }


        public InternalState To
        {
            get;
            set;
        }
    }
}
