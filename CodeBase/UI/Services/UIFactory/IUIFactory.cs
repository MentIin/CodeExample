using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.Services;

namespace CodeBase.UI.Services.UIFactory
{
    public interface IUIFactory : IService
    {
        Task CreateWindow(WindowType type);
        Task CreateUIRoot();
        void Clear();
        bool CheckWindowExistence(WindowType pause);
    }
}