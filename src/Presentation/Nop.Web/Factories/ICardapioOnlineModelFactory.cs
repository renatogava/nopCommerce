using System.Collections.Generic;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public interface ICardapioOnlineModelFactory
    {
        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        List<CategoryModel> PrepareHomepageCategoryModels();
    }
}
