using Android.App;
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

namespace baidumapsdk.demo
{
    /**
     * ��demo����չʾ��ν��е�������������õ�ַ�������꣩����������������������������ַ��
     * ͬʱչʾ�����ʹ��ItemizedOverlay�ڵ�ͼ�ϱ�ע�����
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_geocode", ScreenOrientation = ScreenOrientation.Sensor)]
    public class GeoCoderDemo : Activity
    {
        //UI���
        Button mBtnReverseGeoCode = null;	// �����귴����Ϊ��ַ
        Button mBtnGeoCode = null;	// ����ַ����Ϊ����

        //��ͼ���
        MapView mMapView = null;	// ��ͼView
        //�������
        MKSearch mSearch = null;	// ����ģ�飬Ҳ��ȥ����ͼģ�����ʹ��

        class ClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            GeoCoderDemo geoCoderDemo;

            public ClickListenerImpl(GeoCoderDemo geoCoderDemo)
            {
                this.geoCoderDemo = geoCoderDemo;
            }

            public void OnClick(View v)
            {
                geoCoderDemo.SearchButtonProcess(v);
            }
        }

        class IMKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {
            GeoCoderDemo geoCoderDemo;

            public IMKSearchListenerImpl(GeoCoderDemo geoCoderDemo)
            {
                this.geoCoderDemo = geoCoderDemo;
            }

            public void OnGetPoiDetailSearchResult(int type, int error)
            {
            }

            public void OnGetAddrResult(MKAddrInfo res, int error)
            {
                if (error != 0)
                {
                    string str = String.Format("����ţ�%d", error);
                    Toast.MakeText(geoCoderDemo, str, ToastLength.Long).Show();
                    return;
                }
                //��ͼ�ƶ����õ�
                geoCoderDemo.mMapView.Controller.AnimateTo(res.GeoPt);
                if (res.Type == MKAddrInfo.MkGeocode)
                {
                    //������룺ͨ����ַ���������
                    string strInfo = String.Format("γ�ȣ�%f ���ȣ�%f", res.GeoPt.LatitudeE6 / 1e6, res.GeoPt.LongitudeE6 / 1e6);
                    Toast.MakeText(geoCoderDemo, strInfo, ToastLength.Long).Show();
                }
                if (res.Type == MKAddrInfo.MkReversegeocode)
                {
                    //��������룺ͨ������������ϸ��ַ���ܱ�poi
                    string strInfo = res.StrAddr;
                    Toast.MakeText(geoCoderDemo, strInfo, ToastLength.Long).Show();
                }
                //����ItemizedOverlayͼ��������ע�����
                ItemizedOverlay<OverlayItem> itemOverlay = new ItemizedOverlay<OverlayItem>(null, geoCoderDemo.mMapView);
                //����Item
                OverlayItem item = new OverlayItem(res.GeoPt, "", null);
                //�õ���Ҫ���ڵ�ͼ�ϵ���Դ
                Drawable marker = geoCoderDemo.Resources.GetDrawable(Resource.Drawable.icon_markf);
                //Ϊmaker����λ�úͱ߽�
                marker.SetBounds(0, 0, marker.IntrinsicWidth, marker.IntrinsicHeight);
                //��item����marker
                item.Marker = marker;
                //��ͼ�������item
                itemOverlay.AddItem(item);

                //�����ͼ����ͼ��
                geoCoderDemo.mMapView.Overlays.Clear();
                //���һ����עItemizedOverlayͼ��
                geoCoderDemo.mMapView.Overlays.Add(itemOverlay);
                //ִ��ˢ��ʹ��Ч
                geoCoderDemo.mMapView.Refresh();
            }

            public void OnGetPoiResult(MKPoiResult res, int type, int error)
            {
            }

            public void OnGetDrivingRouteResult(MKDrivingRouteResult res, int error)
            {
            }

            public void OnGetTransitRouteResult(MKTransitRouteResult res, int error)
            {
            }

            public void OnGetWalkingRouteResult(MKWalkingRouteResult res, int error)
            {
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
                // TODO Auto-generated method stub
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
            SetContentView(Resource.Layout.geocoder);
            ICharSequence titleLable = new String("������빦��");
            Title = titleLable.ToString();

            //��ͼ��ʼ��
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.EnableClick(true);
            mMapView.Controller.SetZoom(12);

            // ��ʼ������ģ�飬ע���¼�����
            mSearch = new MKSearch();
            mSearch.Init(app.mBMapManager, new IMKSearchListenerImpl(this));

            // �趨������뼰��������밴ť����Ӧ
            mBtnReverseGeoCode = FindViewById<Button>(Resource.Id.reversegeocode);
            mBtnGeoCode = FindViewById<Button>(Resource.Id.geocode);

            Android.Views.View.IOnClickListener clickListener = new ClickListenerImpl(this);

            mBtnReverseGeoCode.SetOnClickListener(clickListener);
            mBtnGeoCode.SetOnClickListener(clickListener);
        }

        /**
         * ��������
         * @param v
         */
        void SearchButtonProcess(View v)
        {
            if (mBtnReverseGeoCode.Equals(v))
            {
                EditText lat = FindViewById<EditText>(Resource.Id.lat);
                EditText lon = FindViewById<EditText>(Resource.Id.lon);
                GeoPoint ptCenter = new GeoPoint((int)(float.Parse(lat.Text) * 1e6), (int)(float.Parse(lon.Text) * 1e6));
                //��Geo����
                mSearch.ReverseGeocode(ptCenter);
            }
            else if (mBtnGeoCode.Equals(v))
            {
                EditText editCity = FindViewById<EditText>(Resource.Id.city);
                EditText editGeoCodeKey = FindViewById<EditText>(Resource.Id.geocodekey);
                //Geo����
                mSearch.Geocode(editGeoCodeKey.Text, editCity.Text);
            }
        }

        protected override void OnDestroy()
        {
            mMapView.Destroy();
            mSearch.Destory();
            base.OnDestroy();
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
    }
}