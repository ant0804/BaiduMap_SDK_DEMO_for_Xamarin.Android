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
using Com.Baidu.Location;
using Android.Content.PM;

namespace baidumapsdk.demo
{
    /**
     * ��demo����չʾ��ν�϶�λSDKʵ�ֶ�λ����ʹ��MyLocationOverlay���ƶ�λλ��
     * ͬʱչʾ���ʹ���Զ���ͼ����Ʋ����ʱ��������
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_location", ScreenOrientation = ScreenOrientation.Sensor)]
    public class LocationOverlayDemo : Activity
    {
        private enum E_BUTTON_TYPE
        {
            LOC,
            COMPASS,
            FOLLOW
        }

        private E_BUTTON_TYPE mCurBtnType;

        // ��λ���
        LocationClient mLocClient;
        LocationData locData = null;
        public MyLocationListenner myListener;// = new MyLocationListenner(this);

        //��λͼ��
        LocationOverlay myLocationOverlay = null;
        //��������ͼ��
        private PopupOverlay pop = null;//��������ͼ�㣬����ڵ�ʱʹ��
        private TextView popupText = null;//����view
        private View viewCache = null;

        //��ͼ��أ�ʹ�ü̳�MapView��MyLocationMapViewĿ������дtouch�¼�ʵ�����ݴ���
        //���������touch�¼���������̳У�ֱ��ʹ��MapView����
        MyLocationMapView mMapView = null;	// ��ͼView
        private MapController mMapController = null;

        //UI���
        Android.Widget.RadioGroup.IOnCheckedChangeListener radioButtonListener = null;
        Button requestLocButton = null;
        bool isRequest = false;//�Ƿ��ֶ���������λ
        bool isFirstLoc = true;//�Ƿ��״ζ�λ

        class BtnClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            LocationOverlayDemo locationOverlayDemo;

            public BtnClickListenerImpl(LocationOverlayDemo locationOverlayDemo)
            {
                this.locationOverlayDemo = locationOverlayDemo;
            }

            public void OnClick(View v)
            {
                switch (locationOverlayDemo.mCurBtnType)
                {
                    case E_BUTTON_TYPE.LOC:
                        //�ֶ���λ����
                        locationOverlayDemo.RequestLocClick();
                        break;
                    case E_BUTTON_TYPE.COMPASS:
                        locationOverlayDemo.myLocationOverlay.SetLocationMode(Com.Baidu.Mapapi.Map.MyLocationOverlay.LocationMode.Normal);
                        locationOverlayDemo.requestLocButton.Text = "��λ";
                        locationOverlayDemo.mCurBtnType = E_BUTTON_TYPE.LOC;
                        break;
                    case E_BUTTON_TYPE.FOLLOW:
                        locationOverlayDemo.myLocationOverlay.SetLocationMode(Com.Baidu.Mapapi.Map.MyLocationOverlay.LocationMode.Compass);
                        locationOverlayDemo.requestLocButton.Text = "����";
                        locationOverlayDemo.mCurBtnType = E_BUTTON_TYPE.COMPASS;
                        break;
                }
            }
        }

        class RadioButtonListenerImpl : Java.Lang.Object, Android.Widget.RadioGroup.IOnCheckedChangeListener
        {
            LocationOverlayDemo locationOverlayDemo;

            public RadioButtonListenerImpl(LocationOverlayDemo locationOverlayDemo)
            {
                this.locationOverlayDemo = locationOverlayDemo;
            }

            public void OnCheckedChanged(RadioGroup group, int checkedId)
            {
                if (checkedId == Resource.Id.defaulticon)
                {
                    //����null�򣬻ָ�Ĭ��ͼ��
                    locationOverlayDemo.ModifyLocationOverlayIcon(null);
                }
                if (checkedId == Resource.Id.customicon)
                {
                    //�޸�Ϊ�Զ���marker
                    locationOverlayDemo.ModifyLocationOverlayIcon(locationOverlayDemo.Resources.GetDrawable(Resource.Drawable.icon_geo));
                }
            }
        }

        class PopListenerImpl : Java.Lang.Object, IPopupClickListener
        {
            LocationOverlayDemo locationOverlayDemo;

            public PopListenerImpl(LocationOverlayDemo locationOverlayDemo)
            {
                this.locationOverlayDemo = locationOverlayDemo;
            }

            public void OnClickedPopup(int index)
            {
                Log.Verbose("click", "clickapoapo");
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
            SetContentView(Resource.Layout.activity_locationoverlay);
            ICharSequence titleLable = new String("��λ����");
            Title = titleLable.ToString();

            myListener = new MyLocationListenner(this);

            requestLocButton = FindViewById<Button>(Resource.Id.button1);
            mCurBtnType = E_BUTTON_TYPE.LOC;
            Android.Views.View.IOnClickListener btnClickListener = new BtnClickListenerImpl(this);

            requestLocButton.SetOnClickListener(btnClickListener);

            RadioGroup group = this.FindViewById<RadioGroup>(Resource.Id.radioGroup);
            radioButtonListener = new RadioButtonListenerImpl(this);
            group.SetOnCheckedChangeListener(radioButtonListener);

            //��ͼ��ʼ��
            mMapView = FindViewById<MyLocationMapView>(Resource.Id.bmapView);
            mMapController = mMapView.Controller;
            mMapView.Controller.SetZoom(14);
            mMapView.Controller.EnableClick(true);
            mMapView.SetBuiltInZoomControls(true);
            //���� ��������ͼ��
            CreatePaopao();

            //��λ��ʼ��
            mLocClient = new LocationClient(this);
            locData = new LocationData();
            mLocClient.RegisterLocationListener(myListener);
            LocationClientOption option = new LocationClientOption();
            option.OpenGps = true;//��gps
            option.CoorType = "bd09ll";     //������������
            option.ScanSpan = 1000;
            mLocClient.LocOption = option;
            mLocClient.Start();

            //��λͼ���ʼ��
            myLocationOverlay = new LocationOverlay(this, mMapView);
            //���ö�λ����
            myLocationOverlay.SetData(locData);
            //��Ӷ�λͼ��
            mMapView.Overlays.Add(myLocationOverlay);
            myLocationOverlay.EnableCompass();
            //�޸Ķ�λ���ݺ�ˢ��ͼ����Ч
            mMapView.Refresh();

        }
        /**
         * �ֶ�����һ�ζ�λ����
         */
        public void RequestLocClick()
        {
            isRequest = true;
            mLocClient.RequestLocation();
            Toast.MakeText(this, "���ڶ�λ����", ToastLength.Short).Show();
        }
        /**
         * �޸�λ��ͼ��
         * @param marker
         */
        public void ModifyLocationOverlayIcon(Drawable marker)
        {
            //������markerΪnullʱ��ʹ��Ĭ��ͼ�����
            myLocationOverlay.SetMarker(marker);
            //�޸�ͼ�㣬��Ҫˢ��MapView��Ч
            mMapView.Refresh();
        }
        /**
	     * ������������ͼ��
	     */
        public void CreatePaopao()
        {
            viewCache = LayoutInflater.Inflate(Resource.Layout.custom_text_view, null);
            popupText = viewCache.FindViewById<TextView>(Resource.Id.textcache);
            //���ݵ����Ӧ�ص�
            IPopupClickListener popListener = new PopListenerImpl(this);
            pop = new PopupOverlay(mMapView, popListener);
            MyLocationMapView.pop = pop;
        }
        /**
         * ��λSDK��������
         */
        public class MyLocationListenner : Java.Lang.Object, IBDLocationListener
        {

            LocationOverlayDemo locationOverlayDemo;

            public MyLocationListenner(LocationOverlayDemo locationOverlayDemo)
            {
                this.locationOverlayDemo = locationOverlayDemo;
            }

            public void OnReceiveLocation(BDLocation location)
            {
                if (location == null)
                    return;

                locationOverlayDemo.locData.Latitude = location.Latitude;
                locationOverlayDemo.locData.Longitude = location.Longitude;
                //�������ʾ��λ����Ȧ����accuracy��ֵΪ0����
                locationOverlayDemo.locData.Accuracy = location.Radius;
                // �˴��������� locData�ķ�����Ϣ, �����λ SDK δ���ط�����Ϣ���û������Լ�ʵ�����̹�����ӷ�����Ϣ��
                locationOverlayDemo.locData.Direction = location.Derect;
                //���¶�λ����
                locationOverlayDemo.myLocationOverlay.SetData(locationOverlayDemo.locData);
                //����ͼ������ִ��ˢ�º���Ч
                locationOverlayDemo.mMapView.Refresh();
                //���ֶ�����������״ζ�λʱ���ƶ�����λ��
                if (locationOverlayDemo.isRequest || locationOverlayDemo.isFirstLoc)
                {
                    //�ƶ���ͼ����λ��
                    Log.Debug("LocationOverlay", "receive location, animate to it");
                    locationOverlayDemo.mMapController.AnimateTo(new GeoPoint((int)(locationOverlayDemo.locData.Latitude * 1e6), (int)(locationOverlayDemo.locData.Longitude * 1e6)));
                    locationOverlayDemo.isRequest = false;
                    locationOverlayDemo.myLocationOverlay.SetLocationMode(Com.Baidu.Mapapi.Map.MyLocationOverlay.LocationMode.Following);
                    locationOverlayDemo.requestLocButton.Text = "����";
                    locationOverlayDemo.mCurBtnType = E_BUTTON_TYPE.FOLLOW;
                }
                //�״ζ�λ���
                locationOverlayDemo.isFirstLoc = false;
            }

            public void OnReceivePoi(BDLocation poiLocation)
            {
                if (poiLocation == null)
                {
                    return;
                }
            }
        }

        //�̳�MyLocationOverlay��дdispatchTapʵ�ֵ������
        public class LocationOverlay : MyLocationOverlay
        {


            LocationOverlayDemo locationOverlayDemo;

            public LocationOverlay(LocationOverlayDemo locationOverlayDemo, MapView mapView)
                : base(mapView)
            {
                // TODO Auto-generated constructor stub
                this.locationOverlayDemo = locationOverlayDemo;
            }

            protected override bool DispatchTap()
            {
                // TODO Auto-generated method stub
                //�������¼�,��������
                locationOverlayDemo.popupText.SetBackgroundResource(Resource.Drawable.popup);
                locationOverlayDemo.popupText.Text = "�ҵ�λ��";
                locationOverlayDemo.pop.ShowPopup(BMapUtil.GetBitmapFromView(locationOverlayDemo.popupText),
                        new GeoPoint((int)(locationOverlayDemo.locData.Latitude * 1e6), (int)(locationOverlayDemo.locData.Longitude * 1e6)),
                        8);
                return true;
            }

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
            //�˳�ʱ���ٶ�λ
            if (mLocClient != null)
                mLocClient.Stop();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //        getMenuInflater().inflate(R.menu.activity_main, menu);
            return true;
        }
    }

    /**
     * �̳�MapView��дonTouchEventʵ�����ݴ������
     * @author hejin
     *
     */
    class MyLocationMapView : MapView
    {
        internal static PopupOverlay pop = null;//��������ͼ�㣬���ͼ��ʹ��
        public MyLocationMapView(Context context)
            : base(context)
        {

            // TODO Auto-generated constructor stub
        }
        public MyLocationMapView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }
        public MyLocationMapView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }
        public override bool OnTouchEvent(MotionEvent eventX)
        {
            if (!base.OnTouchEvent(eventX))
            {
                //��������
                if (pop != null && eventX.Action == MotionEventActions.Up)
                    pop.HidePop();
            }
            return true;
        }
    }
}