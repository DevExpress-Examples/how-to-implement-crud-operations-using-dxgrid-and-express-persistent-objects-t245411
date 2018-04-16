using System;
using System.Configuration;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace XPOService {
    public class WcfXpoSCService: DataStoreService {
        private static readonly IDataStore Provider = XpoDefault.GetConnectionProvider(ConfigurationManager.ConnectionStrings["XpoConnection"].ConnectionString, AutoCreateOption.DatabaseAndSchema);
        static WcfXpoSCService() { }
        public WcfXpoSCService() : base(WcfXpoSCService.Provider) { }
    }
}