Imports System
Imports System.Configuration
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB

Namespace XPOService
    Public Class WcfXpoSCService
        Inherits DataStoreService

        Private Shared ReadOnly ConnectionProvider As IDataStore = XpoDefault.GetConnectionProvider(ConfigurationManager.ConnectionStrings("XpoConnection").ConnectionString, AutoCreateOption.DatabaseAndSchema)
        Shared Sub New()
        End Sub
        Public Sub New()
            MyBase.New(WcfXpoSCService.ConnectionProvider)
        End Sub
    End Class
End Namespace