// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/Product.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Soa.Protos {
  public static partial class ProductService
  {
    static readonly string __ServiceName = "Soa.Protos.ProductService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Soa.Protos.CategoryRequest> __Marshaller_Soa_Protos_CategoryRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Soa.Protos.CategoryRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Soa.Protos.ProductsResponse> __Marshaller_Soa_Protos_ProductsResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Soa.Protos.ProductsResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Soa.Protos.ProductRequest> __Marshaller_Soa_Protos_ProductRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Soa.Protos.ProductRequest.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Soa.Protos.CategoryRequest, global::Soa.Protos.ProductsResponse> __Method_GetProductsByCategory = new grpc::Method<global::Soa.Protos.CategoryRequest, global::Soa.Protos.ProductsResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetProductsByCategory",
        __Marshaller_Soa_Protos_CategoryRequest,
        __Marshaller_Soa_Protos_ProductsResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Soa.Protos.ProductRequest, global::Soa.Protos.ProductsResponse> __Method_CreateProduct = new grpc::Method<global::Soa.Protos.ProductRequest, global::Soa.Protos.ProductsResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CreateProduct",
        __Marshaller_Soa_Protos_ProductRequest,
        __Marshaller_Soa_Protos_ProductsResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Soa.Protos.ProductReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of ProductService</summary>
    [grpc::BindServiceMethod(typeof(ProductService), "BindService")]
    public abstract partial class ProductServiceBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Soa.Protos.ProductsResponse> GetProductsByCategory(global::Soa.Protos.CategoryRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Soa.Protos.ProductsResponse> CreateProduct(global::Soa.Protos.ProductRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(ProductServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_GetProductsByCategory, serviceImpl.GetProductsByCategory)
          .AddMethod(__Method_CreateProduct, serviceImpl.CreateProduct).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, ProductServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_GetProductsByCategory, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Soa.Protos.CategoryRequest, global::Soa.Protos.ProductsResponse>(serviceImpl.GetProductsByCategory));
      serviceBinder.AddMethod(__Method_CreateProduct, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Soa.Protos.ProductRequest, global::Soa.Protos.ProductsResponse>(serviceImpl.CreateProduct));
    }

  }
}
#endregion
