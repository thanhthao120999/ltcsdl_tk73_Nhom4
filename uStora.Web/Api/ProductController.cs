using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.Core;
using uStora.Web.Infrastructure.Extensions;
using uStora.Web.Models;

namespace uStora.Web.API
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiControllerBase
    {
        #region Initialize

        private IProductService _productService;
        private IBrandService _brandService;

        public ProductController(IErrorService errorService,
            IProductService productService, IBrandService brandService)
            : base(errorService)
        {
            _productService = productService;
            _brandService = brandService;
        }

        #endregion Initialize

        #region Methods

        [Route("getallparents")]
        [HttpGet]
        [Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _productService.GetAll();

                var responseData = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }

        [Route("listbrands")]
        [HttpGet]
        [Authorize(Roles = "ViewUser")]
        public HttpResponseMessage ListBrands(HttpRequestMessage request)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _brandService.GetAll("");

                var responseData = Mapper.Map<IEnumerable<Brand>, IEnumerable<BrandViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productService.FindById(id);

                var responseData = Mapper.Map<Product, ProductViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        [Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int page, int pageSize = 20, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _productService.GetAll(filter);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);

                var responseData = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(query);

                var paginationSet = new PaginationSet<ProductViewModel>()
                {
                    Items = responseData,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };
                var response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("create")]
        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "AddUser")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductViewModel productCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newProduct = new Product();
                    newProduct.UpdateProduct(productCategoryVm);
                    newProduct.CreatedDate = DateTime.Now;
                    newProduct.CreatedBy = User.Identity.Name;
                    _productService.Add(newProduct);
                    _productService.SaveChanges();

                    var responseData = Mapper.Map<Product, ProductViewModel>(newProduct);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        [Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductViewModel productVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbProduct = _productService.FindById(productVm.ID);

                    dbProduct.UpdateProduct(productVm);
                    dbProduct.UpdatedDate = DateTime.Now;
                    dbProduct.UpdatedBy = User.Identity.Name;
                    _productService.Update(dbProduct);
                    _productService.SaveChanges();

                    var responseData = Mapper.Map<Product, ProductViewModel>(dbProduct);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        [Authorize(Roles = "DeleteUser")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    _productService.IsDeleted(id);
                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [Authorize(Roles = "DeleteUser")]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedProducts)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(selectedProducts);
                    foreach (var item in listProductCategory)
                    {
                        _productService.Delete(item);
                    }

                    _productService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
                }

                return response;
            });
        }

        #endregion Methods

    }

}