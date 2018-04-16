Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Interactivity
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Windows.Input
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpo
Imports DevExpress.Xpf.Core.WPFCompatibility

Namespace XPOInstant.SL
    Public Class XPOInstantModeCRUDBehavior
        Inherits Behavior(Of GridControl)

        Public Shared ReadOnly NewRowFormProperty As DependencyProperty = DependencyPropertyManager.Register("NewRowForm", GetType(DataTemplate), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(Nothing))
        Public Shared ReadOnly EditRowFormProperty As DependencyProperty = DependencyPropertyManager.Register("EditRowForm", GetType(DataTemplate), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(Nothing))
        Public Shared ReadOnly XPObjectTypeProperty As DependencyProperty = DependencyPropertyManager.Register("XPObjectType", GetType(Type), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(Nothing))
        Public Shared ReadOnly CollectionSourceProperty As DependencyProperty = DependencyPropertyManager.Register("CollectionSource", GetType(XPInstantFeedbackSource), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(Nothing))
        Public Shared ReadOnly AllowKeyDownActionsProperty As DependencyProperty = DependencyPropertyManager.Register("AllowKeyDownActions", GetType(Boolean), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(False))
        Public Shared ReadOnly PrimaryKeyProperty As DependencyProperty = DependencyPropertyManager.Register("PrimaryKey", GetType(String), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(String.Empty))
        Public Shared ReadOnly ServiceHelperProperty As DependencyProperty = DependencyPropertyManager.Register("ServiceHelper", GetType(IServiceHelper), GetType(XPOInstantModeCRUDBehavior), New SLPropertyMetadata(Nothing))

        Public Property NewRowForm() As DataTemplate
            Get
                Return CType(GetValue(NewRowFormProperty), DataTemplate)
            End Get
            Set(ByVal value As DataTemplate)
                SetValue(NewRowFormProperty, value)
            End Set
        End Property
        Public Property EditRowForm() As DataTemplate
            Get
                Return CType(GetValue(EditRowFormProperty), DataTemplate)
            End Get
            Set(ByVal value As DataTemplate)
                SetValue(EditRowFormProperty, value)
            End Set
        End Property
        Public Property XPObjectType() As Type
            Get
                Return CType(GetValue(XPObjectTypeProperty), Type)
            End Get
            Set(ByVal value As Type)
                SetValue(XPObjectTypeProperty, value)
            End Set
        End Property
        Public Property CollectionSource() As XPInstantFeedbackSource
            Get
                Return CType(GetValue(CollectionSourceProperty), XPInstantFeedbackSource)
            End Get
            Set(ByVal value As XPInstantFeedbackSource)
                SetValue(CollectionSourceProperty, value)
            End Set
        End Property
        Public Property AllowKeyDownActions() As Boolean
            Get
                Return CBool(GetValue(AllowKeyDownActionsProperty))
            End Get
            Set(ByVal value As Boolean)
                SetValue(AllowKeyDownActionsProperty, value)
            End Set
        End Property
        Public Property PrimaryKey() As String
            Get
                Return CStr(GetValue(PrimaryKeyProperty))
            End Get
            Set(ByVal value As String)
                SetValue(PrimaryKeyProperty, value)
            End Set
        End Property
        Public Property ServiceHelper() As IServiceHelper
            Get
                Return DirectCast(GetValue(ServiceHelperProperty), IServiceHelper)
            End Get
            Set(ByVal value As IServiceHelper)
                SetValue(ServiceHelperProperty, value)
            End Set
        End Property

        Public ReadOnly Property Grid() As GridControl
            Get
                Return AssociatedObject
            End Get
        End Property
        Public ReadOnly Property View() As TableView
            Get
                Return If(Grid IsNot Nothing, CType(Grid.View, TableView), Nothing)
            End Get
        End Property
        Public ServerCollectionSource As XPServerCollectionSource
        Public EditRowItem As Object

        #Region "Commands"
        Private privateNewRowCommand As CustomCommand
        Public Property NewRowCommand() As CustomCommand
            Get
                Return privateNewRowCommand
            End Get
            Private Set(ByVal value As CustomCommand)
                privateNewRowCommand = value
            End Set
        End Property
        Private privateRemoveRowCommand As CustomCommand
        Public Property RemoveRowCommand() As CustomCommand
            Get
                Return privateRemoveRowCommand
            End Get
            Private Set(ByVal value As CustomCommand)
                privateRemoveRowCommand = value
            End Set
        End Property
        Private privateEditRowCommand As CustomCommand
        Public Property EditRowCommand() As CustomCommand
            Get
                Return privateEditRowCommand
            End Get
            Private Set(ByVal value As CustomCommand)
                privateEditRowCommand = value
            End Set
        End Property
        Protected Overridable Sub ExecuteNewRowCommand()
            AddNewRow()
        End Sub
        Protected Overridable Function CanExecuteNewRowCommand() As Boolean
            If CollectionSource Is Nothing Then
                Return False
            End If
            Return True
        End Function
        Protected Overridable Sub ExecuteRemoveRowCommand()
            RemoveSelectedRows()
        End Sub
        Protected Overridable Function CanExecuteRemoveRowCommand() As Boolean
            If CollectionSource Is Nothing OrElse Grid Is Nothing OrElse View Is Nothing OrElse View.FocusedRow Is Nothing Then
                Return False
            End If
            Return True
        End Function
        Protected Overridable Sub ExecuteEditRowCommand()
            EditRow()
        End Sub
        Protected Overridable Function CanExecuteEditRowCommand() As Boolean
            Return CanExecuteRemoveRowCommand()
        End Function
        #End Region

        Public Sub New()
            NewRowCommand = New CustomCommand(AddressOf ExecuteNewRowCommand, AddressOf CanExecuteNewRowCommand)
            RemoveRowCommand = New CustomCommand(AddressOf ExecuteRemoveRowCommand, AddressOf CanExecuteRemoveRowCommand)
            EditRowCommand = New CustomCommand(AddressOf ExecuteEditRowCommand, AddressOf CanExecuteEditRowCommand)
        End Sub
        Public Overridable Function CreateNewRow() As Object
            Return Activator.CreateInstance(XPObjectType, ServerCollectionSource.Session)
        End Function
        Public Sub AddNewRow(ByVal newRow As Object)
            If CollectionSource Is Nothing Then
                Return
            End If
            ServiceHelper.AddNewItems(newRow)
        End Sub
        Public Sub EditRow()
            If View Is Nothing OrElse View.FocusedRow Is Nothing Then
                Return
            End If
            ServiceHelper.GetItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString())
        End Sub
        Public Sub CreateEditDialog()
            Dim dialog As DXWindow = CreateDialogWindow(EditRowItem, True)
            AddHandler dialog.Closed, AddressOf OnEditRowDialogClosed
            dialog.ShowDialog()
        End Sub
        Public Sub AddNewRow()
            Dim dialog As DXWindow = CreateDialogWindow(CreateNewRow(), False)
            AddHandler dialog.Closed, AddressOf OnNewRowDialogClosed
            dialog.ShowDialog()
        End Sub
        Public Sub RemoveRow()
            If CollectionSource Is Nothing Then
                Return
            End If
            ServiceHelper.RemoveItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString())
        End Sub
        Public Overridable Sub RemoveSelectedRows()
            If CollectionSource Is Nothing Then
                Return
            End If
            Dim selectedRowsHandles() As Integer = View.GetSelectedRowHandles()
            If selectedRowsHandles IsNot Nothing OrElse selectedRowsHandles.Length > 0 Then
                Dim rowKeys As New List(Of String)()
                For Each index As Integer In selectedRowsHandles
                    Dim cellValue As Object = Grid.GetCellValue(index, PrimaryKey)
                    If cellValue Is Nothing Then
                        Return
                    End If
                    rowKeys.Add(cellValue.ToString())
                Next index
                For Each rowValue As String In rowKeys
                    ServiceHelper.RemoveItem(rowValue)
                Next rowValue
            ElseIf View.FocusedRow IsNot Nothing Then
                RemoveRow()
            End If
        End Sub
        Protected Overridable Function CreateDialogWindow(ByVal content As Object, Optional ByVal isEditingMode As Boolean = False) As DXWindow
            Dim dialog As DXDialog = New DXDialog With {.Tag = content, .Buttons = DialogButtons.OkCancel, .Title = If(isEditingMode, "Edit Row", "Add New Row")}
            Dim c As ContentControl = New ContentControl With {.Content = content}
            If isEditingMode Then
                dialog.Title = "Edit Row"
                c.ContentTemplate = EditRowForm
            Else
                dialog.Title = "Add New Row"
                c.ContentTemplate = NewRowForm
            End If
            dialog.Content = c
            Return dialog
        End Function
        Protected Overridable Sub OnNewRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
            RemoveHandler DirectCast(sender, DXWindow).Closed, AddressOf OnNewRowDialogClosed
            If DirectCast(sender, DXDialog).DialogResult = DialogResult.OK Then
                AddNewRow(DirectCast(sender, DXWindow).Tag)
            End If
        End Sub
        Protected Overridable Sub OnEditRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
            RemoveHandler DirectCast(sender, DXWindow).Closed, AddressOf OnEditRowDialogClosed
            If DirectCast(sender, DXDialog).DialogResult = DialogResult.OK Then
                ServiceHelper.EditItem(Grid.GetFocusedRowCellValue(PrimaryKey).ToString(), DirectCast(sender, DXWindow).Tag)
            End If
        End Sub
        Protected Overridable Sub OnViewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            If Not AllowKeyDownActions Then
                Return
            End If
            If e.Key = Key.Delete Then
                RemoveSelectedRows()
                e.Handled = True
            End If
            If e.Key = Key.Enter Then
                EditRow()
                e.Handled = True
            End If
        End Sub
        Protected Overridable Sub OnViewRowDoubleClick(ByVal sender As Object, ByVal e As RowDoubleClickEventArgs)
            EditRow()
            e.Handled = True
        End Sub
        Protected Overridable Sub OnGridLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            RemoveHandler Grid.Loaded, AddressOf OnGridLoaded
            Initialize()
        End Sub
        Protected Overridable Sub OnViewFocusedRowChanged(ByVal sender As Object, ByVal e As FocusedRowChangedEventArgs)
            RemoveRowCommand.RaiseCanExecuteChangedEvent()
            EditRowCommand.RaiseCanExecuteChangedEvent()
        End Sub
        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            If View IsNot Nothing AndAlso CollectionSource IsNot Nothing Then
                Initialize()
            Else
                AddHandler Grid.Loaded, AddressOf OnGridLoaded
            End If
        End Sub
        Protected Overrides Sub OnDetaching()
            Uninitialize()
            MyBase.OnDetaching()
        End Sub
        Private Sub Initialize()
            Grid.ItemsSource = CollectionSource
            NewRowCommand.RaiseCanExecuteChangedEvent()
            ServerCollectionSource = New XPServerCollectionSource(New UnitOfWork(), CollectionSource.ObjectType)
            AddHandler View.KeyDown, AddressOf OnViewKeyDown
            AddHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
            AddHandler View.FocusedRowChanged, AddressOf OnViewFocusedRowChanged
        End Sub
        Private Sub Uninitialize()
            RemoveHandler View.KeyDown, AddressOf OnViewKeyDown
            RemoveHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
            RemoveHandler View.FocusedRowChanged, AddressOf OnViewFocusedRowChanged
        End Sub
    End Class
    Public Class CustomCommand
        Implements ICommand

        Private _executeMethod As Action
        Private _canExecuteMethod As Func(Of Boolean)
        Public Sub New(ByVal executeMethod As Action, ByVal canExecuteMethod As Func(Of Boolean))
            _executeMethod = executeMethod
            _canExecuteMethod = canExecuteMethod
        End Sub
        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
            Return _canExecuteMethod()
        End Function
        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
            _executeMethod()
        End Sub
        Public Sub RaiseCanExecuteChangedEvent()
            RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
        End Sub
    End Class
    Public Interface IServiceHelper
        Sub AddNewItems(ByVal newItem As Object)
        Sub RemoveItem(ByVal id As String)
        Sub EditItem(ByVal oldId As String, ByVal itemValue As Object)
        Sub GetItem(ByVal id As String)
    End Interface
End Namespace