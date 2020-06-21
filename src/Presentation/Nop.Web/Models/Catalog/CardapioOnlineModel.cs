using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class CardapioOnlineModel : BaseNopEntityModel
    {
        public CardapioOnlineModel()
        {
            Categories = new List<CategoryModel>();
        }

        public IList<CategoryModel> Categories { get; set; }
    }
}