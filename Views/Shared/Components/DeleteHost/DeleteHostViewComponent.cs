using FiDa.DatabaseModels;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FiDa.Views.Shared.Components.StorageUsage
{
	[ViewComponent(Name = "DeleteHost")]
	public class DeleteHostViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(UserHost host) => await Task.FromResult<IViewComponentResult>(View("DeleteHost", new DeleteHostModel { Host = host }));
	}
}
