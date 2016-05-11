using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using CRM.Infrastructure;
using CRMViettour.Controllers;
using CRM.Core;

namespace CRMViettour.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<IGenericRepository<tbl_DictionaryCategory>, GenericRepository<tbl_DictionaryCategory>>();
            container.RegisterType<IGenericRepository<tbl_Dictionary>, GenericRepository<tbl_Dictionary>>();
            container.RegisterType<IGenericRepository<tbl_Tags>, GenericRepository<tbl_Tags>>();
            container.RegisterType<IGenericRepository<tbl_Company>, GenericRepository<tbl_Company>>();
            container.RegisterType<IGenericRepository<tbl_DocumentFile>, GenericRepository<tbl_DocumentFile>>();
            container.RegisterType<IGenericRepository<tbl_Customer>, GenericRepository<tbl_Customer>>();
            container.RegisterType<IGenericRepository<tbl_CustomerVisa>, GenericRepository<tbl_CustomerVisa>>();
            container.RegisterType<IGenericRepository<tbl_Form>, GenericRepository<tbl_Form>>();
            container.RegisterType<IGenericRepository<tbl_Function>, GenericRepository<tbl_Function>>();
            container.RegisterType<IGenericRepository<tbl_Headquater>, GenericRepository<tbl_Headquater>>();
            container.RegisterType<IGenericRepository<tbl_Module>, GenericRepository<tbl_Module>>();
            container.RegisterType<IGenericRepository<tbl_Partner>, GenericRepository<tbl_Partner>>();
            container.RegisterType<IGenericRepository<tbl_PartnerNote>, GenericRepository<tbl_PartnerNote>>();
            container.RegisterType<IGenericRepository<tbl_RoleGroup>, GenericRepository<tbl_RoleGroup>>();
            container.RegisterType<IGenericRepository<tbl_ServicesPartner>, GenericRepository<tbl_ServicesPartner>>();
            container.RegisterType<IGenericRepository<tbl_Staff>, GenericRepository<tbl_Staff>>();
            container.RegisterType<IGenericRepository<tbl_StaffGroup>, GenericRepository<tbl_StaffGroup>>();
            container.RegisterType<IGenericRepository<tbl_StaffVisa>, GenericRepository<tbl_StaffVisa>>();
            container.RegisterType<IGenericRepository<tbl_VisaInfomation>, GenericRepository<tbl_VisaInfomation>>();
            container.RegisterType<IGenericRepository<tbl_CustomerContact>, GenericRepository<tbl_CustomerContact>>();
            container.RegisterType<IGenericRepository<tbl_CustomerContactVisa>, GenericRepository<tbl_CustomerContactVisa>>();
            container.RegisterType<IGenericRepository<tbl_Quotation>, GenericRepository<tbl_Quotation>>();
            container.RegisterType<IGenericRepository<tbl_QuotationForm>, GenericRepository<tbl_QuotationForm>>();
            container.RegisterType<IGenericRepository<tbl_ReviewTour>, GenericRepository<tbl_ReviewTour>>();
            container.RegisterType<IGenericRepository<tbl_ReviewTourDetail>, GenericRepository<tbl_ReviewTourDetail>>();
            container.RegisterType<IGenericRepository<tbl_Tour>, GenericRepository<tbl_Tour>>();
            container.RegisterType<IGenericRepository<tbl_Program>, GenericRepository<tbl_Program>>();
            container.RegisterType<IGenericRepository<tbl_Contract>, GenericRepository<tbl_Contract>>();
            container.RegisterType<IGenericRepository<tbl_Promotion>, GenericRepository<tbl_Promotion>>();
            container.RegisterType<IGenericRepository<tbl_UpdateHistory>, GenericRepository<tbl_UpdateHistory>>();
            container.RegisterType<IGenericRepository<tbl_ContactHistory>, GenericRepository<tbl_ContactHistory>>();
            container.RegisterType<IGenericRepository<tbl_AppointmentHistory>, GenericRepository<tbl_AppointmentHistory>>();
            container.RegisterType<IGenericRepository<tbl_Task>, GenericRepository<tbl_Task>>();
            container.RegisterType<IGenericRepository<tbl_TaskStaff>, GenericRepository<tbl_TaskStaff>>();
            container.RegisterType<IGenericRepository<tbl_TaskNote>, GenericRepository<tbl_TaskNote>>();
            container.RegisterType<IGenericRepository<tbl_TaskHandling>, GenericRepository<tbl_TaskHandling>>();
            container.RegisterType<IGenericRepository<tbl_TourGuide>, GenericRepository<tbl_TourGuide>>();
            container.RegisterType<IGenericRepository<tbl_LiabilityCustomer>, GenericRepository<tbl_LiabilityCustomer>>();
            container.RegisterType<IGenericRepository<tbl_LiabilityPartner>, GenericRepository<tbl_LiabilityPartner>>();
            container.RegisterType<IGenericRepository<tbl_TourCustomer>, GenericRepository<tbl_TourCustomer>>();
            container.RegisterType<IGenericRepository<tbl_TourCustomerVisa>, GenericRepository<tbl_TourCustomerVisa>>();
            container.RegisterType<IGenericRepository<tbl_TourOption>, GenericRepository<tbl_TourOption>>();
            container.RegisterType<IGenericRepository<tbl_TourSchedule>, GenericRepository<tbl_TourSchedule>>();
            container.RegisterType<IGenericRepository<tbl_DeadlineOption>, GenericRepository<tbl_DeadlineOption>>();

            container.RegisterType<IBaseRepository, BaseRepository>();
            container.RegisterType<IHomeRepository, HomeRepository>();
            container.RegisterType<IConfigRepository, ConfigRepository>();
            container.RegisterType<ManageController>(new InjectionConstructor());
            container.RegisterType<AccountController>(new InjectionConstructor(typeof(AccountRepository)));
        }
    }
}
