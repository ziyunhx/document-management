using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine
{
    public static class engineManager
    {
        static ObservableCollection<runInfo> _runInfoList = new ObservableCollection<runInfo>();

        public static ObservableCollection<runInfo> runInfoList
        {
            get { return _runInfoList; }
            set { _runInfoList = value; }
        }

        public static WorkflowApplication createInstance(string xamlString, Dictionary<string, object> dictionary, TrackingParticipant tracking)
        {
            WorkflowApplication instance = null;

            DynamicActivity dynamicActivity = tool.activityByXaml(xamlString) as DynamicActivity;

            if (dictionary != null)
            {
                instance = new WorkflowApplication(dynamicActivity, dictionary);
            }
            else
            {
                instance = new WorkflowApplication(dynamicActivity);
            }

            instance.Aborted = aborted;
            instance.Completed = completed;
            instance.OnUnhandledException = onUnhandledException;
            instance.PersistableIdle = persistableIdle;
            instance.Unloaded = unloaded;

            if (tracking != null)
            {
                instance.Extensions.Add(tracking);
            }

            return instance;
        }


        static void aborted(WorkflowApplicationAbortedEventArgs e)
        {
            writeRunInfo(e.InstanceId, "aborted", e.Reason.Message);
        }

        static void completed(WorkflowApplicationCompletedEventArgs e)
        {
            writeRunInfo(e.InstanceId, "completed", e.CompletionState.ToString());
        }

        static UnhandledExceptionAction onUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            writeRunInfo(e.InstanceId, "onUnhandledException", e.ExceptionSource.DisplayName + ":" + e.UnhandledException.Message);
            return UnhandledExceptionAction.Abort;
        }

        static PersistableIdleAction persistableIdle(WorkflowApplicationIdleEventArgs e)
        {
            string v = "";
            if (e.Bookmarks[0] != null)
            {
                v = e.Bookmarks[0].BookmarkName;
            }
            writeRunInfo(e.InstanceId, "persistableIdle", v);
            return PersistableIdleAction.Unload;
        }

        static void unloaded(WorkflowApplicationEventArgs e)
        {
            writeRunInfo(e.InstanceId, "unloaded", "");
        }


        public static void writeRunInfo(Guid instanceId, string type, string message)
        {
            if (runInfoList.Count == 0)
            {
                runInfoList.Add(new runInfo() { id = Guid.NewGuid(), time = System.DateTime.Now, instanceId = instanceId, type = type, message = message });

            }
            else
            {
                runInfoList.Insert(runInfoList.Count - 1, new runInfo() { id = Guid.NewGuid(), time = System.DateTime.Now, instanceId = instanceId, type = type, message = message });
            }
        }
    }




    public class runInfo : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        Guid _id;
        public Guid id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                this.RaisePropertyChanged("id");
            }
        }

        Guid _instanceId;
        public Guid instanceId
        {
            get
            {
                return _instanceId;
            }
            set
            {
                _instanceId = value;
                this.RaisePropertyChanged("instanceId");
            }
        }

        string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                this.RaisePropertyChanged("type");
            }
        }

        string _message;
        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("message");
            }
        }

        DateTime _time;
        public DateTime time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                this.RaisePropertyChanged("time");
            }
        }
    }
}
