using BlueLinker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLinker
{
    public class ServiceRegistration
    {
        public static void RegisterCommonServices(IServiceCollection services)
        {
            // Общие сервисы, доступные для обеих платформ
            services.AddSingleton<AndroidViewModel>();
            services.AddSingleton<WindowsView>();
        }
    }
}
