
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

namespace baidumapsdk.demo
{
    /**
     * ��һ��Activity��չʾ�����ͼ
     */
    [Android.App.Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_mutimap", ScreenOrientation = ScreenOrientation.Sensor)]
    public class MutiMapViewDemo : FragmentActivity
    {
        private MapController mMapController1, mMapController2, mMapController3, mMapController4;

        private static readonly GeoPoint GEO_BEIJING = new GeoPoint((int)(39.945 * 1E6), (int)(116.404 * 1E6));
        private static readonly GeoPoint GEO_SHANGHAI = new GeoPoint((int)(31.227 * 1E6), (int)(121.481 * 1E6));
        private static readonly GeoPoint GEO_GUANGZHOU = new GeoPoint((int)(23.155 * 1E6), (int)(113.264 * 1E6));
        private static readonly GeoPoint GEO_SHENGZHENG = new GeoPoint((int)(22.560 * 1E6), (int)(114.064 * 1E6));

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
            SetContentView(Resource.Layout.activity_mutimap);
            InitMap();
        }


        /**
         * ��ʼ��Map
         */
        private void InitMap()
        {
            // ������
            if (mMapController1 == null)
            {
                mMapController1 = Android.Runtime.Extensions.JavaCast<SupportMapFragment>(SupportFragmentManager
                        .FindFragmentById(Resource.Id.map1)).MapView.Controller;
                mMapController1.SetMapStatus(NewMapStatusWithGeoPointAndZoom(GEO_BEIJING, 10));
            }

            // �Ϻ���
            if (mMapController2 == null)
            {
                mMapController2 = Android.Runtime.Extensions.JavaCast<SupportMapFragment>(SupportFragmentManager
                        .FindFragmentById(Resource.Id.map2)).MapView.Controller;
                mMapController2.SetMapStatus(NewMapStatusWithGeoPointAndZoom(GEO_SHANGHAI, 10));
            }

            // ������
            if (mMapController3 == null)
            {
                mMapController3 = Android.Runtime.Extensions.JavaCast<SupportMapFragment>(SupportFragmentManager
                        .FindFragmentById(Resource.Id.map3)).MapView.Controller;
                mMapController3.SetMapStatus(NewMapStatusWithGeoPointAndZoom(GEO_GUANGZHOU, 10));
            }

            // ������
            if (mMapController4 == null)
            {
                mMapController4 = Android.Runtime.Extensions.JavaCast<SupportMapFragment>(SupportFragmentManager
                        .FindFragmentById(Resource.Id.map4)).MapView.Controller;
                mMapController4.SetMapStatus(NewMapStatusWithGeoPointAndZoom(GEO_SHENGZHENG, 10));
            }
        }

        private MKMapStatus NewMapStatusWithGeoPointAndZoom(GeoPoint p, float zoom)
        {
            MKMapStatus status = new MKMapStatus();
            status.TargetGeo = p;
            status.Zoom = zoom;
            return status;
        }
    }
}