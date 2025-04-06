namespace Shared.Communication
{
    // Enum defining all possible operations/message types
    public enum MessageType
    {
        // General
        ErrorResponse,
        SuccessResponse, // Generic success for simple operations like Add/Update/Delete

        // Hero Operations
        GetAllHeroesRequest,
        GetAllHeroesResponse,
        GetHeroByIdRequest,
        GetHeroByIdResponse,
        AddHeroRequest,
        // AddHeroResponse (covered by Success/Error)
        UpdateHeroRequest,
        // UpdateHeroResponse (covered by Success/Error)
        DeleteHeroRequest,
        // DeleteHeroResponse (covered by Success/Error)
        HeroUpdateNotification, // Server -> Client notification

        // Item Operations
        GetAllItemsRequest,
        GetAllItemsResponse,
        GetItemByIdRequest,
        GetItemByIdResponse,
        AddItemRequest,
        UpdateItemRequest,
        DeleteItemRequest,
        ItemUpdateNotification,

        // Inventory Operations (Might be less direct, often part of Hero)
        GetInventoryByIdRequest, // Potentially useful?
        GetInventoryByIdResponse,
        InventoryUpdateNotification, // Likely part of HeroUpdateNotification

        // Order Operations
        GetAllOrdersRequest,
        GetAllOrdersResponse,
        GetOrderByIdRequest,
        GetOrderByIdResponse,
        AddOrderRequest,
        UpdateOrderRequest,
        DeleteOrderRequest,
        OrderUpdateNotification,

        // Other potential operations
        ProcessOrdersRequest, // Client requests server to process orders
        DeductMaintenanceRequest, // Client requests maintenance deduction
    }
}