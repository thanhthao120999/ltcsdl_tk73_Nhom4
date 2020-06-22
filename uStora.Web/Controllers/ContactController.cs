using AutoMapper;
using System.Web.Mvc;
using uStora.Common;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.Extensions;
using uStora.Web.Models;

namespace uStora.Web.Controllers
{
    public class ContactController : Controller
    {
        private IContactDetailService _contactDetailService;

        public ContactController(IContactDetailService contactDetailService)
        {
            _contactDetailService = contactDetailService;
        }

        [OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            FeedbackViewModel viewModel = new FeedbackViewModel();
            viewModel.ContactDetail = GetContactDetail();
            return View(viewModel);
        }


        private ContactDetailViewModel GetContactDetail()
        {
            var contact = _contactDetailService.GetDefaultContact();
            var contactVm = Mapper.Map<ContactDetail, ContactDetailViewModel>(contact);
            return contactVm;
        }
    }
}