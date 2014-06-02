using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
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
    /**
     * ��ʾ��ͼ���ţ���ת���ӽǿ���
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_control", ScreenOrientation = ScreenOrientation.Sensor)]
    public class MapControlDemo : Activity
    {
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
        IMKMapViewListener mMapListener = null;

        /**
         * ���ڽػ�������
         */
        IMKMapTouchListener mapTouchListener = null;

        /**
         * ��ǰ�ص����
         */
        private GeoPoint currentPt = null;

        /**
         * ���ư�ť
         */
        private Button zoomButton = null;
        private Button rotateButton = null;
        private Button overlookButton = null;
        private Button saveScreenButton = null;

        /**
         * 
         */
        private string touchType = null;

        /**
         * ������ʾ��ͼ״̬�����
         */
        private TextView mStateBar = null;

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

            /**
              * ����MapView��setContentView()�г�ʼ��,��������Ҫ��BMapManager��ʼ��֮��
              */
            SetContentView(Resource.Layout.activity_mapcontrol);

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

            mStateBar = FindViewById<TextView>(Resource.Id.state);

            /**
             * ��ʼ����ͼ�¼�����
             */
            InitListener();

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

        private class MKMapTouchListenerImpl : Java.Lang.Object, IMKMapTouchListener
        {
            private MapControlDemo mapControlDemo;

            public MKMapTouchListenerImpl(MapControlDemo mapControlDemo)
            {
                this.mapControlDemo = mapControlDemo;
            }

            public void OnMapClick(GeoPoint point)
            {
                mapControlDemo.touchType = "����";
                mapControlDemo.currentPt = point;
                mapControlDemo.UpdateMapState();
            }

            public void OnMapDoubleClick(GeoPoint point)
            {
                mapControlDemo.touchType = "˫��";
                mapControlDemo.currentPt = point;
                mapControlDemo.UpdateMapState();
            }

            public void OnMapLongClick(GeoPoint point)
            {
                mapControlDemo.touchType = "����";
                mapControlDemo.currentPt = point;
                mapControlDemo.UpdateMapState();
            }
        }

        private class MKMapViewListenerImpl : Java.Lang.Object, IMKMapViewListener
        {
            private MapControlDemo mapControlDemo;

            public MKMapViewListenerImpl(MapControlDemo mapControlDemo)
            {
                this.mapControlDemo = mapControlDemo;
            }

            public void OnMapMoveFinish()
            {
                /**
                 * �ڴ˴����ͼ�ƶ���ɻص�
                 * ���ţ�ƽ�ƵȲ�����ɺ󣬴˻ص�������
                 */
                mapControlDemo.UpdateMapState();
            }

            public void OnClickMapPoi(MapPoi mapPoiInfo)
            {
                /**
                 * �ڴ˴����ͼpoi����¼�
                 * ��ʾ��ͼpoi���Ʋ��ƶ����õ�
                 * ���ù��� mMapController.enableClick(true); ʱ���˻ص����ܱ�����
                 * 
                 */
            }

            public void OnGetCurrentMap(Bitmap b)
            {
                /**
                 *  �����ù� mMapView.getCurrentMap()�󣬴˻ص��ᱻ����
                 *  ���ڴ˱����ͼ���洢�豸
                 */
                string filePath = "/mnt/sdcard/test.png";// File file = new File("/mnt/sdcard/test.png");
                System.IO.FileStream fileOutputStream;

                try
                {
                    fileOutputStream = new System.IO.FileStream(filePath, FileMode.Create);

                    if (b.Compress(Bitmap.CompressFormat.Png, 70, fileOutputStream))
                    {
                        fileOutputStream.Flush();
                        fileOutputStream.Close();
                    }

                    Toast.MakeText(mapControlDemo, "��Ļ��ͼ�ɹ���ͼƬ����: " + filePath.ToString(), ToastLength.Short).Show();
                }
                catch (System.IO.FileNotFoundException e)
                {
                    Log.Error("imknown", e.StackTrace);
                }
                catch (System.IO.IOException e)
                {
                    Log.Error("imknown", e.StackTrace);
                }
            }

            public void OnMapAnimationFinish()
            {
                /**
                 *  ��ͼ��ɴ������Ĳ�������: animationTo()���󣬴˻ص�������
                 */
                mapControlDemo.UpdateMapState();
            }

            public void OnMapLoadFinish()
            {
                // TODO Auto-generated method stub
            }
        }

        private void InitListener()
        {
            /**
             * ���õ�ͼ����¼����� 
             */
            mapTouchListener = new MKMapTouchListenerImpl(this);

            mMapView.RegMapTouchListner(mapTouchListener);

            /**
             * ���õ�ͼ�¼�����
             */
            mMapListener = new MKMapViewListenerImpl(this);

            mMapView.RegMapViewListener(DemoApplication.getInstance().mBMapManager, mMapListener);

            /**
             * ���ð�������
             */
            zoomButton = FindViewById<Button>(Resource.Id.zoombutton);
            rotateButton = FindViewById<Button>(Resource.Id.rotatebutton);
            overlookButton = FindViewById<Button>(Resource.Id.overlookbutton);
            saveScreenButton = FindViewById<Button>(Resource.Id.savescreen);

            Android.Views.View.IOnClickListener onClickListener = new OnClickListenerImpl(this);

            zoomButton.SetOnClickListener(onClickListener);
            rotateButton.SetOnClickListener(onClickListener);
            overlookButton.SetOnClickListener(onClickListener);
            saveScreenButton.SetOnClickListener(onClickListener);
        }

        private class OnClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            MapControlDemo mapControlDemo;

            public OnClickListenerImpl(MapControlDemo mapControlDemo)
            {
                this.mapControlDemo = mapControlDemo;
            }

            public void OnClick(Android.Views.View view)
            {
                if (view.Equals(mapControlDemo.zoomButton))
                {
                    mapControlDemo.PerfomZoom();
                }
                else if (view.Equals(mapControlDemo.rotateButton))
                {
                    mapControlDemo.PerfomRotate();
                }
                else if (view.Equals(mapControlDemo.overlookButton))
                {
                    mapControlDemo.PerfomOverlook();
                }
                else if (view.Equals(mapControlDemo.saveScreenButton))
                {
                    //��ͼ����MKMapViewListener�б���ͼƬ
                    bool x = mapControlDemo.mMapView.CurrentMap;
                    Toast.MakeText(mapControlDemo, "���ڽ�ȡ��ĻͼƬ...", ToastLength.Short).Show();
                }

                mapControlDemo.UpdateMapState();
            }
        }

        /**
         * ��������
         * sdk ���ż���Χ�� [3.0,19.0]
         */
        private void PerfomZoom()
        {
            EditText t = FindViewById<EditText>(Resource.Id.zoomlevel);
            try
            {
                float zoomLevel = Float.ParseFloat(t.Text.ToString());
                mMapController.SetZoom(zoomLevel);
            }
            catch (NumberFormatException e)
            {
                Toast.MakeText(this, "��������ȷ�����ż���", ToastLength.Short).Show();
                e.PrintStackTrace();
            }
        }

        /**
         * ������ת 
         * ��ת�Ƿ�Χ�� -180 ~ 180 , ��λ����   ��ʱ����ת  
         */
        private void PerfomRotate()
        {
            EditText t = FindViewById<EditText>(Resource.Id.rotateangle);

            try
            {
                int rotateAngle = Integer.ParseInt(t.Text.ToString());
                mMapController.SetRotation(rotateAngle);
            }
            catch (NumberFormatException e)
            {
                Toast.MakeText(this, "��������ȷ����ת�Ƕ�", ToastLength.Short).Show();
                e.PrintStackTrace();
            }
        }

        /**
         * ������
         * ���Ƿ�Χ��  -45 ~ 0 , ��λ�� ��
         */
        private void PerfomOverlook()
        {
            EditText t = FindViewById<EditText>(Resource.Id.overlookangle);

            try
            {
                int overlookAngle = Integer.ParseInt(t.Text.ToString());
                mMapController.SetOverlooking(overlookAngle);
            }
            catch (NumberFormatException e)
            {
                Toast.MakeText(this, "��������ȷ�ĸ���", ToastLength.Short).Show();
                e.PrintStackTrace();
            }
        }

        /**
         * ���µ�ͼ״̬��ʾ���
         */
        private void UpdateMapState()
        {
            if (mStateBar == null)
            {
                return;
            }

            string state = "";

            if (currentPt == null)
            {
                state = "�����������˫����ͼ�Ի�ȡ��γ�Ⱥ͵�ͼ״̬";
            }
            else
            {
                state = String.Format(touchType + ",��ǰ���� �� %f ��ǰγ�ȣ�%f", currentPt.LongitudeE6 * 1E-6, currentPt.LatitudeE6 * 1E-6);
            }

            state += "\n";
            state += String.Format("zoom level= %.1f    rotate angle= %d   overlaylook angle=  %d",
                mMapView.ZoomLevel,
                mMapView.MapRotation,
                mMapView.MapOverlooking
            );

            mStateBar.SetText(state, Android.Widget.TextView.BufferType.Normal);
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