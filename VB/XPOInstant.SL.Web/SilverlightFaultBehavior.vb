' This is an auto-generated file to enable WCF faults to reach Silverlight clients.

Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Runtime.Serialization
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Namespace XPOInstant.SL.Web
    Public Class SilverlightFaultBehavior
        Inherits Attribute
        Implements IServiceBehavior

        Private Class SilverlightFaultEndpointBehavior
            Implements IEndpointBehavior

            Public Sub AddBindingParameters(ByVal endpoint As ServiceEndpoint, ByVal bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters
            End Sub

            Public Sub ApplyClientBehavior(ByVal endpoint As ServiceEndpoint, ByVal clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior
            End Sub

            Public Sub ApplyDispatchBehavior(ByVal endpoint As ServiceEndpoint, ByVal endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(New SilverlightFaultMessageInspector())
            End Sub

            Public Sub Validate(ByVal endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate
            End Sub

            Private Class SilverlightFaultMessageInspector
                Implements IDispatchMessageInspector

                Public Function AfterReceiveRequest(ByRef request As Message, ByVal channel As IClientChannel, ByVal instanceContext As InstanceContext) As Object Implements IDispatchMessageInspector.AfterReceiveRequest
                    Return Nothing
                End Function

                Public Sub BeforeSendReply(ByRef reply As Message, ByVal correlationState As Object) Implements IDispatchMessageInspector.BeforeSendReply
                    If (reply IsNot Nothing) AndAlso reply.IsFault Then
                        Dim [property] As New HttpResponseMessageProperty()
                        [property].StatusCode = HttpStatusCode.OK
                        reply.Properties(HttpResponseMessageProperty.Name) = [property]
                    End If
                End Sub
            End Class
        End Class

        Public Sub AddBindingParameters(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase, ByVal endpoints As Collection(Of ServiceEndpoint), ByVal bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        End Sub

        Public Sub ApplyDispatchBehavior(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
            For Each endpoint As ServiceEndpoint In serviceDescription.Endpoints
                endpoint.Behaviors.Add(New SilverlightFaultEndpointBehavior())
            Next endpoint
        End Sub

        Public Sub Validate(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        End Sub
    End Class
End Namespace
