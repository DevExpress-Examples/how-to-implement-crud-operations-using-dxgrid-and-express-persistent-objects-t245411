Imports System
Imports System.Collections.Generic
Imports System.Data.Services
Imports System.Data.Services.Common
Imports System.Linq
Imports System.ServiceModel.Web
Imports System.Web

Namespace XPOInstant.SL.Web
    Public Class WcfDataService
        Inherits DataService(Of DatabaseEntities)

        Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.All)
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead)
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2
        End Sub
    End Class
End Namespace