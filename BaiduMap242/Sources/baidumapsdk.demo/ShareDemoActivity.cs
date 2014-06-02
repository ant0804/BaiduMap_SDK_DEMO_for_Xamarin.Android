using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using baidumapsdk.demo;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Search;
using Com.Baidu.Platform.Comapi.Basestruct;

namespace baidumapsdk.demo
{
    /**
     * ��ʾpoi�������� 
     */
    [Activity(Label = "@string/demo_name_share")]
    public class ShareDemoActivity : Activity
    {
        private MapView mMapView = null;
        private MKSearch mSearch = null;   // ����ģ�飬Ҳ��ȥ����ͼģ�����ʹ��
        //�������������ַ
        private string currentAddr = null;
        //�������� 
        private string mCity = "����";
        //�����ؼ���
        private string searchKey = "�͹�";
        //��������������
        private GeoPoint mPoint = new GeoPoint((int)(40.056878 * 1E6), (int)(116.308141 * 1E6));

        class IMKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {
            ShareDemoActivity shareDemoActivity;
            public IMKSearchListenerImpl(ShareDemoActivity shareDemoActivity)
            {
                this.shareDemoActivity = shareDemoActivity;
            }

            public void OnGetPoiDetailSearchResult(int type, int error)
            {
            }
            /**
             * �ڴ˴���poi������� , ��poioverlay ��ʾ
             */
            public void OnGetPoiResult(MKPoiResult res, int type, int error)
            {
                // ����ſɲο�MKEvent�еĶ���
                if (error != 0 || res == null)
                {
                    Toast.MakeText(shareDemoActivity, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }
                // ����ͼ�ƶ�����һ��POI���ĵ�
                if (res.CurrentNumPois > 0)
                {
                    // ��poi�����ʾ����ͼ��
                    PoiShareOverlay poiOverlay = new PoiShareOverlay(shareDemoActivity, shareDemoActivity.mMapView);
                    poiOverlay.SetData(res.AllPoi);
                    shareDemoActivity.mMapView.Overlays.Clear();
                    shareDemoActivity.mMapView.Overlays.Add(poiOverlay);
                    shareDemoActivity.mMapView.Refresh();
                    //��ePoiTypeΪ2��������·����4��������·��ʱ�� poi����Ϊ��
                    foreach (MKPoiInfo info in res.AllPoi)
                    {
                        if (info.Pt != null)
                        {
                            shareDemoActivity.mMapView.Controller.AnimateTo(info.Pt);
                            break;
                        }
                    }
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
            /**
             * �ڴ˴����������
             */
            public void OnGetAddrResult(MKAddrInfo res, int error)
            {
                // ����ſɲο�MKEvent�еĶ���
                if (error != 0 || res == null)
                {
                    Toast.MakeText(shareDemoActivity, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }
                AddrShareOverlay addrOverlay = new AddrShareOverlay(shareDemoActivity, shareDemoActivity.Resources.GetDrawable(Resource.Drawable.icon_marka), shareDemoActivity.mMapView, res);
                shareDemoActivity.mMapView.Overlays.Clear();
                shareDemoActivity.mMapView.Overlays.Add(addrOverlay);
                shareDemoActivity.mMapView.Refresh();

            }
            public void OnGetBusDetailResult(MKBusLineResult result, int iError)
            {
            }

            public void OnGetSuggestionResult(MKSuggestionResult res, int arg1)
            {
            }
            public void OnGetShareUrlResult(MKShareUrlResult result, int type,
                    int error)
            {
                //����̴����
                Intent it = new Intent(Intent.ActionSend);
                it.PutExtra(Intent.ExtraText, "��������ͨ���ٶȵ�ͼSDK��������һ��λ��: " +
                               shareDemoActivity.currentAddr +
                               " -- " + result.Url);
                it.SetType("text/plain");
                shareDemoActivity.StartActivity(Intent.CreateChooser(it, "���̴�����"));

            }
        }

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
            SetContentView(Resource.Layout.activity_share_demo_activity);
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.EnableClick(true);
            mMapView.Controller.SetZoom(12);

            // ��ʼ������ģ�飬ע�������¼�����
            mSearch = new MKSearch();
            mSearch.Init(app.mBMapManager, new IMKSearchListenerImpl(this)
            {


            });
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

        private void InitMapView()
        {
            mMapView.LongClickable = true;
            mMapView.Controller.SetZoom(14);
            mMapView.Controller.EnableClick(true);
            mMapView.SetBuiltInZoomControls(true);
        }

        [Java.Interop.Export]
        public void SharePoi(View view)
        {
            //����poi����
            mSearch.PoiSearchInCity(mCity, searchKey);
            Toast.MakeText(this,
                    "��" + mCity + "���� " + searchKey,
                    ToastLength.Short).Show();
        }

        [Java.Interop.Export]
        public void ShareAddr(View view)
        {
            //���𷴵����������
            mSearch.ReverseGeocode(mPoint);
            Toast.MakeText(this,
                    string.Format("����λ�ã� %f��%f", (mPoint.LatitudeE6 * 1E-6), (mPoint.LongitudeE6 * 1E-6)),
                    ToastLength.Short).Show();
        }

        /**
         * ʹ��PoiOverlay չʾpoi�㣬��poi�����ʱ����̴�����.
         *
         */
        private class PoiShareOverlay : PoiOverlay
        {
            ShareDemoActivity shareDemoActivity;
            public PoiShareOverlay(Activity activity, MapView mapView) :
                base(activity, mapView)
            {
                this.shareDemoActivity = (ShareDemoActivity)activity;
            }


            protected override bool OnTap(int i)
            {
                MKPoiInfo info = GetPoi(i);
                shareDemoActivity.currentAddr = info.Address;
                shareDemoActivity.mSearch.PoiDetailShareURLSearch(info.Uid);
                return true;
            }
        }
        /**
         * ʹ��ItemizevOvelrayչʾ����������λ�ã����õ㱻���ʱ����̴�����.
         *
         */
        private class AddrShareOverlay : ItemizedOverlay
        {

            ShareDemoActivity shareDemoActivity;


            private MKAddrInfo addrInfo;

            public AddrShareOverlay(ShareDemoActivity shareDemoActivity, Drawable defaultMarker, MapView mapView, MKAddrInfo addrInfo) :
                base(defaultMarker, mapView)
            {
                this.shareDemoActivity = shareDemoActivity;
                this.addrInfo = addrInfo;
                AddItem(new OverlayItem(addrInfo.GeoPt, addrInfo.StrAddr, addrInfo.StrAddr));
            }

            protected override bool OnTap(int index)
            {
                shareDemoActivity.currentAddr = addrInfo.StrAddr;
                shareDemoActivity.mSearch.PoiRGCShareURLSearch(addrInfo.GeoPt, "�����ַ", addrInfo.StrAddr);
                return true;
            }

        }
    }
}