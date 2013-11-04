using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine
{
    using System.ComponentModel;
    using System.Activities;
    using System.Windows.Markup;

    public sealed class Transition
    {

        public string DisplayName
        {
            get;
            set;
        }


        Activity _Trigger=new System.Activities.Statements.Sequence();

       // [DefaultValue(null)]
        public Activity Trigger
        {
            get { return _Trigger; }
            set { _Trigger = value; }
        }

        [DependsOn("Trigger")]
        [DefaultValue(null)]
        public State To
        {
            get;
            set;
        }

        [DependsOn("To")]
        [DefaultValue(null)]
        public Activity Action
        {
            get;
            set;
        }

        [DependsOn("Action")]
        [DefaultValue(null)]
        public Activity<bool> Condition
        {
            get;
            set;
        }

        public State Source
        {
            get;
            set;
        }
    }
}
