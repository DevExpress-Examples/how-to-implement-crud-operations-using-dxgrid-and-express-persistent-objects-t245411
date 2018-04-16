Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpf.Core.WPFCompatibility
Imports System.ServiceModel
Imports XPOInstant.SL.ServiceReference1
Imports DevExpress.Xpf.Core.ServerMode
Imports System.Data.Services.Client
Imports System.Threading
Imports System.Windows.Threading

Namespace XPOInstant.SL
    Partial Public Class MainPage
        Inherits UserControl

        Public Shared ReadOnly CollectionProperty As DependencyProperty = DependencyPropertyManager.Register("Collection", GetType(XPInstantFeedbackSource), GetType(MainPage), New SLPropertyMetadata(Nothing))
        Public Property Collection() As XPInstantFeedbackSource
            Get
                Return CType(GetValue(CollectionProperty), XPInstantFeedbackSource)
            End Get
            Set(ByVal value As XPInstantFeedbackSource)
                SetValue(CollectionProperty, value)
            End Set
        End Property
        Public Sub New()
            InitializeComponent()
            XPOServiceHelper.SetupXpoLayer(New UnitOfWork())
            Collection = New XPInstantFeedbackSource(GetType(Items))
            AddHandler Collection.ResolveSession, AddressOf OnCollectionResolveSession
            AddHandler Collection.DismissSession, AddressOf OnCollectionDismissSession

            helper.XPObjectType = GetType(Items)
            helper.CollectionSource = Collection
            helper.ServiceHelper = New ServiceHelper(helper, New Uri("http://localhost:54177/WcfDataService.svc/"), Dispatcher)
        End Sub
        Private Sub OnCollectionDismissSession(ByVal sender As Object, ByVal e As ResolveSessionEventArgs)
            Dim session As IDisposable = TryCast(e.Session, IDisposable)
            If session IsNot Nothing Then
                session.Dispose()
            End If
        End Sub
        Private Sub OnCollectionResolveSession(ByVal sender As Object, ByVal e As ResolveSessionEventArgs)
            e.Session = New UnitOfWork()
        End Sub
    End Class
    Public Class Items
        Inherits XPObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Sub New()
        End Sub
        Private fId As Integer
        Public Property Id() As Integer
            Get
                Return fId
            End Get
            Set(ByVal value As Integer)
                SetPropertyValue("Id", fId, value)
            End Set
        End Property
        Private fName As String
        Public Property Name() As String
            Get
                Return fName
            End Get
            Set(ByVal value As String)
                SetPropertyValue("Name", fName, value)
            End Set
        End Property
    End Class
    Public NotInheritable Class XPOServiceHelper

        Private Sub New()
        End Sub

        Public Shared Sub SetupXpoLayer(ByVal session As Session)
            Dim address As New EndpointAddress("http://localhost:54177/XPOService.svc")
            Dim binding As New BasicHttpBinding()
            binding.MaxReceivedMessageSize = Int32.MaxValue
            Dim dataStore As New DataStoreClient(binding, address)
            XpoDefault.DataLayer = New SimpleDataLayer(dataStore)
            XpoDefault.Session = session
        End Sub
    End Class
    Public Class ServiceHelper
        Implements IServiceHelper

        Private Entities As DatabaseEntities
        Private Behavior As XPOInstantModeCRUDBehavior
        Private UIDispatcher As Dispatcher
        Public Sub New(ByVal behavior As XPOInstantModeCRUDBehavior, ByVal uri As Uri, ByVal uiDispatcher As Dispatcher)
            Me.Behavior = behavior
            Entities = New DatabaseEntities(uri)
            Me.UIDispatcher = uiDispatcher
        End Sub
        Public Sub AddNewItems(ByVal newItem As Object) Implements IServiceHelper.AddNewItems
            Dim items As Items = TryCast(newItem, Items)
            If items Is Nothing Then
                Return
            End If
            UIDispatcher.BeginInvoke(Sub() AddNewItemsCore(items))
        End Sub
        Public Sub RemoveItem(ByVal id As String) Implements IServiceHelper.RemoveItem
            Dim query As DataServiceQuery(Of Item) = GetQuery(id)
            query.BeginExecute(AddressOf AsyncRemoveCallback, query)
        End Sub
        Public Sub EditItem(ByVal id As String, ByVal itemValue As Object) Implements IServiceHelper.EditItem
            Dim query As DataServiceQuery(Of Item) = GetQuery(id)
            Dim tuple = New Tuple(Of DataServiceQuery(Of Item), Object)(query, itemValue)
            query.BeginExecute(AddressOf AsyncEditCallback, tuple)
        End Sub
        Public Sub GetItem(ByVal id As String) Implements IServiceHelper.GetItem
            Dim query As DataServiceQuery(Of Item) = GetQuery(id)
            query.BeginExecute(AddressOf AsyncGetItemCallback, query)
        End Sub
        Public Function GetQuery(ByVal id As String) As DataServiceQuery(Of Item)
            Dim index As Integer = Nothing
            Int32.TryParse(id, index)
            Dim query As DataServiceQuery(Of Item) = CType(Entities.Items.Where(Function(item) item.Id = index), DataServiceQuery(Of Item))
            Return query
        End Function

        Private Sub AddNewItemsCore(ByVal items As Items)
            Entities.AddToItems(New Item() With {.Id = items.Id, .Name = items.Name})
            Entities.BeginSaveChanges(AddressOf AsyncSaveCallback, Nothing)
        End Sub
        Private Sub SaveCallbackCore(ByVal asyncResult As IAsyncResult)
            Entities.EndSaveChanges(asyncResult)
            Behavior.CollectionSource.Refresh()
        End Sub

        Private Sub RemoveCallbackCore(ByVal item_Renamed As Item)
            Entities.DeleteObject(item_Renamed)
            Entities.BeginSaveChanges(AddressOf AsyncSaveCallback, Nothing)
        End Sub
        Private Sub EditCallbackCore(ByVal source As Item, ByVal destination As Item)
            destination.Id = source.Id
            destination.Name = source.Name
            Entities.UpdateObject(destination)
            Entities.BeginSaveChanges(AddressOf AsyncSaveCallback, Nothing)
        End Sub

        Private Sub GetItemCallbackCore(ByVal item_Renamed As Object)
            Behavior.EditRowItem = item_Renamed
            Behavior.CreateEditDialog()
        End Sub

        Protected Overridable Sub AsyncSaveCallback(ByVal asyncResult As IAsyncResult)
            UIDispatcher.BeginInvoke(Sub() SaveCallbackCore(asyncResult))
        End Sub
        Protected Overridable Sub AsyncRemoveCallback(ByVal asyncResult As IAsyncResult)
            Dim query As DataServiceQuery(Of Item) = TryCast(asyncResult.AsyncState, DataServiceQuery(Of Item))
            If query Is Nothing Then
                Return
            End If
            Dim items As Item = query.EndExecute(asyncResult).First()
            UIDispatcher.BeginInvoke(Sub() RemoveCallbackCore(items))
        End Sub
        Protected Overridable Sub AsyncEditCallback(ByVal asyncResult As IAsyncResult)
            Dim tuple As Tuple(Of DataServiceQuery(Of Item), Object) = TryCast(asyncResult.AsyncState, Tuple(Of DataServiceQuery(Of Item), Object))
            If tuple Is Nothing Then
                Return
            End If
            Dim items As IEnumerable(Of Item) = tuple.Item1.EndExecute(asyncResult)
            Dim en = items.GetEnumerator()
            en.MoveNext()
            UIDispatcher.BeginInvoke(Sub() EditCallbackCore(CType(tuple.Item2, Item), DirectCast(en.Current, Item)))
        End Sub
        Protected Overridable Sub AsyncGetItemCallback(ByVal asyncResult As IAsyncResult)
            Dim query As DataServiceQuery(Of Item) = TryCast(asyncResult.AsyncState, DataServiceQuery(Of Item))
            If query Is Nothing Then
                Return
            End If
            Dim items As IEnumerable(Of Item) = query.EndExecute(asyncResult)
            Dim en = items.GetEnumerator()
            en.MoveNext()
            UIDispatcher.BeginInvoke(Sub() GetItemCallbackCore(en.Current))
        End Sub
    End Class
End Namespace