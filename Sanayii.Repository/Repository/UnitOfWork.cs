using Sanayii.Core;
using Sanayii.Core.Entities;
using Sanayii.Repository.Data;
using Sanayii.Core.Entities;
using Sanayii.Repository;
using Sanayii.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanayii.Core.Repository;

namespace Sanayii.Repository.Repository
{
    public class UnitOFWork
    {
        SanayiiContext db;

        CategoryRepository CategoryRepo;
        GenericRepository<Service> ServiceRepo;

        GenericRepository<Payment> PaymentMethodsRepo;
        GenericRepository<Payment> PaymentRepo;

        AdminRepository AdminRepo;
        ServiceRequestPaymentRepository serviceRequestPaymentRepo;
        CustomerRepository CustomerRepo;
        ArtisanRepository ArtisanRepo;

        GenericRepository<Contract> ContractRepo;
        GenericRepository<Violation> ViolationRepo;


        GenericRepository<Discount> DiscountRepo;
        GenericRepository<CustomerDiscount> CustomerDiscountRepo;

        GenericRepository<Review> reviewRepo;
        GenericRepository<Notification> notificationRepo;


        public UnitOFWork(SanayiiContext db)
        {
            this.db = db;
        }

       

        public CategoryRepository _CategoryRepo
        {
            get
            {
                if (CategoryRepo == null)
                {
                    CategoryRepo = new CategoryRepository(db);
                }
                return CategoryRepo;
            }
        }
        
        public GenericRepository<Service> _ServiceRepo
        {
            get
            {
                if (ServiceRepo == null)
                {
                    ServiceRepo = new GenericRepository<Service>(db);
                }
                return ServiceRepo;
            }
        }

        public GenericRepository<Payment> _paymentMethodsRepo
        {
            get
            {
                if (PaymentMethodsRepo == null)
                {
                    PaymentMethodsRepo = new GenericRepository<Payment>(db);
                }
                return PaymentMethodsRepo;
            }
        }

        public GenericRepository<Payment> _PaymentRepo
        {
            get
            {
                if (PaymentRepo == null)
                {
                    PaymentRepo = new GenericRepository<Payment>(db);
                }
                return PaymentRepo;
            }
        }

        public AdminRepository _AdminRepo
        {
            get
            {
                if (AdminRepo == null)
                {
                    AdminRepo = new AdminRepository(db);
                }
                return AdminRepo;
            }
        }
        public ServiceRequestPaymentRepository _ServiceRequestPaymentRepo
        {
            get
            {
                if (serviceRequestPaymentRepo == null)
                {
                    serviceRequestPaymentRepo = new ServiceRequestPaymentRepository(db);
                }
                return serviceRequestPaymentRepo;
            }
        }

        public CustomerRepository _CustomerRepo
        {
            get
            {
                if (CustomerRepo == null)
                {
                    CustomerRepo = new CustomerRepository(db);
                }
                return CustomerRepo;
            }
        }


        public ArtisanRepository _ArtisanRepo
        {
            get
            {
                if (ArtisanRepo == null)
                {
                    ArtisanRepo = new ArtisanRepository(db);
                }
                return ArtisanRepo;
            }
        }


        public GenericRepository<Contract> _ContractRepo
        {
            get
            {
                if (ContractRepo == null)
                {
                    ContractRepo = new GenericRepository<Contract>(db);
                }
                return ContractRepo;
            }
        }

        public GenericRepository<Violation> _ViolationRepo
        {
            get
            {
                if (ViolationRepo == null)
                {
                    ViolationRepo = new GenericRepository<Violation>(db);
                }
                return ViolationRepo;
            }
        }

        public GenericRepository<Discount> _DiscountRepo
        {
            get
            {
                if (DiscountRepo == null)
                {
                    DiscountRepo = new GenericRepository<Discount>(db);
                }
                return DiscountRepo;
            }
        }

        public GenericRepository<CustomerDiscount> _CustomerDiscountRepo
        {
            get
            {
                if (CustomerDiscountRepo == null)
                {
                    CustomerDiscountRepo = new GenericRepository<CustomerDiscount>(db);
                }
                return CustomerDiscountRepo;
            }
        }


        public GenericRepository<Review> _ReviewRepo
        {
            get
            {
                if (reviewRepo == null)
                {
                    reviewRepo = new GenericRepository<Review>(db);
                }
                return reviewRepo;
            }
        }
        public GenericRepository<Notification> _NotificationRepo
        {
            get
            {
                if (notificationRepo == null)
                {
                    notificationRepo = new GenericRepository<Notification>(db);
                }
                return notificationRepo;
            }
        }
        public void save()
        {
            db.SaveChanges();
        }
    }
}
