using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
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

namespace baidumapsdk.demo
{
    /**
     * ��ʾ��ͼͼ����ʾ�Ŀ��Ʒ���
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_layers", ScreenOrientation = ScreenOrientation.Sensor)]
    public class LayersDemo : Activity
    {
        /**
	     *  MapView �ǵ�ͼ���ؼ�
	     */
        private MapView mMapView = null;
        /**
         *  ��MapController��ɵ�ͼ���� 
         */
        private MapController mMapController = null;

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
            SetContentView(Resource.Layout.activity_layers);

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
             * ��ʾ�������ſؼ�
             */
            mMapView.SetBuiltInZoomControls(true);

            /**
             * ����ͼ�ƶ����찲��
             * ʹ�ðٶȾ�γ�����꣬����ͨ��http://api.map.baidu.com/lbsapi/getpoint/index.html��ѯ��������
             * �����Ҫ�ڰٶȵ�ͼ����ʾʹ����������ϵͳ��λ�ã��뷢�ʼ���mapapi@baidu.com��������ת���ӿ�
             */
            double cLat = 39.945;
            double cLon = 116.404;
            GeoPoint p = new GeoPoint((int)(cLat * 1E6), (int)(cLon * 1E6));
            mMapController.SetCenter(p);
        }

        /**
         * ���õ�ͼ��ʾģʽ
         * @param view
         */
        [Java.Interop.Export]
        public void SetMapMode(View view)
        {
            bool checkedX = ((RadioButton)view).Checked;
            switch (view.Id)
            {
                case Resource.Id.normal:
                    if (checkedX)
                        mMapView.Satellite = false;
                    break;
                case Resource.Id.statellite:
                    if (checkedX)
                        mMapView.Satellite = true;
                    break;
            }
        }
        /**
         * �����Ƿ���ʾ��ͨͼ
         * @param view
         */
        [Java.Interop.Export]
        public void SetTraffic(View view)
        {
            mMapView.Traffic = ((CheckBox)view).Checked;
        }

        protected override void OnPause()
        {
            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.OnPause()
             */
            mMapView.OnPause();
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.Destroy()
             */
            mMapView.Destroy();
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            /**
             *  MapView������������Activityͬ������activity�ָ�ʱ�����MapView.OnResume()
             */
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