using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using baidumapsdk.demo;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Cloud;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Search;
using Com.Baidu.Platform.Comapi.Basestruct;
using Java.Lang;
using System.Collections.Generic;

namespace baidumapsdk.demo
{
    /**
     * ��demo����չʾ������Լ������ݹ���һ��·���ڵ�ͼ�ϻ��Ƴ���
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, ScreenOrientation = ScreenOrientation.Sensor)]
    public class CustomRouteOverlayDemo : Activity
    {
        //��ͼ���
        MapView mMapView = null;	// ��ͼView
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
            SetContentView(Resource.Layout.activity_customroute);
            ICharSequence titleLable = new String("·�߹滮���ܡ�������·��ʾ��");
            Title = titleLable.ToString();
            //��ʼ����ͼ
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.EnableClick(true);
            mMapView.Controller.SetZoom(13);

            /** ��ʾ�Զ���·��ʹ�÷���	
             *  �ڱ�����ͼ�ϻ�һ����������
             *  ��֪��ĳ����İٶȾ�γ������������http://api.map.baidu.com/lbsapi/getpoint/index.html	
             */
            GeoPoint p1 = new GeoPoint((int)(39.9411 * 1E6), (int)(116.3714 * 1E6));
            GeoPoint p2 = new GeoPoint((int)(39.9498 * 1E6), (int)(116.3785 * 1E6));
            GeoPoint p3 = new GeoPoint((int)(39.9436 * 1E6), (int)(116.4029 * 1E6));
            GeoPoint p4 = new GeoPoint((int)(39.9329 * 1E6), (int)(116.4035 * 1E6));
            GeoPoint p5 = new GeoPoint((int)(39.9218 * 1E6), (int)(116.4115 * 1E6));
            GeoPoint p6 = new GeoPoint((int)(39.9144 * 1E6), (int)(116.4230 * 1E6));
            GeoPoint p7 = new GeoPoint((int)(39.9126 * 1E6), (int)(116.4387 * 1E6));
            //�������
            GeoPoint start = p1;
            //�յ�����
            GeoPoint stop = p7;
            //��һվ��վ������Ϊp3,����p1,p2
            GeoPoint[] step1 = new GeoPoint[3];
            step1[0] = p1;
            step1[1] = p2;
            step1[2] = p3;
            //�ڶ�վ��վ������Ϊp5,����p4
            GeoPoint[] step2 = new GeoPoint[2];
            step2[0] = p4;
            step2[1] = p5;
            //����վ��վ������Ϊp7,����p6
            GeoPoint[] step3 = new GeoPoint[2];
            step3[0] = p6;
            step3[1] = p7;
            //վ�����ݱ�����һ����ά������
            GeoPoint[][] routeData = new GeoPoint[3][];
            routeData[0] = step1;
            routeData[1] = step2;
            routeData[2] = step3;
            //��վ�����ݹ���һ��MKRoute
            MKRoute route = new MKRoute();
            route.CustomizeRoute(start, stop, routeData);
            //������վ����Ϣ��MKRoute��ӵ�RouteOverlay��
            RouteOverlay routeOverlay = new RouteOverlay(this, mMapView);
            routeOverlay.SetData(route);
            //���ͼ��ӹ���õ�RouteOverlay
            mMapView.Overlays.Add(routeOverlay);
            //ִ��ˢ��ʹ��Ч
            mMapView.Refresh();
        }

        protected override void OnDestroy()
        {
            mMapView.Destroy();
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