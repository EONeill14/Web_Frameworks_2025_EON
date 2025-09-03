using Microsoft.AspNetCore.Mvc;
using Web_Frameworks_2025_EON.Repositories;

namespace Web_Frameworks_2025_EON.ViewComponents
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly IItemRepository _itemRepository;

        public CategoryNavigationViewComponent(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            var categories = await _itemRepository.GetAllCategoriesAsync();
            return View(categories);
        }
    }
}