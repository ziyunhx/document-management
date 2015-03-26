using System.Activities;

namespace ActivityLibrary
{
    //[Designer(typeof(DocActivityDesigner))]
    public sealed class DocActivity : NativeActivity<string>
    {
        #region 自定义属性

        private string _stepId = "";

        /// <summary>
        /// 步骤ID
        /// </summary>
        public string StepID
        {
            get { return _stepId; }
            set { _stepId = value; }
        }

        #endregion

        public InArgument<string> bookmarkName { get; set; }

        protected override bool CanInduceIdle
        {
            get
            { return true; }
        }
        protected override void Execute(NativeActivityContext context)
        {
            DebugStatus debug = new DebugStatus();
            string bookmark = context.GetValue(bookmarkName);
            context.CreateBookmark(bookmark, new BookmarkCallback(bookmarkCallback));
            if (debug.status == 0)
            {
                //TODO Change to the choice step.
                //BLL.Document.DocumentStep(context.WorkflowInstanceId, Convert.ToInt32(this.StepID).ToString());
            }
        }
        void bookmarkCallback(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            this.Result.Set(context, (string)obj);
        }
    }

}
