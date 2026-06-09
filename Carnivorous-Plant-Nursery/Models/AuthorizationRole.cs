namespace Carnivorous_Plant_Nursery.Models
{
    public static class AuthorizationRole
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Customer = "Customer";
        public const string AdminOrManager = Admin + "," + Manager;
    }
}
