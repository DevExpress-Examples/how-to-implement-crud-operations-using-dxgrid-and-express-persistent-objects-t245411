using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpo;
using DevExpress.Xpf.Core.WPFCompatibility;

namespace XPOInstant.SL {
    public class XPOInstantModeCRUDBehavior: Behavior<GridControl> {
        public static readonly DependencyProperty NewRowFormProperty =
            DependencyPropertyManager.Register("NewRowForm", typeof(DataTemplate), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(null));
        public static readonly DependencyProperty EditRowFormProperty =
            DependencyPropertyManager.Register("EditRowForm", typeof(DataTemplate), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(null));
        public static readonly DependencyProperty XPObjectTypeProperty =
            DependencyPropertyManager.Register("XPObjectType", typeof(Type), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(null));
        public static readonly DependencyProperty CollectionSourceProperty =
            DependencyPropertyManager.Register("CollectionSource", typeof(XPInstantFeedbackSource), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(null));
        public static readonly DependencyProperty AllowKeyDownActionsProperty =
            DependencyPropertyManager.Register("AllowKeyDownActions", typeof(bool), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(false));
        public static readonly DependencyProperty PrimaryKeyProperty =
            DependencyPropertyManager.Register("PrimaryKey", typeof(string), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ServiceHelperProperty =
            DependencyPropertyManager.Register("ServiceHelper", typeof(IServiceHelper), typeof(XPOInstantModeCRUDBehavior), new SLPropertyMetadata(null));

        public DataTemplate NewRowForm {
            get { return (DataTemplate)GetValue(NewRowFormProperty); }
            set { SetValue(NewRowFormProperty, value); }
        }
        public DataTemplate EditRowForm {
            get { return (DataTemplate)GetValue(EditRowFormProperty); }
            set { SetValue(EditRowFormProperty, value); }
        }
        public Type XPObjectType {
            get { return (Type)GetValue(XPObjectTypeProperty); }
            set { SetValue(XPObjectTypeProperty, value); }
        }
        public XPInstantFeedbackSource CollectionSource {
            get { return (XPInstantFeedbackSource)GetValue(CollectionSourceProperty); }
            set { SetValue(CollectionSourceProperty, value); }
        }
        public bool AllowKeyDownActions {
            get { return (bool)GetValue(AllowKeyDownActionsProperty); }
            set { SetValue(AllowKeyDownActionsProperty, value); }
        }
        public string PrimaryKey {
            get { return (string)GetValue(PrimaryKeyProperty); }
            set { SetValue(PrimaryKeyProperty, value); }
        }
        public IServiceHelper ServiceHelper {
            get { return (IServiceHelper)GetValue(ServiceHelperProperty); }
            set { SetValue(ServiceHelperProperty, value); }
        }

        public GridControl Grid { get { return AssociatedObject; } }
        public TableView View { get { return Grid != null ? (TableView)Grid.View : null; } }
        public XPServerCollectionSource ServerCollectionSource;
        public object EditRowItem;

        #region Commands
        public CustomCommand NewRowCommand { get; private set; }
        public CustomCommand RemoveRowCommand { get; private set; }
        public CustomCommand EditRowCommand { get; private set; }
        protected virtual void ExecuteNewRowCommand() {
            AddNewRow();
        }
        protected virtual bool CanExecuteNewRowCommand() {
            if(CollectionSource == null) return false;
            return true;
        }
        protected virtual void ExecuteRemoveRowCommand() {
            RemoveSelectedRows();
        }
        protected virtual bool CanExecuteRemoveRowCommand() {
            if(CollectionSource == null || Grid == null || View == null || View.FocusedRow == null) return false;
            return true;
        }
        protected virtual void ExecuteEditRowCommand() {
            EditRow();
        }
        protected virtual bool CanExecuteEditRowCommand() {
            return CanExecuteRemoveRowCommand();
        }
        #endregion

        public XPOInstantModeCRUDBehavior() {
            NewRowCommand = new CustomCommand(ExecuteNewRowCommand, CanExecuteNewRowCommand);
            RemoveRowCommand = new CustomCommand(ExecuteRemoveRowCommand, CanExecuteRemoveRowCommand);
            EditRowCommand = new CustomCommand(ExecuteEditRowCommand, CanExecuteEditRowCommand);
        }
        public virtual object CreateNewRow() {
            return Activator.CreateInstance(XPObjectType, ServerCollectionSource.Session);
        }
        public void AddNewRow(object newRow) {
            if(CollectionSource == null) return;
            ServiceHelper.AddNewItems(newRow);
        }
        public void EditRow() {
            if(View == null || View.FocusedRow == null) return;
            ServiceHelper.GetItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString());
        }
        public void CreateEditDialog() {
            DXWindow dialog = CreateDialogWindow(EditRowItem, true);
            dialog.Closed += OnEditRowDialogClosed;
            dialog.ShowDialog();
        }
        public void AddNewRow() {
            DXWindow dialog = CreateDialogWindow(CreateNewRow(), false);
            dialog.Closed += OnNewRowDialogClosed;
            dialog.ShowDialog();
        }
        public void RemoveRow() {
            if(CollectionSource == null) return;
            ServiceHelper.RemoveItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString());
        }
        public virtual void RemoveSelectedRows() {
            if(CollectionSource == null) return;
            int[] selectedRowsHandles = View.GetSelectedRowHandles();
            if(selectedRowsHandles != null || selectedRowsHandles.Length > 0) {
                List<string> rowKeys = new List<string>();
                foreach(int index in selectedRowsHandles) {
                    object cellValue = Grid.GetCellValue(index, PrimaryKey);
                    if(cellValue == null) return;
                    rowKeys.Add(cellValue.ToString());
                }
                foreach(string rowValue in rowKeys)
                    ServiceHelper.RemoveItem(rowValue);
            }
            else if(View.FocusedRow != null)
                RemoveRow();
        }
        protected virtual DXWindow CreateDialogWindow(object content, bool isEditingMode = false) {
            DXDialog dialog = new DXDialog {
                Tag = content,
                Buttons = DialogButtons.OkCancel,
                Title = isEditingMode ? "Edit Row" : "Add New Row",
            };
            ContentControl c = new ContentControl { Content = content };
            if(isEditingMode) {
                dialog.Title = "Edit Row";
                c.ContentTemplate = EditRowForm;
            }
            else {
                dialog.Title = "Add New Row";
                c.ContentTemplate = NewRowForm;
            }
            dialog.Content = c;
            return dialog;
        }
        protected virtual void OnNewRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnNewRowDialogClosed;
            if(((DXDialog)sender).DialogResult == DialogResult.OK)
                AddNewRow(((DXWindow)sender).Tag);
        }
        protected virtual void OnEditRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnEditRowDialogClosed;
            if(((DXDialog)sender).DialogResult == DialogResult.OK)
                ServiceHelper.EditItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString(), ((DXWindow)sender).Tag);
        }
        protected virtual void OnViewKeyDown(object sender, KeyEventArgs e) {
            if(!AllowKeyDownActions)
                return;
            if(e.Key == Key.Delete) {
                RemoveSelectedRows();
                e.Handled = true;
            }
            if(e.Key == Key.Enter) {
                EditRow();
                e.Handled = true;
            }
        }
        protected virtual void OnViewRowDoubleClick(object sender, RowDoubleClickEventArgs e) {
            EditRow();
            e.Handled = true;
        }
        protected virtual void OnGridLoaded(object sender, RoutedEventArgs e) {
            Grid.Loaded -= OnGridLoaded;
            Initialize();
        }
        protected virtual void OnViewFocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            RemoveRowCommand.RaiseCanExecuteChangedEvent();
            EditRowCommand.RaiseCanExecuteChangedEvent();
        }
        protected override void OnAttached() {
            base.OnAttached();
            if(View != null && CollectionSource != null)
                Initialize();
            else
                Grid.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching() {
            Uninitialize();
            base.OnDetaching();
        }
        void Initialize() {
            Grid.ItemsSource = CollectionSource;
            NewRowCommand.RaiseCanExecuteChangedEvent();
            ServerCollectionSource = new XPServerCollectionSource(new UnitOfWork(), CollectionSource.ObjectType);
            View.KeyDown += OnViewKeyDown;
            View.RowDoubleClick += OnViewRowDoubleClick;
            View.FocusedRowChanged += OnViewFocusedRowChanged;
        }
        void Uninitialize() {
            View.KeyDown -= OnViewKeyDown;
            View.RowDoubleClick -= OnViewRowDoubleClick;
            View.FocusedRowChanged -= OnViewFocusedRowChanged;
        }
    }
    public class CustomCommand: ICommand {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;
        public CustomCommand(Action executeMethod, Func<bool> canExecuteMethod) {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }
        public bool CanExecute(object parameter) {
            return _canExecuteMethod();
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) {
            _executeMethod();
        }
        public void RaiseCanExecuteChangedEvent() {
            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
    public interface IServiceHelper {
        void AddNewItems(object newItem);
        void RemoveItem(string id);
        void EditItem(string oldId, object itemValue);
        void GetItem(string id);
    }
}