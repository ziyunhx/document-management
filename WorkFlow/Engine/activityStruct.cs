using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Engine
{
    public class activityStruct
    {
        public activityStruct parent { set; get; }

        public List<activityStruct> nodes { set; get; }

        public string displayName { set; get; }

        public Activity currentActivity { set; get; }

        public activityStruct()
        {
            nodes = new List<activityStruct>();
        }




        static void printActivityTree(Activity activity, string tag)
        {
            Console.WriteLine("{0} DisplayName:{1},type:{2}", tag, activity.DisplayName, activity.GetType());

            System.Collections.Generic.IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(activity).GetEnumerator();

            while (list.MoveNext())
            {
                printActivityTree(list.Current, "  " + tag);
            }
        }

        public static void printActivityTree(Activity activity)
        {
            printActivityTree(activity, "|--");
        }

        static activityStruct tag;
        public static void printActivityTreeA(Activity activity)
        {
            if (tag == null)
            {
                tag = new activityStruct();
                tag.parent = null;
                tag.currentActivity = activity;
                tag.displayName = activity.DisplayName;

            }


            System.Collections.Generic.IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(activity).GetEnumerator();

            while (list.MoveNext())
            {
                activityStruct temp = new activityStruct();

                temp.parent = tag;
                temp.currentActivity = list.Current;
                temp.displayName = list.Current.DisplayName;
                printActivityTreeA(list.Current);

            }

        }




    }
}
