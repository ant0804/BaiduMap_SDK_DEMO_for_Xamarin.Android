using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using baidumapsdk.demo;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Platform.Comapi.Basestruct;
using Java.IO;
using Java.Lang;
using System.IO;

namespace baidumapsdk.demo
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_ui", ScreenOrientation = ScreenOrientation.Sensor)]
    public class UISettingDemo : Activity
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
            SetContentView(Resource.Layout.activity_uisetting);
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
             * ���õ�ͼ����
             */
            mMapController.SetOverlooking(-30);
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
         * �Ƿ�������������
         * @param v
         */
        [Java.Interop.Export]
        public void SetZoomEnable(View v)
        {
            mMapController.ZoomGesturesEnabled = ((CheckBox)v).Checked;
        }
        /**
         * �Ƿ�����ƽ������
         * @param v
         */
        [Java.Interop.Export]
        public void SetScrollEnable(View v)
        {
            mMapController.ScrollGesturesEnabled = ((CheckBox)v).Checked;
        }
        /**
         * �Ƿ�����˫���Ŵ�
         * @param v
         */
        [Java.Interop.Export]
        public void SetDoubleClickEnable(View v)
        {
            mMapView.DoubleClickZooming = ((CheckBox)v).Checked;
        }
        /**
         * �Ƿ�������ת����
         * @param v
         */
        [Java.Interop.Export]
        public void SetRotateEnable(View v)
        {
            mMapController.RotationGesturesEnabled = ((CheckBox)v).Checked;
        }
        /**
         * �Ƿ����ø�������
         * @param v
         */
        [Java.Interop.Export]
        public void SetOverlookEnable(View v)
        {
            mMapController.OverlookingGesturesEnabled = ((CheckBox)v).Checked;
        }
        /**
         * �Ƿ���ʾ�������ſؼ�
         * @param v
         */
        [Java.Interop.Export]
        public void SetBuiltInZoomControllEnable(View v)
        {
            mMapView.SetBuiltInZoomControls(((CheckBox)v).Checked);
        }

        /**
         * ����ָ����λ��,ָ������3Dģʽ���Զ�����
         * @param view
         */
        [Java.Interop.Export]
        public void SetCompassLocation(View view)
        {
            bool checkedx = ((RadioButton)view).Checked;
            switch (view.Id)
            {
                case Resource.Id.lefttop:
                    if (checkedx)
                        //����ָ������ʾ�����Ͻ�
                        mMapController.SetCompassMargin(100, 100);
                    break;
                case Resource.Id.righttop:
                    if (checkedx)
                        mMapController.SetCompassMargin(mMapView.Width - 100, 100);
                    break;
            }
        }

        protected override void OnPause()
        {
            /**
             *  MapView������������Activityͬ������activity����ʱ�����MapView.OnPause()
             */
            mMapView.OnPause();
            base.OnPause();
        }

        protected override void OnResume()
        {
            /**
             *  MapView������������Activityͬ������activity�ָ�ʱ�����MapView.OnResume()
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