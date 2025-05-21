using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Soa.Protos;

public class Query
{
    public async Task<IEnumerable<Product>> GetRecommendationsAsync(
        [Service] UserService.UserServiceClient   userClient,
        [Service] ProductService.ProductServiceClient productClient,
        string userId)
    {
        try
        {
            var userResponse = await userClient.GetUserPreferencesAsync(
                new UserRequest { UserId = userId },
                deadline: DateTime.UtcNow.AddSeconds(5));

            var prefs = userResponse?.Preferences;
            if (prefs == null || prefs.Count == 0)
                return await GetDefaultProductsAsync(productClient);

            var category = prefs.FirstOrDefault() ?? "default";
            return await GetProductsByCategoryAsync(productClient, category);
        }
        catch (RpcException ex) when (
                 ex.StatusCode == StatusCode.NotFound     ||
                 ex.StatusCode == StatusCode.DeadlineExceeded ||
                 ex.StatusCode == StatusCode.Unimplemented)
        {
            return await GetDefaultProductsAsync(productClient);
        }
    }

    private static Task<IEnumerable<Product>> GetDefaultProductsAsync(
        ProductService.ProductServiceClient productClient)
        => GetProductsByCategoryAsync(productClient, "default");

    private static async Task<IEnumerable<Product>> GetProductsByCategoryAsync(
        ProductService.ProductServiceClient productClient,
        string category)
    {
        
        var response = await productClient.GetProductsByCategoryAsync(
            new CategoryRequest { Category = category },
            deadline: DateTime.UtcNow.AddSeconds(5));

        
        return (response?.Products ?? Enumerable.Empty<Product>())
               .Take(5);
    }
}
