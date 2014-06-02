
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Views;
using Android.Widget;
using baidumapsdk.demo;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Cloud;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Search;
using Com.Baidu.Platform.Comapi.Basestruct;
using Java.Lang;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Content.Res;
using Android.Support.V4.App;
using Android.App;

namespace baidumapsdk.demo
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_offline", ScreenOrientation = ScreenOrientation.Sensor)]
    public class OfflineDemo : Activity, IMKOfflineMapListener
    {
        private MapView mMapView = null;
        private MKOfflineMap mOffline = null;
        private TextView cidView;
        private TextView stateView;
        private EditText cityNameView;
        private MapController mMapController = null;
        /**
         * �����ص����ߵ�ͼ��Ϣ�б�
         */
        private IList<MKOLUpdateElement> localMapList = null;
        private LocalMapAdapter lAdapter = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            /**
             * ʹ�õ�ͼsdkǰ���ȳ�ʼ��BMapManager.
             * BMapManager��ȫ�ֵģ���Ϊ���MapView���ã�����Ҫ��ͼģ�鴴��ǰ������
             * ���ڵ�ͼ��ͼģ�����ٺ����٣�ֻҪ���е�ͼģ����ʹ�ã�BMapManager�Ͳ�Ӧ������
             */
            DemoApplication app = (DemoApplication)this.Application;
            if (app.mBMapManager == null)
            {
                app.mBMapManager = new BMapManager(ApplicationContext);
                /**
                 * ���BMapManagerû�г�ʼ�����ʼ��BMapManager
                 */
                app.mBMapManager.Init(new DemoApplication.MyGeneralListener());
            }
            SetContentView(Resource.Layout.activity_offline);
            mMapView = new MapView(this);
            mMapController = mMapView.Controller;

            mOffline = new MKOfflineMap();
            /**
             * ��ʼ�����ߵ�ͼģ��,MapControler�ɴ�MapView.getController()��ȡ
             */
            mOffline.Init(mMapController, this);
            InitView();

        }

        private void InitView()
        {

            cidView = FindViewById<TextView>(Resource.Id.cityid);
            cityNameView = FindViewById<EditText>(Resource.Id.city);
            stateView = FindViewById<TextView>(Resource.Id.state);

            ListView hotCityList = FindViewById<ListView>(Resource.Id.hotcitylist);
            IList<string> hotCities = new List<string>();
            //��ȡ���ֳ����б�
            IList<MKOLSearchRecord> records1 = mOffline.HotCityList;
            if (records1 != null)
            {
                foreach (MKOLSearchRecord r in records1)
                {
                    hotCities.Add(r.CityName + "(" + r.CityID + ")" + "   --" + this.FormatDataSize(r.Size));
                }
            }
            IListAdapter hAdapter = (IListAdapter)new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, hotCities);
            hotCityList.Adapter = hAdapter;

            ListView allCityList = FindViewById<ListView>(Resource.Id.allcitylist);
            //��ȡ����֧�����ߵ�ͼ�ĳ���
            IList<string> allCities = new List<string>();
            IList<MKOLSearchRecord> records2 = mOffline.OfflineCityList;
            if (records1 != null)
            {
                foreach (MKOLSearchRecord r in records2)
                {
                    allCities.Add(r.CityName + "(" + r.CityID + ")" + "   --" + this.FormatDataSize(r.Size));
                }
            }
            IListAdapter aAdapter = (IListAdapter)new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, allCities);
            allCityList.Adapter = aAdapter;

            LinearLayout cl = FindViewById<LinearLayout>(Resource.Id.citylist_layout);
            LinearLayout lm = FindViewById<LinearLayout>(Resource.Id.localmap_layout);
            lm.Visibility = ViewStates.Gone;
            cl.Visibility = ViewStates.Visible;

            //��ȡ���¹������ߵ�ͼ��Ϣ
            localMapList = mOffline.AllUpdateInfo;
            if (localMapList == null)
            {
                localMapList = new List<MKOLUpdateElement>();
            }

            ListView localMapListView = FindViewById<ListView>(Resource.Id.localmaplist);
            lAdapter = new LocalMapAdapter(this);
            localMapListView.Adapter = lAdapter;

        }
        /**
         * �л��������б�
         * @param view
         */
        [Java.Interop.Export]
        public void ClickCityListButton(View view)
        {
            LinearLayout cl = FindViewById<LinearLayout>(Resource.Id.citylist_layout);
            LinearLayout lm = FindViewById<LinearLayout>(Resource.Id.localmap_layout);
            lm.Visibility = ViewStates.Gone;
            cl.Visibility = ViewStates.Visible;

        }
        /**
         * �л������ع����б�
         * @param view
         */
        [Java.Interop.Export]
        public void ClickLocalMapListButton(View view)
        {
            LinearLayout cl = FindViewById<LinearLayout>(Resource.Id.citylist_layout);
            LinearLayout lm = FindViewById<LinearLayout>(Resource.Id.localmap_layout);
            lm.Visibility = ViewStates.Visible;
            cl.Visibility = ViewStates.Gone;
        }
        /**
         * ������������
         * @param view
         */
        [Java.Interop.Export]
        public void Search(View view)
        {
            IList<MKOLSearchRecord> records = mOffline.SearchCity(cityNameView.Text);
            if (records == null || records.Count != 1)
                return;
            cidView.Text = String.ValueOf(records[0].CityID);
        }

        /**
         * ��ʼ����
         * @param view
         */
        [Java.Interop.Export]
        public void Start(View view)
        {
            int cityid = Integer.ParseInt(cidView.Text);
            mOffline.Start(cityid);
            ClickLocalMapListButton(null);
            Toast.MakeText(this, "��ʼ�������ߵ�ͼ. cityid: " + cityid, ToastLength.Short)
                      .Show();
        }
        /**
         * ��ͣ����
         * @param view
         */
        [Java.Interop.Export]
        public void Stop(View view)
        {
            int cityid = Integer.ParseInt(cidView.Text);
            mOffline.Pause(cityid);
            Toast.MakeText(this, "��ͣ�������ߵ�ͼ. cityid: " + cityid, ToastLength.Short)
                      .Show();
        }
        /**
         * ɾ�����ߵ�ͼ
         * @param view
         */
        [Java.Interop.Export]
        public void Remove(View view)
        {
            int cityid = Integer.ParseInt(cidView.Text);
            mOffline.Remove(cityid);
            Toast.MakeText(this, "ɾ�����ߵ�ͼ. cityid: " + cityid, ToastLength.Short)
                      .Show();
        }
        /**
         * ��SD���������ߵ�ͼ��װ��
         * @param view
         */
        [Java.Interop.Export]
        public void ImportFromSDCard(View view)
        {
            int num = mOffline.Scan();
            string msg = "";
            if (num == 0)
            {
                msg = "û�е������߰�������������߰�����λ�ò���ȷ�������߰��Ѿ������";
            }
            else
            {
                msg = string.Format("�ɹ����� {0} �����߰������������ع���鿴", num);
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }
        /**
         * ����״̬��ʾ 
         */
        public void UpdateView()
        {
            localMapList = mOffline.AllUpdateInfo;
            if (localMapList == null)
            {
                localMapList = new List<MKOLUpdateElement>();
            }
            lAdapter.NotifyDataSetChanged();
        }




        protected override void OnPause()
        {
            int cityid = Integer.ParseInt(cidView.Text);
            mOffline.Pause(cityid);
            mMapView.OnPause();
            base.OnPause();
        }


        protected override void OnResume()
        {
            mMapView.OnResume();
            base.OnResume();
        }


        public string FormatDataSize(int size)
        {
            string ret = "";
            if (size < (1024 * 1024))
            {
                ret = string.Format("{0}K", size / 1024);
            }
            else
            {
                ret = string.Format("{0:F1}M", size / (1024 * 1024.0));
            }
            return ret;
        }


        protected override void OnDestroy()
        {
            /**
             * �˳�ʱ���������ߵ�ͼģ��
             */
            mOffline.Destroy();
            mMapView.Destroy();
            base.OnDestroy();
        }


        public void OnGetOfflineMapState(int type, int state)
        {
            switch (type)
            {
                case MKOfflineMap.TypeDownloadUpdate:
                    {
                        MKOLUpdateElement update = mOffline.GetUpdateInfo(state);
                        //�������ؽ��ȸ�����ʾ
                        if (update != null)
                        {
                            stateView.Text = string.Format("{0} : {1}" + "%", update.CityName, update.Ratio);
                            UpdateView();
                        }
                    }
                    break;
                case MKOfflineMap.TypeNewOffline:
                    //�������ߵ�ͼ��װ
                    Log.Debug("OfflineDemo", string.Format("add offlinemap num:{0}", state));
                    break;
                case MKOfflineMap.TypeVerUpdate:
                    // �汾������ʾ
                    //	MKOLUpdateElement e = mOffline.getUpdateInfo(state);

                    break;
            }

        }
        /**
         * ���ߵ�ͼ�����б�������
         */
        public class LocalMapAdapter : BaseAdapter
        {

            class IOnClickRemoveListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
            {
                private LocalMapAdapter localMapAdapter;
                private MKOLUpdateElement e;

                public IOnClickRemoveListenerImpl(LocalMapAdapter localMapAdapter, MKOLUpdateElement e)
                {
                    this.localMapAdapter = localMapAdapter;
                    this.e = e;
                }

                public void OnClick(View arg0)
                {
                    localMapAdapter.offlineDemo.mOffline.Remove(e.CityID);
                    localMapAdapter.offlineDemo.UpdateView();
                }
            }

            class IOnClickDisplayListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
            {
                private LocalMapAdapter localMapAdapter;
                private MKOLUpdateElement e;

                public IOnClickDisplayListenerImpl(LocalMapAdapter localMapAdapter, MKOLUpdateElement e)
                {
                    this.localMapAdapter = localMapAdapter;
                    this.e = e;
                }

                public void OnClick(View arg0)
                {
                    Intent intent = new Intent();
                    intent.PutExtra("x", e.GeoPt.LongitudeE6);
                    intent.PutExtra("y", e.GeoPt.LatitudeE6);
                    intent.SetClass(localMapAdapter.offlineDemo, typeof(BaseMapDemo));
                    localMapAdapter.offlineDemo.StartActivity(intent);
                }
            }

            private OfflineDemo offlineDemo;

            public LocalMapAdapter(OfflineDemo offlineDemo)
            {
                this.offlineDemo = offlineDemo;
            }

            public override int Count
            {
                get { return offlineDemo.localMapList.Count; }
            }

            public override Object GetItem(int index)
            {
                return offlineDemo.localMapList[index];
            }


            public override long GetItemId(int index)
            {
                return index;
            }


            public override View GetView(int index, View view, ViewGroup arg2)
            {
                MKOLUpdateElement e = (MKOLUpdateElement)GetItem(index);
                view = View.Inflate(offlineDemo, Resource.Layout.offline_localmap_list, null);
                InitViewItem(view, e);
                return view;
            }

            void InitViewItem(View view, MKOLUpdateElement e)
            {
                Button display = view.FindViewById<Button>(Resource.Id.display);
                Button remove = view.FindViewById<Button>(Resource.Id.remove);
                TextView title = view.FindViewById<TextView>(Resource.Id.title);
                TextView update = view.FindViewById<TextView>(Resource.Id.update);
                TextView ratio = view.FindViewById<TextView>(Resource.Id.ratio);

                ratio.Text = e.Ratio + "%";
                title.Text = e.CityName;
                if (e.Update)
                {
                    update.Text = "�ɸ���";
                }
                else
                {
                    update.Text = "����";
                }
                if (e.Ratio != 100)
                {
                    display.Enabled = false;
                }
                else
                {
                    display.Enabled = true;
                }




                remove.SetOnClickListener(new IOnClickRemoveListenerImpl(this, e));


                display.SetOnClickListener(new IOnClickDisplayListenerImpl(this, e));
            }




        }



    }
}