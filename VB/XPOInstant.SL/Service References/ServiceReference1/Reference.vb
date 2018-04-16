﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.261
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

' Original file name:
' Generation date: 24/2/2012 5:05:51 PM
Namespace XPOInstant.SL.ServiceReference1

    ''' <summary>
    ''' There are no comments for DatabaseEntities in the schema.
    ''' </summary>
    Partial Public Class DatabaseEntities
        Inherits System.Data.Services.Client.DataServiceContext

        ''' <summary>
        ''' Initialize a new DatabaseEntities object.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            MyBase.New(serviceRoot)
            Me.ResolveName = New Global.System.Func(Of Global.System.Type, String)(AddressOf Me.ResolveNameFromType)
            Me.ResolveType = New Global.System.Func(Of String, Global.System.Type)(AddressOf Me.ResolveTypeFromName)
            Me.OnContextCreated()
        End Sub
        Partial Private Sub OnContextCreated()
        End Sub
        ''' <summary>
        ''' Since the namespace configured for this service reference
        ''' in Visual Studio is different from the one indicated in the
        ''' server schema, use type-mappers to map between the two.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Protected Function ResolveTypeFromName(ByVal typeName As String) As Global.System.Type
            If typeName.StartsWith("DatabaseModel", Global.System.StringComparison.Ordinal) Then
                Return Me.GetType().Assembly.GetType(String.Concat("XPOInstant.SL.ServiceReference1", typeName.Substring(13)), False)
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' Since the namespace configured for this service reference
        ''' in Visual Studio is different from the one indicated in the
        ''' server schema, use type-mappers to map between the two.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Protected Function ResolveNameFromType(ByVal clientType As Global.System.Type) As String
            If clientType.Namespace.Equals("XPOInstant.SL.ServiceReference1", Global.System.StringComparison.Ordinal) Then
                Return String.Concat("DatabaseModel.", clientType.Name)
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' There are no comments for Items in the schema.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public ReadOnly Property Items() As Global.System.Data.Services.Client.DataServiceQuery(Of Item)
            Get
                If (Me._Items Is Nothing) Then
                    Me._Items = MyBase.CreateQuery(Of Item)("Items")
                End If
                Return Me._Items
            End Get
        End Property
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Private _Items As Global.System.Data.Services.Client.DataServiceQuery(Of Item)
        ''' <summary>
        ''' There are no comments for Items in the schema.
        ''' </summary>

        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Sub AddToItems(ByVal item_Renamed As Item)
            MyBase.AddObject("Items", item_Renamed)
        End Sub
    End Class
    ''' <summary>
    ''' There are no comments for DatabaseModel.Item in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' Id
    ''' </KeyProperties>
    <Global.System.Data.Services.Common.EntitySetAttribute("Items"), Global.System.Data.Services.Common.DataServiceKeyAttribute("Id")> _
    Partial Public Class Item
        Implements System.ComponentModel.INotifyPropertyChanged

        ''' <summary>
        ''' Create a new Item object.
        ''' </summary>
        ''' <param name="ID">Initial value of Id.</param>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Shared Function CreateItem(ByVal ID As Integer) As Item

            Dim item_Renamed As New Item()
            item_Renamed.Id = ID
            Return item_Renamed
        End Function
        ''' <summary>
        ''' There are no comments for Property Id in the schema.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Property Id() As Integer
            Get
                Return Me._Id
            End Get
            Set(ByVal value As Integer)
                Me.OnIdChanging(value)
                Me._Id = value
                Me.OnIdChanged()
                Me.OnPropertyChanged("Id")
            End Set
        End Property
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Private _Id As Integer
        Partial Private Sub OnIdChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnIdChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property Name in the schema.
        ''' </summary>
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Property Name() As String
            Get
                Return Me._Name
            End Get
            Set(ByVal value As String)
                Me.OnNameChanging(value)
                Me._Name = value
                Me.OnNameChanged()
                Me.OnPropertyChanged("Name")
            End Set
        End Property
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Private _Name As String
        Partial Private Sub OnNameChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnNameChanged()
        End Sub
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")> _
        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
        End Sub
    End Class
End Namespace
