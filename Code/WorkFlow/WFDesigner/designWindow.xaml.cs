using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Activities.Presentation.Services;
using System.Activities.Core.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Statements;
using System.Activities.Presentation.Metadata;
using System.Reflection;

using UserDesigner;
using WFDesigner.dialog;
using Engine;

namespace WFDesigner
{

    public partial class designWindow : Window
    {
        string workflowFilePathName = "tempRun.xaml";

        WorkflowDesigner designer;

        ModelEditingScope modelEditingScope;

        ModelItem rootModelItem;

        UndoEngine undoEngine;

        DesignerView designerView;

        ModelService modelService;

        ToolboxControl nodeToolboxControl;

       

        designerDebugTracking tracker;


        //构造函数
        public designWindow()
        {
            InitializeComponent();

            (new DesignerMetadata()).Register();


            (new Machine.Design.DesignerMetadata()).Register();



            this.DataContext = this;

            nodeToolboxControl = new ToolboxControl() { Categories = toolBox.loadToolbox() };
          
            nodePanel.Content = nodeToolboxControl;

           
      
        } //end

   
        //加载流程
        void loadWorkflowFromFile(string workflowFilePathName)
        {

            desienerPanel.Content = null;
            propertyPanel.Content = null;


            designer = new WorkflowDesigner();

           
            try
            {
                designer.Load(workflowFilePathName);

                modelService = designer.Context.Services.GetService<ModelService>();

                rootModelItem = modelService.Root;

                undoEngine = designer.Context.Services.GetService<UndoEngine>();
         
                undoEngine.UndoUnitAdded += delegate(object ss, UndoUnitEventArgs ee)
                                           {
                                               designer.Flush(); //调用Flush使designer.Text得到数据
                                               desigeerActionList.Items.Add(string.Format("{0}  ,   {1}", DateTime.Now.ToString(), ee.UndoUnit.Description));
                                           };

                designerView = designer.Context.Services.GetService<DesignerView>();



                designerView.WorkflowShellBarItemVisibility = ShellBarItemVisibility.Arguments    //如果不使用Activity做根,无法出现参数选项
                                                              | ShellBarItemVisibility.Imports
                                                              | ShellBarItemVisibility.MiniMap
                                                              | ShellBarItemVisibility.Variables
                                                               | ShellBarItemVisibility.Zoom
                                                              ;

                desienerPanel.Content = designer.View;

                propertyPanel.Content = designer.PropertyInspectorView;

   
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }  //end

        //保存流程
        void saveWorkflowToFile()
        {
            {
                if (workflowFilePathName == "tempRun.xaml")
                {
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    if (saveFileDialog.ShowDialog(this).Value)
                    {
                        designer.Save(saveFileDialog.FileName);
                        workflowFilePathName = saveFileDialog.FileName;
                        this.Title = "流程设计器  -   " + workflowFilePathName;
                    }
                }
                else
                {
                    designer.Save(workflowFilePathName);
                }

                loadWorkflowFromFile(workflowFilePathName);
            }
        }




    }//end class


    //UI响应
    public partial class designWindow
    {

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem == null)
            {
                return;
            }

            string menuItemValue = menuItem.Header.ToString();

            switch (menuItemValue)
            {
                //---------[工作流 ]---------------------

                case "新建":
                    {
                        dialog.createWorkflowWindow createWorkflowWindow = new dialog.createWorkflowWindow();

                        createWorkflowWindow.ShowDialog();

                        if (!string.IsNullOrEmpty(createWorkflowWindow.templateName))
                        {
                            loadWorkflowFromFile(createWorkflowWindow.templateName);
                        }
                        createWorkflowWindow.Close();


                        workflowFilePathName = "tempRun.xaml";

                        this.Title = "流程设计器: " + workflowFilePathName;
                    }
                    break;

                case "打开":
                    Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                    if (openDialog.ShowDialog(this).Value)
                    {
                        loadWorkflowFromFile(openDialog.FileName);
                        workflowFilePathName = openDialog.FileName;
                        this.Title = "流程设计器  -   " + workflowFilePathName;
                    }

                    break;

                case "保存":
                    saveWorkflowToFile();
                    MessageBox.Show("保存成功");
                    break;


                //---------[子流程 ]---------------------
                case "嵌入式子流程":

                    var list = designerTool.getSelectActivityList(designer);
                    Type type = typeof(System.Activities.Statements.Sequence);

                    foreach (var item in list)
                    {
                        if (item.ItemType == type)
                        {
                            dialog.openChildWorkflowWindow o = new dialog.openChildWorkflowWindow();
                            o.ShowDialog();
                            if (o.activity != null)
                            {
                                DynamicActivity d = o.activity as DynamicActivity;

                                if (d != null)
                                {
                                    foreach (var v in d.Properties)
                                    {
                                        if (v.Value is InArgument<string>)
                                        {
                                            Variable<string> t = new Variable<string>(v.Name);
                                            item.Properties["Variables"].Collection.Add(t);
                                        }



                                    }
                                    item.Properties["Activities"].Collection.Add(d.Implementation());

                                }
                                else
                                {
                                    item.Properties["Activities"].Collection.Add(o.activity);
                                }



                            }
                            o.Close();
                        }

                        break;

                    }
                    break;


                case "插入参数":
                    (new addArgumentWindow(this.designer)).ShowDialog();
                    saveWorkflowToFile();//不保存,参数已添加,但在流程设计器上不显示
                    break;

                //---------[操作 ]---------------------


                case "撤销":
                    undoEngine.Undo();
                    break;

                case "重做":
                    undoEngine.Redo();
                    break;

                case "清空流程设计跟踪":
                    desigeerActionList.Items.Clear();
                    break;





                //--------- [调试] -----------------
                case "运行":

                    saveWorkflowToFile();
                    runWorkflow();

                    break;


                case "清除跟踪信息":
                    trackingList.ItemsSource = null;
                    tracker.clearTrackInfo();
                    break;

                //--------- [查看] -----------------




                case "XAML":

                    (new Window() { Content = new TextBox() { Text = designer.Text, AcceptsReturn = true, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible } }).Show();
                    break;

                case "XAML(无ViewState)":

                    (new Window() { Content = new TextBox() { Text = tool.removeViewState(tool.activityBuilderFromXaml(designer.Text)), AcceptsReturn = true, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible } }).Show();
                    break;

                case "流程信息":
                    {
                        ListBox workflowInfo = new ListBox();

                        workflowInfo.Items.Add("Xaml File Path : " + designerTool.getXamlFilePath(designer));

                        workflowInfo.Items.Add("Activity Type : " + tool.activityByXaml(designer.Text).GetType().ToString());




                        (new Window() { Content = workflowInfo }).Show();
                    }
                    break;
                case "用户流程图":
                    {
                        displayFlowcharControl f = new displayFlowcharControl();
                        f.showFlowchar(tool.getFlowcharStruct(designer.Text));
                        (new Window() { Content = f }).Show();
                    }
                    break;

                case "运行信息":
                    (new Window() { Content = new Engine.runInfoControl() }).Show();
                    break;

                case "控制台输出":
                    (new Window() { Content = new Engine.controlControl() }).Show();
                    break;


                //--------- [工具栏] -----------------

                case "Auto ExpandCollapse":
                    nodeToolboxControl.CategoryItemStyle = new System.Windows.Style(typeof(TreeViewItem))
                                    {
                                        Setters = { new Setter(TreeViewItem.IsExpandedProperty, false)}
                                    };
                    break;
            } // end switch


        }//end

        //
        private void trackingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trackingList.SelectedItem != null)
            {
                designerDebugTrackingData td = trackingList.SelectedItem as designerDebugTrackingData;
                this.designer.DebugManagerView.CurrentLocation = td.sourceLocation;
            }
        } //end





    }//end class

    //流程启动部分
    public partial class designWindow
    {
        WorkflowApplication instance = null;

        //-----------------------------------------------------

        public void runWorkflow()
        {
            List<string> key = new List<string>();
          

            DynamicActivity dynamicActivity = tool.activityByXaml(designer.Text) as DynamicActivity;
           
            foreach (var inArgumen in dynamicActivity.Properties)
            {

                if (inArgumen.Value is InArgument<string>)
                {

                    key.Add(inArgumen.Name);
                }

            }
            tracker = new designerDebugTracking(designer);

            trackingList.ItemsSource = tracker.trackingDataList;

            dialog.startWorkflowWindow startWorkflowWindow = new dialog.startWorkflowWindow(key);
             startWorkflowWindow.ShowDialog();

                switch (startWorkflowWindow.selectButtonValue)
                {
                    case "参数启动":
                        
                      instance = Engine.engineManager.createInstance(designer.Text, startWorkflowWindow.dictionary, tracker);
                       
                        startWorkflowWindow.Close();
                        break;

                    case "无参数启动":
                      instance = Engine.engineManager.createInstance(designer.Text,null, tracker);
                      
                        startWorkflowWindow.Close();
                        break;

                    case "取消":
                   
                        startWorkflowWindow.Close();
                        return;

                    default :

                        startWorkflowWindow.Close();
                        return;
                }
                 

            instance.Run();
        } //end

        //
        private void submit_Click(object sender, RoutedEventArgs e)
        {
            string bookName = bookmarkNameTextbox.Text;
            string inputValue =submitTextbox.Text;

            if (instance != null)
            {
                if (instance.GetBookmarks().Count(p => p.BookmarkName == bookName) == 1)
                {
                    instance.ResumeBookmark(bookName, inputValue);
                }
                else
                {
                    foreach (var v in instance.GetBookmarks())
                    {
                        System.Console.WriteLine("--------请从下面选项中选择一BookmarkName---------------------------");
                        System.Console.WriteLine("BookmarkName:{0}:,OwnerDisplayName:{1}", v.BookmarkName, v.OwnerDisplayName);
                        System.Console.WriteLine("================================");
                    }
                }
            }
            else
            {
                MessageBox.Show("没有创建实例");
            }

        }//end
    }

    //测试部分
    public partial class designWindow
    {
        //
        private void find_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            string buttonValue = button.Content.ToString();

            switch (buttonValue)
            {
                case "查找根容器中所有WriteLine并赋值":
                    {
                        IEnumerable<ModelItem> list = modelService.Find(rootModelItem, typeof(WriteLine));

                        //读取属性
                        foreach (ModelItem item in list)
                        {
                            MessageBox.Show(item.Properties["DisplayName"].Value.ToString());
                        }

                        //属性赋值
                        foreach (ModelItem item in list)
                        {
                            InArgument<String> value = new InArgument<string>("wxwinter");

                            item.Properties["Text"].SetValue(value);
                        }

                    }
                    break;

                case "按名称查找并赋值":
                    {
                        MessageBox.Show("该功能没完成");
                        //   ModelItem item=  modelService.FromName(rootModelItem, findNameTextbox.Text);

                        //   MessageBox.Show(item.Properties["DisplayName"].Value.ToString());

                        //   item.Properties["DisplayName"].SetValue("wxd");
                    }
                    break;
            }
        }//end

        //
        private void edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            string buttonValue = button.Content.ToString();

            switch (buttonValue)
            {
                case "createStaticWorkflow":
                    loadWorkflowFromFile(@"template\sequence.xaml");
                    break;


                case "add":
                    {
                        modelEditingScope = rootModelItem.BeginEdit();

                        //如果容器是FlowChar,要添加的对象要用 FlowStep 包装,添加到 modelItem.Properties["Nodes"]中
                        // FlowStep flowCharNode = new FlowStep();
                        // flowCharNode.Action = new WriteLine { Text = "这是新增加的部分?" };
                        // rootModelItem.Properties["Nodes"].Collection.Add(flowCharNode);

                        //如果容器是Sequence,直接向  modelItem.Properties["Activities"] 中添加
                        rootModelItem.Properties["Activities"].Collection.Add(new Flowchart());

                    }
                    break;

                case "complete":
                    {
                        modelEditingScope.Complete();
                    }
                    break;

                case "revert":
                    {
                        modelEditingScope.Revert();
                    }
                    break;
            }
        }//end
      
    }



  
}
