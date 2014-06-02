
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
using Android.Support.V4.App;
using Android.Content.Res;
using Android.App;
using Android.Text;

namespace baidumapsdk.demo
{
    /**
     * ��ʾpoi�������� 
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_poi", ScreenOrientation = ScreenOrientation.Sensor)]
    public class PoiSearchDemo : Activity
    {
        private MapView mMapView = null;
        private MKSearch mSearch = null;   // ����ģ�飬Ҳ��ȥ����ͼģ�����ʹ��
        /**
         * �����ؼ������봰��
         */
        private AutoCompleteTextView keyWorldsView = null;
        private ArrayAdapter<string> sugAdapter = null;
        private int load_Index;

        class IMKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {
            PoiSearchDemo poiSearchDemo;

            public IMKSearchListenerImpl(PoiSearchDemo poiSearchDemo)
            {
                this.poiSearchDemo = poiSearchDemo;
            }

            //�ڴ˴�������ҳ���            
            public void OnGetPoiDetailSearchResult(int type, int error)
            {
                if (error != 0)
                {
                    Toast.MakeText(poiSearchDemo, "��Ǹ��δ�ҵ����", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(poiSearchDemo, "�ɹ����鿴����ҳ��", ToastLength.Short).Show();
                }
            }

            /**
             * �ڴ˴���poi�������
             */
            public void OnGetPoiResult(MKPoiResult res, int type, int error)
            {
                // ����ſɲο�MKEvent�еĶ���
                if (error != 0 || res == null)
                {
                    Toast.MakeText(poiSearchDemo, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }
                // ����ͼ�ƶ�����һ��POI���ĵ�
                if (res.CurrentNumPois > 0)
                {
                    // ��poi�����ʾ����ͼ��
                    MyPoiOverlay poiOverlay = new MyPoiOverlay(poiSearchDemo, poiSearchDemo.mMapView, poiSearchDemo.mSearch);
                    poiOverlay.SetData(res.AllPoi);
                    poiSearchDemo.mMapView.Overlays.Clear();
                    poiSearchDemo.mMapView.Overlays.Add(poiOverlay);
                    poiSearchDemo.mMapView.Refresh();
                    //��ePoiTypeΪ2��������·����4��������·��ʱ�� poi����Ϊ��
                    foreach (MKPoiInfo info in res.AllPoi)
                    {
                        if (info.Pt != null)
                        {
                            poiSearchDemo.mMapView.Controller.AnimateTo(info.Pt);
                            break;
                        }
                    }
                }
                else if (res.CityListNum > 0)
                {
                    //������ؼ����ڱ���û���ҵ����������������ҵ�ʱ�����ذ����ùؼ�����Ϣ�ĳ����б�
                    string strInfo = "��";
                    for (int i = 0; i < res.CityListNum; i++)
                    {
                        strInfo += res.GetCityListInfo(i).City;
                        strInfo += ",";
                    }
                    strInfo += "�ҵ����";
                    Toast.MakeText(poiSearchDemo, strInfo, ToastLength.Long).Show();
                }
            }
            public void OnGetDrivingRouteResult(MKDrivingRouteResult res,
                    int error)
            {
            }
            public void OnGetTransitRouteResult(MKTransitRouteResult res,
                    int error)
            {
            }
            public void OnGetWalkingRouteResult(MKWalkingRouteResult res,
                    int error)
            {
            }
            public void OnGetAddrResult(MKAddrInfo res, int error)
            {
            }
            public void OnGetBusDetailResult(MKBusLineResult result, int iError)
            {
            }
            /**
             * ���½����б�
             */
            public void OnGetSuggestionResult(MKSuggestionResult res, int arg1)
            {
                if (res == null || res.AllSuggestions == null)
                {
                    return;
                }
                poiSearchDemo.sugAdapter.Clear();
                foreach (MKSuggestionInfo info in res.AllSuggestions)
                {
                    if (info.Key != null)
                        poiSearchDemo.sugAdapter.Add(info.Key);
                }
                poiSearchDemo.sugAdapter.NotifyDataSetChanged();

            }

            public void OnGetShareUrlResult(MKShareUrlResult result, int type,
                    int error)
            {
                // TODO Auto-generated method stub

            }
        }

        class ITextWatcherImpl : Java.Lang.Object, ITextWatcher
        {
            PoiSearchDemo poiSearchDemo;

            public ITextWatcherImpl(PoiSearchDemo poiSearchDemo)
            {
                this.poiSearchDemo = poiSearchDemo;
            }

            public void AfterTextChanged(IEditable arg0)
            {
            }

            public void BeforeTextChanged(ICharSequence arg0, int arg1,
                    int arg2, int arg3)
            {
            }

            public void OnTextChanged(ICharSequence cs, int arg1, int arg2,
                    int arg3)
            {
                if (cs.Length() <= 0)
                {
                    return;
                }
                string city = (poiSearchDemo.FindViewById<EditText>(Resource.Id.city)).Text;
                /**
                 * ʹ�ý������������ȡ�����б������OnSuggestionResult()�и���
                 */
                poiSearchDemo.mSearch.SuggestionSearch(cs.ToString(), city);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            /**
             * ʹ�õ�ͼsdkǰ���ȳ�ʼ��BMapManageResource.
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

            SetContentView(Resource.Layout.activity_poisearch);
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.EnableClick(true);
            mMapView.Controller.SetZoom(12);

            // ��ʼ������ģ�飬ע�������¼�����
            mSearch = new MKSearch();
            mSearch.Init(app.mBMapManager, new IMKSearchListenerImpl(this));

            keyWorldsView = FindViewById<AutoCompleteTextView>(Resource.Id.searchkey);
            sugAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line);
            keyWorldsView.Adapter = sugAdapter;

            /**
             * ������ؼ��ֱ仯ʱ����̬���½����б�
             */

            keyWorldsView.AddTextChangedListener(new ITextWatcherImpl(this));
        }

        protected override void OnPause()
        {
            mMapView.OnPause();
            base.OnPause();
        }

        protected override void OnResume()
        {
            mMapView.OnResume();
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            mMapView.Destroy();
            mSearch.Destory();
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mMapView.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            mMapView.OnRestoreInstanceState(savedInstanceState);
        }

        private void initMapView()
        {
            mMapView.LongClickable = true;
            mMapView.Controller.SetZoom(14);
            mMapView.Controller.EnableClick(true);
            mMapView.SetBuiltInZoomControls(true);
        }

        /**
         * Ӱ��������ť����¼�
         * @param v
         */
        [Java.Interop.Export]
        public void SearchButtonProcess(View v)
        {
            EditText editCity = FindViewById<EditText>(Resource.Id.city);
            EditText editSearchKey = FindViewById<EditText>(Resource.Id.searchkey);
            mSearch.PoiSearchInCity(editCity.Text,
                    editSearchKey.Text);
        }

        [Java.Interop.Export]
        public void GoToNextPage(View v)
        {
            //������һ��poi
            int flag = mSearch.GoToPoiPage(++load_Index);
            if (flag != 0)
            {
                Toast.MakeText(this, "��������ʼ��Ȼ����������һ������", ToastLength.Short).Show();
            }
        }
    }
}