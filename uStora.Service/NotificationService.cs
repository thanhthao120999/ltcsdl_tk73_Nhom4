using System;
using System.Collections.Generic;
using System.Linq;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;

namespace uStora.Service
{
    public interface INotificationService
    {
        IEnumerable<ApplicationUser> GetAllUsers();

        IEnumerable<ApplicationUser> GetUnViewedUsers(DateTime date);
    }

    public class NotificationService : INotificationService
    {
        private IApplicationUserRepository _userAppRepository;
        private IUnitOfWork _unitOfWork;

        public NotificationService(IApplicationUserRepository userAppRepository, IUnitOfWork unitOfWork)
        {
            _userAppRepository = userAppRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ApplicationUser> GetUnViewedUsers(DateTime date)
        {
            return _userAppRepository.GetMulti(x => x.CreatedDate > date).OrderByDescending(y => y.CreatedDate);
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _userAppRepository.GetAll().OrderByDescending(x => x.CreatedDate);
        }
    }
}