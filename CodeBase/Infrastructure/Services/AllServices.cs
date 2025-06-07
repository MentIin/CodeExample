namespace Assets.CodeBase.Infrastructure.Services
{
    public class AllServices
    {
        private static AllServices _instance;
        public static AllServices Container => _instance ?? (_instance = new AllServices());

        public void RegisterSingle<TService>(TService implementation) where TService : IService
        {
            Implementation<TService>.serviceInstance = implementation;
        }

        public TService Single<TService>() where TService : IService
        {
            return Implementation<TService>.serviceInstance;
        }

        private static class Implementation<TService> where TService : IService
        {
            public static TService serviceInstance;
        }
    }
}