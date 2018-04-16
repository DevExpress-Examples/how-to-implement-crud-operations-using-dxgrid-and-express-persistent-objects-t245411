using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpf.Core.WPFCompatibility;
using System.ServiceModel;
using XPOInstant.SL.ServiceReference1;
using DevExpress.Xpf.Core.ServerMode;
using System.Data.Services.Client;
using System.Threading;
using System.Windows.Threading;

namespace XPOInstant.SL {
    public partial class MainPage: UserControl {
        public static readonly DependencyProperty CollectionProperty =
            DependencyPropertyManager.Register("Collection", typeof(XPInstantFeedbackSource), typeof(MainPage), new SLPropertyMetadata(null));
        public XPInstantFeedbackSource Collection {
            get { return (XPInstantFeedbackSource)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }
        public MainPage() {
            InitializeComponent();
            XPOServiceHelper.SetupXpoLayer(new UnitOfWork());
            Collection = new XPInstantFeedbackSource(typeof(Items));
            Collection.ResolveSession += OnCollectionResolveSession;
            Collection.DismissSession += OnCollectionDismissSession;

            helper.XPObjectType = typeof(Items);
            helper.CollectionSource = Collection;
            helper.ServiceHelper = new ServiceHelper(helper, new Uri("http://localhost:54177/WcfDataService.svc/"), Dispatcher);
        }
        void OnCollectionDismissSession(object sender, ResolveSessionEventArgs e) {
            IDisposable session = e.Session as IDisposable;
            if(session != null)
                session.Dispose();
        }
        void OnCollectionResolveSession(object sender, ResolveSessionEventArgs e) {
            e.Session = new UnitOfWork();
        }
    }
    public class Items: XPObject {
        public Items(Session session) : base(session) { }
        public Items() { }
        int fId;
        public int Id {
            get { return fId; }
            set { SetPropertyValue("Id", ref fId, value); }
        }
        string fName;
        public string Name {
            get { return fName; }
            set { SetPropertyValue("Name", ref fName, value); }
        }
    }
    public static class XPOServiceHelper {
        public static void SetupXpoLayer(Session session) {
            EndpointAddress address = new EndpointAddress("http://localhost:54177/XPOService.svc");
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            DataStoreClient dataStore = new DataStoreClient(binding, address);
            XpoDefault.DataLayer = new SimpleDataLayer(dataStore);
            XpoDefault.Session = session;
        }
    }
    public class ServiceHelper: IServiceHelper {
        DatabaseEntities Entities;
        XPOInstantModeCRUDBehavior Behavior;
        Dispatcher UIDispatcher;
        public ServiceHelper(XPOInstantModeCRUDBehavior behavior, Uri uri, Dispatcher uiDispatcher) {
            Behavior = behavior;
            Entities = new DatabaseEntities(uri);
            UIDispatcher = uiDispatcher;
        }
        public void AddNewItems(object newItem) {
            Items items = newItem as Items;
            if(items == null)
                return;
            UIDispatcher.BeginInvoke(() => AddNewItemsCore(items));
        }
        public void RemoveItem(string id) {
            DataServiceQuery<Item> query = GetQuery(id);
            query.BeginExecute(AsyncRemoveCallback, query);
        }
        public void EditItem(string id, object itemValue) {
            DataServiceQuery<Item> query = GetQuery(id);
            var tuple = new Tuple<DataServiceQuery<Item>, object>(query, itemValue);
            query.BeginExecute(AsyncEditCallback, tuple);
        }
        public void GetItem(string id) {
            DataServiceQuery<Item> query = GetQuery(id);
            query.BeginExecute(AsyncGetItemCallback, query);
        }
        public DataServiceQuery<Item> GetQuery(string id) {
            int index;
            Int32.TryParse(id, out index);
            DataServiceQuery<Item> query = (DataServiceQuery<Item>)Entities.Items.Where<Item>(item => item.Id == index);
            return query;
        }

        void AddNewItemsCore(Items items) {
            Entities.AddToItems(new Item() { Id = items.Id, Name = items.Name });
            Entities.BeginSaveChanges(AsyncSaveCallback, null);
        }
        void SaveCallbackCore(IAsyncResult asyncResult) {
            Entities.EndSaveChanges(asyncResult);
            Behavior.CollectionSource.Refresh();
        }
        void RemoveCallbackCore(Item item) {
            Entities.DeleteObject(item);
            Entities.BeginSaveChanges(AsyncSaveCallback, null);
        }
        void EditCallbackCore(Item source, Item destination) {
            destination.Id = source.Id;
            destination.Name = source.Name;
            Entities.UpdateObject(destination);
            Entities.BeginSaveChanges(AsyncSaveCallback, null);
        }
        void GetItemCallbackCore(object item) {
            Behavior.EditRowItem = item;
            Behavior.CreateEditDialog();
        }

        protected virtual void AsyncSaveCallback(IAsyncResult asyncResult) {
            UIDispatcher.BeginInvoke(() => SaveCallbackCore(asyncResult));
        }
        protected virtual void AsyncRemoveCallback(IAsyncResult asyncResult) {
            DataServiceQuery<Item> query = asyncResult.AsyncState as DataServiceQuery<Item>;
            if(query == null)
                return;
            Item items = query.EndExecute(asyncResult).First();
            UIDispatcher.BeginInvoke(() => RemoveCallbackCore(items));
        }
        protected virtual void AsyncEditCallback(IAsyncResult asyncResult) {
            Tuple<DataServiceQuery<Item>, object> tuple = asyncResult.AsyncState as Tuple<DataServiceQuery<Item>, object>;
            if(tuple == null)
                return;
            IEnumerable<Item> items = tuple.Item1.EndExecute(asyncResult);
            var en = items.GetEnumerator();
            en.MoveNext();
            UIDispatcher.BeginInvoke(() => EditCallbackCore((Item)tuple.Item2, (Item)en.Current));
        }
        protected virtual void AsyncGetItemCallback(IAsyncResult asyncResult) {
            DataServiceQuery<Item> query = asyncResult.AsyncState as DataServiceQuery<Item>;
            if(query == null)
                return;
            IEnumerable<Item> items = query.EndExecute(asyncResult);
            var en = items.GetEnumerator();
            en.MoveNext();
            UIDispatcher.BeginInvoke(() => GetItemCallbackCore(en.Current));
        }
    }
}