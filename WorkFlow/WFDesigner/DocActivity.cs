using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;

namespace WFDesigner
{
    [Designer(typeof(DocActivityDesigner))]
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
            string bookmark = context.GetValue(bookmarkName);
            context.CreateBookmark(bookmark, new BookmarkCallback(bookmarkCallback));
        }
        void bookmarkCallback(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            this.Result.Set(context, (string)obj);
        }
    }

}
