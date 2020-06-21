using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    public class CardapioOnlineModelFactory : ICardapioOnlineModelFactory
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;

        #region Ctor

        public CardapioOnlineModelFactory(
            IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            ICustomerService customerService,
            IProductService productService,
            IProductModelFactory productModelFactory
            )
        {
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _storeContext = storeContext;
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _productService = productService;
            _productModelFactory = productModelFactory;
        }

        #endregion

        public List<CategoryModel> PrepareHomepageCategoryModels()
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            var categoriesCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryHomepageKey,
                pictureSize,
                _workContext.WorkingLanguage,
                _webHelper.IsCurrentConnectionSecured());

            var model = _staticCacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomepage()
                    .Select(category =>
                    {
                        var catModel = new CategoryModel
                        {
                            Id = category.Id,
                            Name = _localizationService.GetLocalized(category, x => x.Name),
                            Description = _localizationService.GetLocalized(category, x => x.Description),
                            MetaKeywords = _localizationService.GetLocalized(category, x => x.MetaKeywords),
                            MetaDescription = _localizationService.GetLocalized(category, x => x.MetaDescription),
                            MetaTitle = _localizationService.GetLocalized(category, x => x.MetaTitle),
                            SeName = _urlRecordService.GetSeName(category),
                        };

                        //prepare picture model
                        var categoryPictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey,
                            category, pictureSize, true, _workContext.WorkingLanguage,
                            _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);
                        catModel.PictureModel = _staticCacheManager.Get(categoryPictureCacheKey, () =>
                        {
                            var picture = _pictureService.GetPictureById(category.PictureId);
                            var pictureModel = new PictureModel
                            {
                                FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                                ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                                Title = string.Format(
                                    _localizationService.GetResource("Media.Category.ImageLinkTitleFormat"),
                                    catModel.Name),
                                AlternateText =
                                    string.Format(
                                        _localizationService.GetResource("Media.Category.ImageAlternateTextFormat"),
                                        catModel.Name)
                            };
                            return pictureModel;
                        });

                        //featured products
                        if (!_catalogSettings.IgnoreFeaturedProducts)
                        {
                            //We cache a value indicating whether we have featured products
                            IPagedList<Product> featuredProducts = null;
                            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryHasFeaturedProductsKey, category,
                                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer), _storeContext.CurrentStore);
                            var hasFeaturedProductsCache = _staticCacheManager.Get(cacheKey, () =>
                            {
                                //no value in the cache yet
                                //let's load products and cache the result (true/false)
                                featuredProducts = _productService.SearchProducts(
                                   categoryIds: new List<int> { category.Id },
                                   storeId: _storeContext.CurrentStore.Id,
                                   visibleIndividuallyOnly: true,
                                   featuredProducts: true);

                                return featuredProducts.TotalCount > 0;
                            });

                            if (hasFeaturedProductsCache && featuredProducts == null)
                            {
                                //cache indicates that the category has featured products
                                //let's load them
                                featuredProducts = _productService.SearchProducts(
                                   categoryIds: new List<int> { category.Id },
                                   storeId: _storeContext.CurrentStore.Id,
                                   visibleIndividuallyOnly: true,
                                   featuredProducts: true);
                            }

                            if (featuredProducts != null)
                            {
                                catModel.FeaturedProducts = _productModelFactory.PrepareProductOverviewModels(featuredProducts).ToList();
                            }
                        }

                        var categoryIds = new List<int> { category.Id };

                        //include subcategories
                        if (_catalogSettings.ShowProductsFromSubcategories)
                            categoryIds.AddRange(catModel.SubCategories.Select(sc => sc.Id));

                        //products
                        var products = _productService.SearchProducts(out var filterableSpecificationAttributeOptionIds,
                            true,
                            categoryIds: categoryIds,
                            storeId: _storeContext.CurrentStore.Id,
                            visibleIndividuallyOnly: true,
                            featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false);
                        catModel.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();

                        return catModel;
                    }).ToList());

            return model;
        }
    }
}
