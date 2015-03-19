using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine
{
    using System.Activities;
    using System.Runtime.Serialization;


    [DataContract]
    public class TriggerCompletedEvent
    {
  
        [DataMember]
        public int TriggedId
        {
            get;
            set;
        }

        [DataMember]
        public Bookmark Bookmark
        {
            get;
            set;
        }
    }
}
