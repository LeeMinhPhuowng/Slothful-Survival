using Reflex.Core;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Reflex DI installer for the Inventory feature.
    /// Chỉ đăng ký shared dependencies. 
    /// InventoryModel được tạo bởi mỗi InventoryView (1 view = 1 model).
    /// </summary>
    public class InventoryInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            // IEventBus đã được đăng ký bởi CoreInstaller.
            // InventoryModel được tạo trực tiếp bởi InventoryView.Start().
            // Installer này dành cho các dependency dùng chung trong tương lai.
        }
    }
}
