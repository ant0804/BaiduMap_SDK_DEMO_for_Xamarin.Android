
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
using Com.Baidu.Mapapi.PanoramaX;
using Android.Graphics;
using Android.Runtime;

namespace baidumapsdk.demo
{
    [Activity(Label = "@string/title_activity_panorama_poi_selector")]
    public class PanoramaPoiSelectorActivity : FragmentActivity
    {
        MKSearch mSearch = null;
        MapView mMapView = null;

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
            /**
             * ����MapView��setContentView()�г�ʼ��,��������Ҫ��BMapManager��ʼ��֮��
             */
            SetContentView(Resource.Layout.activity_panorama_poi_selector);
            InitMap();
            InitSearcher();
        }

        private void InitMap()
        {
            mMapView = (Android.Runtime.Extensions.JavaCast<SupportMapFragment>(SupportFragmentManager
                    .FindFragmentById(Resource.Id.map)).MapView);
            GeoPoint p = new GeoPoint((int)(39.945 * 1E6), (int)(116.404 * 1E6));
            mMapView.Controller.SetCenter(p);
            mMapView.Controller.SetZoom(13);
        }

        class IMKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {
            PanoramaPoiSelectorActivity panoramaPoiSelectorActivity;
            public IMKSearchListenerImpl(PanoramaPoiSelectorActivity panoramaPoiSelectorActivity)
            {
                this.panoramaPoiSelectorActivity = panoramaPoiSelectorActivity;
            }

            public void OnGetPoiResult(MKPoiResult res, int type, int iError)
            {
                if (iError != 0)
                {
                    Toast.MakeText(panoramaPoiSelectorActivity,
                            "��Ǹ��δ���ҵ����", ToastLength.Short).Show();
                    return;
                }
                if (res.CurrentNumPois > 0)
                {
                    // ��poi�����ʾ����ͼ��
                    SelectPoiOverlay poiOverlay = new SelectPoiOverlay(panoramaPoiSelectorActivity, panoramaPoiSelectorActivity.mMapView);
                    poiOverlay.SetData(res.AllPoi);
                    panoramaPoiSelectorActivity.mMapView.Overlays.Clear();
                    panoramaPoiSelectorActivity.mMapView.Overlays.Add(poiOverlay);
                    panoramaPoiSelectorActivity.mMapView.Refresh();
                    //��ePoiTypeΪ2��������·����4��������·��ʱ�� poi����Ϊ��
                    foreach (MKPoiInfo info in res.AllPoi)
                    {
                        if (info.Pt != null)
                        {
                            panoramaPoiSelectorActivity.mMapView.Controller.AnimateTo(info.Pt);
                            break;
                        }
                    }
                }

            }


            public void OnGetTransitRouteResult(MKTransitRouteResult result, int iError)
            {
            }


            public void OnGetDrivingRouteResult(MKDrivingRouteResult result, int iError)
            {
            }


            public void OnGetWalkingRouteResult(MKWalkingRouteResult result, int iError)
            {
            }


            public void OnGetAddrResult(MKAddrInfo result, int iError)
            {
            }


            public void OnGetBusDetailResult(MKBusLineResult result, int iError)
            {
            }


            public void OnGetSuggestionResult(MKSuggestionResult result, int iError)
            {
            }


            public void OnGetPoiDetailSearchResult(int type, int iError)
            {
            }


            public void OnGetShareUrlResult(MKShareUrlResult result, int type, int error)
            {

            }
        }

        private void InitSearcher()
        {
            mSearch = new MKSearch();
            mSearch.Init(((DemoApplication)this.Application).mBMapManager, new IMKSearchListenerImpl(this));
        }

        [Java.Interop.Export]
        public void DoPoiSearch(View v)
        {
            mSearch.PoiSearchInCity((FindViewById<EditText>(Resource.Id.city)).Text,
                    (FindViewById<EditText>(Resource.Id.key)).Text);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mSearch != null)
            {
                mSearch.Destory();
                mSearch = null;
            }
        }

        private class SelectPoiOverlay : PoiOverlay
        {
            Activity activity;

            public SelectPoiOverlay(Activity activity, MapView mapView) :
                base(activity, mapView)
            {
                this.activity = activity;
            }


            protected override bool OnTap(int i)
            {
                base.OnTap(i);
                MKPoiInfo info = GetPoi(i);
                if (!info.IsPano)
                {
                    Toast.MakeText(activity,
                            "��ǰPOI��������ȫ����Ϣ", ToastLength.Short).Show();
                }
                else
                {
                    Intent intent = new Intent();
                    intent.SetClass(activity,
                            typeof(PanoramaDemoActivityMain));
                    intent.PutExtra("uid", info.Uid);
                    activity.StartActivity(intent);
                }
                return true;
            }
        }
    }
}