using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageProductsCardapioOnlineViewComponent : NopViewComponent
    {
        private readonly ICardapioOnlineModelFactory _cardapioOnlineFactory;

        public HomepageProductsCardapioOnlineViewComponent(
            ICardapioOnlineModelFactory cardapioOnlineFactory)
        {
            _cardapioOnlineFactory = cardapioOnlineFactory;
        }

        public IViewComponentResult Invoke(int? productThumbPictureSize)
        {
            var model = _cardapioOnlineFactory.PrepareHomepageCategoryModels();
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}