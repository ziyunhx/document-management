//----------------------------------------------------------------

//----------------------------------------------------------------
namespace Machine.Design
{
    using System;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.View;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Machine.Design.FreeFormEditing;
    using System.Activities.Presentation;

    //This class is visual representation of ResizeGrip like control, which is used in a Grid to allow resizing.
    class ResizeGrip : Control
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(DrawingBrush), typeof(ResizeGrip));

        public static readonly DependencyProperty ParentStateContainerEditorProperty =
            DependencyProperty.Register("ParentStateContainerEditor", typeof(StateContainerEditor), typeof(ResizeGrip));

        public static readonly DependencyProperty DisabledProperty =
            DependencyProperty.Register("Disabled", typeof(bool), typeof(ResizeGrip), new UIPropertyMetadata(false));

        Point offset;

        public DrawingBrush Icon
        {
            get { return (DrawingBrush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public StateContainerEditor ParentStateContainerEditor
        {
            get { return (StateContainerEditor)GetValue(ParentStateContainerEditorProperty); }
            set { SetValue(ParentStateContainerEditorProperty, value); }
        }

        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e != null && !this.Disabled)
            {
                this.Cursor = Cursors.SizeNWSE;
                this.offset = e.GetPosition(this);
                this.CaptureMouse();
                // Select the designer when it is being resized
                WorkflowViewElement designer = this.ParentStateContainerEditor.ModelItem.View as WorkflowViewElement;
                Keyboard.Focus(designer);
                e.Handled = true;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (args != null && !this.Disabled)
            {
                if (args.LeftButton == MouseButtonState.Pressed && this.IsMouseCaptured)
                {
                    StateContainerEditor stateContainerEditor = this.ParentStateContainerEditor;
                    FreeFormPanel panel = stateContainerEditor.Panel;
                    Grid stateContainerGrid = stateContainerEditor.stateContainerGrid;
                    Point currentPosition = Mouse.GetPosition(stateContainerGrid);
                    currentPosition.Offset(this.offset.X, this.offset.Y);
                    stateContainerEditor.StateContainerWidth = Math.Min(Math.Max(panel.RequiredWidth, currentPosition.X), stateContainerGrid.MaxWidth);
                    stateContainerEditor.StateContainerHeight = Math.Min(Math.Max(panel.RequiredHeight, currentPosition.Y), stateContainerGrid.MaxHeight);
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e != null && !this.Disabled)
            {
                ModelItem stateContainerModelItem = this.ParentStateContainerEditor.ModelItem;
                // Save the new size to view state.
                using (ModelEditingScope scope = stateContainerModelItem.BeginEdit(SR.Resize))
                {
                    ViewStateService viewStateService = this.ParentStateContainerEditor.Context.Services.GetService<ViewStateService>();
                    viewStateService.StoreViewStateWithUndo(stateContainerModelItem, StateContainerEditor.StateContainerWidthViewStateKey, this.ParentStateContainerEditor.StateContainerWidth);
                    viewStateService.StoreViewStateWithUndo(stateContainerModelItem, StateContainerEditor.StateContainerHeightViewStateKey, this.ParentStateContainerEditor.StateContainerHeight);
                    scope.Complete();
                }
                Mouse.OverrideCursor = null;
                Mouse.Capture(null);
                e.Handled = true;
            }
            base.OnPreviewMouseLeftButtonUp(e);
        }
    }
}
