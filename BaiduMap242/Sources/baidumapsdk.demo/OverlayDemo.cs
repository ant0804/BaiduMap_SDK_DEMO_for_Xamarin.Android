
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
using Android.App;
using Android.Graphics;

namespace baidumapsdk.demo
{
    /**
     * ��ʾ��������÷�
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_overlay", ScreenOrientation = ScreenOrientation.Sensor)]
    public class OverlayDemo : Activity
    {
        /**
	     *  MapView �ǵ�ͼ���ؼ�
	     */
        private MapView mMapView = null;
        private Button mClearBtn;
        private Button mResetBtn;
        /**
         *  ��MapController��ɵ�ͼ���� 
         */
        private MapController mMapController = null;
        private MyOverlay mOverlay = null;
        private PopupOverlay pop = null;
        private List<OverlayItem> mItems = null;
        private TextView popupText = null;
        private View viewCache = null;
        private View popupInfo = null;
        private View popupLeft = null;
        private View popupRight = null;
        private Button button = null;
        private MapView.LayoutParams layoutParam = null;
        private OverlayItem mCurItem = null;
        /**
         * overlay λ������
         */
        double mLon1 = 116.400244;
        double mLat1 = 39.963175;
        double mLon2 = 116.369199;
        double mLat2 = 39.942821;
        double mLon3 = 116.425541;
        double mLat3 = 39.939723;
        double mLon4 = 116.401394;
        double mLat4 = 39.906965;

        // ground overlay
        private GroundOverlay mGroundOverlay;
        private Ground mGround;
        private double mLon5 = 116.380338;
        private double mLat5 = 39.92235;
        private double mLon6 = 116.414977;
        private double mLat6 = 39.947246;

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
            SetContentView(Resource.Layout.activity_overlay);
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mClearBtn = FindViewById<Button>(Resource.Id.clear);
            mResetBtn = FindViewById<Button>(Resource.Id.reset);
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
            mMapController.SetZoom(14);
            /**
             * ��ʾ�������ſؼ�
             */
            mMapView.SetBuiltInZoomControls(true);

            InitOverlay();

            /**
             * �趨��ͼ���ĵ�
             */
            GeoPoint p = new GeoPoint((int)(39.933859 * 1E6), (int)(116.400191 * 1E6));
            mMapController.SetCenter(p);
        }


        public void InitOverlay()
        {
            /**
             * �����Զ���overlay
             */
            mOverlay = new MyOverlay(this, Resources.GetDrawable(Resource.Drawable.icon_marka), mMapView);
            /**
             * ׼��overlay ����
             */
            GeoPoint p1 = new GeoPoint((int)(mLat1 * 1E6), (int)(mLon1 * 1E6));
            OverlayItem item1 = new OverlayItem(p1, "������1", "");
            /**
             * ����overlayͼ�꣬�粻���ã���ʹ�ô���ItemizedOverlayʱ��Ĭ��ͼ��.
             */
            item1.Marker = Resources.GetDrawable(Resource.Drawable.icon_marka);

            GeoPoint p2 = new GeoPoint((int)(mLat2 * 1E6), (int)(mLon2 * 1E6));
            OverlayItem item2 = new OverlayItem(p2, "������2", "");
            item2.Marker = Resources.GetDrawable(Resource.Drawable.icon_markb);

            GeoPoint p3 = new GeoPoint((int)(mLat3 * 1E6), (int)(mLon3 * 1E6));
            OverlayItem item3 = new OverlayItem(p3, "������3", "");
            item3.Marker = Resources.GetDrawable(Resource.Drawable.icon_markc);

            GeoPoint p4 = new GeoPoint((int)(mLat4 * 1E6), (int)(mLon4 * 1E6));
            OverlayItem item4 = new OverlayItem(p4, "������4", "");
            item4.Marker = Resources.GetDrawable(Resource.Drawable.icon_gcoding);
            /**
             * ��item ��ӵ�overlay��
             * ע�⣺ ͬһ��itmeֻ��addһ��
             */
            mOverlay.AddItem(item1);
            mOverlay.AddItem(item2);
            mOverlay.AddItem(item3);
            mOverlay.AddItem(item4);
            /**
             * ��������item���Ա�overlay��reset���������
             */
            mItems = new List<OverlayItem>();
            mItems.AddRange(mOverlay.AllItem);

            // ��ʼ�� ground ͼ��
            mGroundOverlay = new GroundOverlay(mMapView);
            GeoPoint leftBottom = new GeoPoint((int)(mLat5 * 1E6),
                    (int)(mLon5 * 1E6));
            GeoPoint rightTop = new GeoPoint((int)(mLat6 * 1E6),
                    (int)(mLon6 * 1E6));
            Drawable d = Resources.GetDrawable(Resource.Drawable.ground_overlay);
            Bitmap bitmap = ((BitmapDrawable)d).Bitmap;
            mGround = new Ground(bitmap, leftBottom, rightTop);

            /**
             * ��overlay �����MapView��
             */
            mMapView.Overlays.Add(mOverlay);
            mMapView.Overlays.Add(mGroundOverlay);
            mGroundOverlay.AddGround(mGround);
            /**
             * ˢ�µ�ͼ
             */
            mMapView.Refresh();
            mResetBtn.Enabled = false;
            mClearBtn.Enabled = true;
            /**
             * ���ͼ����Զ���View.
             */
            viewCache = LayoutInflater.Inflate(Resource.Layout.custom_text_view, null);
            popupInfo = viewCache.FindViewById<View>(Resource.Id.popinfo);
            popupLeft = viewCache.FindViewById<View>(Resource.Id.popleft);
            popupRight = viewCache.FindViewById<View>(Resource.Id.popright);
            popupText = viewCache.FindViewById<TextView>(Resource.Id.textcache);

            button = new Button(this);
            button.SetBackgroundResource(Resource.Drawable.popup);

            /**
             * ����һ��popupoverlay
             */
            IPopupClickListener popListener = new IPopupClickListenerImpl(this);
            pop = new PopupOverlay(mMapView, popListener);
        }

        class IPopupClickListenerImpl : Java.Lang.Object, IPopupClickListener
        {

            OverlayDemo overlayDemo;
            public IPopupClickListenerImpl(OverlayDemo overlayDemo)
            {
                this.overlayDemo = overlayDemo;
            }

            public void OnClickedPopup(int index)
            {
                if (index == 0)
                {
                    //����itemλ��
                    overlayDemo.pop.HidePop();
                    GeoPoint p = new GeoPoint(overlayDemo.mCurItem.Point.LatitudeE6 + 5000,
                            overlayDemo.mCurItem.Point.LongitudeE6 + 5000);
                    overlayDemo.mCurItem.SetGeoPoint(p);
                    overlayDemo.mOverlay.UpdateItem(overlayDemo.mCurItem);
                    overlayDemo.mMapView.Refresh();
                }
                else if (index == 2)
                {
                    //����ͼ��
                    overlayDemo.mCurItem.Marker = overlayDemo.Resources.GetDrawable(Resource.Drawable.nav_turn_via_1);
                    overlayDemo.mOverlay.UpdateItem(overlayDemo.mCurItem);
                    overlayDemo.mMapView.Refresh();
                }
            }
        }

        /**
         * �������Overlay
         * @param view
         */
        [Java.Interop.Export]
        public void ClearOverlay(View view)
        {
            mOverlay.RemoveAll();
            mGroundOverlay.RemoveGround(mGround);
            if (pop != null)
            {
                pop.HidePop();
            }
            mMapView.RemoveView(button);
            mMapView.Refresh();
            mResetBtn.Enabled = true;
            mClearBtn.Enabled = false;
        }
        /**
         * �������Overlay
         * @param view
         */
        [Java.Interop.Export]
        public void ResetOverlay(View view)
        {
            //����add overlay
            mOverlay.AddItem(mItems);
            mGroundOverlay.AddGround(mGround);
            mMapView.Refresh();
            mResetBtn.Enabled = false;
            mClearBtn.Enabled = true;
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

        public class MyOverlay : ItemizedOverlay
        {
            OverlayDemo overlayDemo;
            public MyOverlay(OverlayDemo overlayDemo, Drawable defaultMarker, MapView mapView) :
                base(defaultMarker, mapView)
            {
                this.overlayDemo = overlayDemo;
            }

            protected override bool OnTap(int index)
            {
                OverlayItem item = GetItem(index);
                overlayDemo.mCurItem = item;
                if (index == 3)
                {
                    overlayDemo.button.Text = "����һ��ϵͳ�ؼ�";
                    GeoPoint pt = new GeoPoint((int)(overlayDemo.mLat4 * 1E6),
                            (int)(overlayDemo.mLon4 * 1E6));
                    // �����Զ���View
                    overlayDemo.pop.ShowPopup(overlayDemo.button, pt, 32);
                }
                else
                {
                    overlayDemo.popupText.Text = GetItem(index).Title;
                    Bitmap[] bitMaps ={
				    BMapUtil.GetBitmapFromView(overlayDemo.popupLeft), 		
				    BMapUtil.GetBitmapFromView(overlayDemo.popupInfo), 		
				    BMapUtil.GetBitmapFromView(overlayDemo.popupRight) 		
			    };
                    overlayDemo.pop.ShowPopup(bitMaps, item.Point, 32);
                }
                return true;
            }


            public override bool OnTap(GeoPoint pt, MapView mMapView)
            {
                if (overlayDemo.pop != null)
                {
                    overlayDemo.pop.HidePop();
                    overlayDemo.mMapView.RemoveView(overlayDemo.button);
                }
                return false;
            }
        }
    }
}