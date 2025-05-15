using HotChocolate.Types;

public class ProductType : ObjectType<Soa.Protos.Product>
{
    protected override void Configure(IObjectTypeDescriptor<Soa.Protos.Product> descriptor)
    {
        descriptor.Field(p => p.Id).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Category).Type<NonNullType<StringType>>();
    }
}