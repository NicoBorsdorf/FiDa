using FiDa.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FiDa.Views.Shared.Components.HostView
{
    [ViewComponent(Name = "HostView")]
    public class HostViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(BaseViewModel model) => await Task.FromResult<IViewComponentResult>(View("HostView", model));
    }
}
