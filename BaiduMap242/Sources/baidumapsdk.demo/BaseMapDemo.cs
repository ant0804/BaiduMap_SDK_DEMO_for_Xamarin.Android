using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using baidumapsdk.demo;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Platform.Comapi.Basestruct;

namespace baidumapsdk.demo
{
    /**
     * ��ʾMapView�Ļ����÷�
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_basemap", ScreenOrientation = ScreenOrientation.Sensor)]
    public class BaseMapDemo : Activity
    {
        // readonly static string TAG = "MainActivity";

        /**
         *  MapView �ǵ�ͼ���ؼ�
         */
        private MapView mMapView = null;

        /**
         *  ��MapController��ɵ�ͼ���� 
         */
        private MapController mMapController = null;

        /**
         *  MKMapViewListener ���ڴ����ͼ�¼��ص�
         */
        MKMapViewListenerImpl mMapListener = null;

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
            SetContentView(Resource.Layout.activity_main);

            mMapView = FindViewById<MapView>(Resource.Id.bmapView);

            /**
             * ��ȡ��ͼ������
             */
            mMapController = mMapView.Controller;

            /**
             *  ���õ�ͼ�Ƿ���Ӧ����¼�  .
             */
            mMapController.EnableClick(true);

            /**
             * ���õ�ͼ���ż���
             */
            mMapController.SetZoom(12);

            /**
             * ����ͼ�ƶ���ָ����
             * ʹ�ðٶȾ�γ�����꣬����ͨ��http://api.map.baidu.com/lbsapi/getpoint/index.html��ѯ��������
             * �����Ҫ�ڰٶȵ�ͼ����ʾʹ����������ϵͳ��λ�ã��뷢�ʼ���mapapi@baidu.com��������ת���ӿ�
             */
            GeoPoint p;
            double cLat = 39.945;
            double cLon = 116.404;

            var intent = Intent;

            if (intent.HasExtra("x") && intent.HasExtra("y"))
            {
                //����intent����ʱ���������ĵ�Ϊָ����
                Bundle b = intent.Extras;

                p = new GeoPoint(b.GetInt("y"), b.GetInt("x"));
            }
            else
            {
                //�������ĵ�Ϊ�찲��
                p = new GeoPoint((int)(cLat * 1E6), (int)(cLon * 1E6));
            }

            mMapController.SetCenter(p);

            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.onPause()
             */
            mMapListener = new MKMapViewListenerImpl(this);

            mMapView.RegMapViewListener(DemoApplication.getInstance().mBMapManager, mMapListener);
        }

        private class MKMapViewListenerImpl : Java.Lang.Object, IMKMapViewListener
        {
            private BaseMapDemo baseMapDemo;

            public MKMapViewListenerImpl(BaseMapDemo baseMapDemo)
            {
                this.baseMapDemo = baseMapDemo;
            }

            public void OnMapMoveFinish()
            {
                /**
                 * �ڴ˴����ͼ�ƶ���ɻص�
                 * ���ţ�ƽ�ƵȲ�����ɺ󣬴˻ص�������
                 */
            }

            public void OnClickMapPoi(MapPoi mapPoiInfo)
            {
                /**
                 * �ڴ˴����ͼpoi����¼�
                 * ��ʾ��ͼpoi���Ʋ��ƶ����õ�
                 * ���ù��� mMapController.enableClick(true); ʱ���˻ص����ܱ�����
                 * 
                 */
                string title = "";

                if (mapPoiInfo != null)
                {
                    title = mapPoiInfo.StrText;
                    Toast.MakeText(baseMapDemo, title, ToastLength.Short).Show();
                    baseMapDemo.mMapController.AnimateTo(mapPoiInfo.GeoPt);
                }
            }

            public void OnGetCurrentMap(Bitmap b)
            {
                /**
                 *  �����ù� mMapView.getCurrentMap()�󣬴˻ص��ᱻ����
                 *  ���ڴ˱����ͼ���洢�豸
                 */
            }

            public void OnMapAnimationFinish()
            {
                /**
                 *  ��ͼ��ɴ������Ĳ�������: animationTo()���󣬴˻ص�������
                 */
            }

            /**
             * �ڴ˴����ͼ������¼� 
             */
            public void OnMapLoadFinish()
            {
                Toast.MakeText(baseMapDemo, "��ͼ�������", ToastLength.Short).Show();
            }
        }

        protected override void OnPause()
        {
            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.onPause()
             */
            mMapView.OnPause();
            base.OnPause();
        }

        protected override void OnResume()
        {
            /**
             *  MapView������������Activityͬ������activity�ָ�ʱ�����MapView.onResume()
             */
            mMapView.OnResume();
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.destroy()
             */
            mMapView.Destroy();
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
    }
}