using ActivityLibrary;
using System;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Services;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace WFDesigner
{
    public partial class designerDebugTracking : TrackingParticipant
    {
        public DebuggerService debugService { get; set; }

        public BindingList<designerDebugTrackingData> trackingDataList { get; set; }

        public WorkflowDesigner designer { get; set; }

        //构造函数
        public designerDebugTracking(WorkflowDesigner designer)
        {
            //(1)
            this.designer = designer;
            this.debugService = designer.DebugManagerView as DebuggerService; ;

            //ziyunhx 2013-8-8 add debug status
            DebugStatus debug = new DebugStatus();
            debug.status = 1;

            //(2) TrackingProfile
            TrackingProfile trackingProfile = new TrackingProfile();
            ActivityStateQuery activityStateQuery = new ActivityStateQuery()
            {
                ActivityName = "*",
                States = { ActivityStates.Executing },
                Variables = { "*" },
                Arguments = { "*" }
            };
            trackingProfile.Queries.Add(activityStateQuery);

            this.TrackingProfile = trackingProfile;

            //(3)
            clearTrackInfo();

            //(4)
            sourceLocationList = getSourceLocationMap();
            activityMapList = getActivityMapList(sourceLocationList);

        } //end

        //清除Tracking信息
        public void clearTrackInfo()
        {
            if (trackingDataList == null)
            {
                trackingDataList = new BindingList<designerDebugTrackingData>();
            }
            else
            {
                trackingDataList.Clear();
            }
            WorkflowFileItem fileItem = designer.Context.Items.GetValue(typeof(WorkflowFileItem)) as WorkflowFileItem;
            designer.DebugManagerView.CurrentLocation = new SourceLocation(fileItem.LoadedFile, 1, 1, 1, 10);
        }


        //================

        private void AttachLocationPropertyValues(Dictionary<object, SourceLocation> sourceLocationMapping)
        {
            SourceLocation currentLocation;
            foreach (object instance in sourceLocationMapping.Keys)
            {
                currentLocation = sourceLocationMapping[instance];
                SetLocation(instance, currentLocation);
            }
        }

        private void SetLocation(object instance, SourceLocation location)
        {
            if (location != null)
            {
                XamlDebuggerXmlReader.SetFileName(instance, location.FileName);
                XamlDebuggerXmlReader.SetStartLine(instance, location.StartLine);
                XamlDebuggerXmlReader.SetStartColumn(instance, location.StartColumn);
                XamlDebuggerXmlReader.SetEndLine(instance, location.EndLine);
                XamlDebuggerXmlReader.SetEndColumn(instance, location.EndColumn);
            }
        }

        //===============

    } //end class


    //核心部分
    public partial class designerDebugTracking
    {
        Dictionary<string, Activity> activityMapList;

        Dictionary<object, SourceLocation> sourceLocationList;

        int step = 0;
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            {
                ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
                if (activityStateRecord != null)
                {
                    if (activityMapList.ContainsKey(activityStateRecord.Activity.Id))
                    {

                        designerDebugTrackingData trackingData = new designerDebugTrackingData(record, timeout, activityMapList[activityStateRecord.Activity.Id]);

                        step = step + 1;

                        trackingData.stepID = step.ToString();
                        trackingData.displayName = trackingData.Activity.DisplayName;
                        trackingData.state = ((ActivityStateRecord)trackingData.Record).State;
                        trackingData.sourceLocation = sourceLocationList[trackingData.Activity];
                        //
                        designer.View.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(() =>
                        {
                            designer.DebugManagerView.CurrentLocation = this.sourceLocationList[trackingData.Activity];
                            trackingDataList.Add(trackingData);
                            System.Threading.Thread.Sleep(1000);
                        }));
                    }
                }
            }
        }

        Dictionary<string, Activity> getActivityMapList(Dictionary<object, SourceLocation> sourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in sourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;

        } //end

        Dictionary<object, SourceLocation> getSourceLocationMap()
        {

            Dictionary<object, SourceLocation> runtime_debug = new Dictionary<object, SourceLocation>();
            Dictionary<object, SourceLocation> debug_debug = new Dictionary<object, SourceLocation>();

            WorkflowFileItem fileItem = designer.Context.Items.GetValue(typeof(WorkflowFileItem)) as WorkflowFileItem;

            Activity debugActivity = getDebugActivity();
            Activity runtimeActivity = getRuntimeActivity();

            SourceLocationProvider.CollectMapping(runtimeActivity, debugActivity, runtime_debug, fileItem.LoadedFile);

            SourceLocationProvider.CollectMapping(debugActivity, debugActivity, debug_debug, fileItem.LoadedFile);

            this.debugService.UpdateSourceLocations(debug_debug);

            return runtime_debug;
        } //end 

        Activity getDebugActivity()
        {
            ModelService modelService = designer.Context.Services.GetService<ModelService>();

            IDebuggableWorkflowTree debugTree = modelService.Root.GetCurrentValue() as IDebuggableWorkflowTree;

            if (debugTree != null)
            {
                return debugTree.GetWorkflowRoot();
            }
            else
            {
                return null;
            }

        } //end 

        Activity getDynamicActivity()
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(designer.Text);

            Activity root = System.Activities.XamlIntegration.ActivityXamlServices.Load(stringReader);

            WorkflowInspectionServices.CacheMetadata(root);

            return root;
        } //end 

        Activity getRuntimeActivity()
        {
            Activity root = getDynamicActivity();

            IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(root).GetEnumerator();

            list.MoveNext();

            Activity runtimeActivity = list.Current;

            return runtimeActivity;
        } //end 
    }//end class

    public class designerDebugTrackingData
    {
        public SourceLocation sourceLocation { get; set; }

        public string stepID { get; set; }

        public string displayName { get; set; }

        public string state { get; set; }

        public TrackingRecord Record { get; set; }

        public TimeSpan Timeout { get; set; }

        public Activity Activity { get; set; }


        public designerDebugTrackingData(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity)
        {
            this.Record = trackingRecord;
            this.Timeout = timeout;
            this.Activity = activity;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", stepID, displayName, state);
        }
    }//end class
}