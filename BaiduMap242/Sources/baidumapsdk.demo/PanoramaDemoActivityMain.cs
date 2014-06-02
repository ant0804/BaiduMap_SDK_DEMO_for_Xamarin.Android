
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
using Android.Support.V4.App;
using Android.Content.Res;
using Android.App;
using Com.Baidu.Mapapi.PanoramaX;
using Android.Graphics;

namespace baidumapsdk.demo
{
    /**
     * ȫ��Demo��Activity
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_panorama", ScreenOrientation = ScreenOrientation.Sensor)]
    public class PanoramaDemoActivityMain : Activity, IPanoramaViewListener
    {
#pragma warning disable 0169, 0414
        private static readonly string LTAG = typeof(PanoramaDemoActivityMain).Name;
        private PanoramaView mPanoramaView;
        private PanoramaService mService;
        private Com.Baidu.Mapapi.PanoramaX.PanoramaService.IPanoramaServiceCallback mCallback;
        private MyOverlay mOverlay = null;
        private int mSrcType = -1;
        private Button mBtn = null;
        private bool isShowOverlay = true;
        private ProgressDialog pd;
        private TextView mRoadName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //�ȳ�ʼ��BMapManager
            DemoApplication app = (DemoApplication)this.Application;
            if (app.mBMapManager == null)
            {
                app.mBMapManager = new BMapManager(this);

                app.mBMapManager.Init(
                        new DemoApplication.MyGeneralListener());
            }
            SetContentView(Resource.Layout.activity_panorama_main);
            mPanoramaView = FindViewById<PanoramaView>(Resource.Id.panorama);
            //UI��ʼ��
            mBtn = FindViewById<Button>(Resource.Id.button);
            mBtn.Visibility = ViewStates.Invisible;
            //��·��
            mRoadName = FindViewById<TextView>(Resource.Id.road);
            mRoadName.Visibility = ViewStates.Visible;
            mRoadName.Text = "�ٶ�ȫ��";
            mRoadName.SetBackgroundColor(Color.Argb(200, 5, 5, 5));  //����͸����
            mRoadName.SetTextColor(Color.Argb(255, 250, 250, 250));  //����͸����
            mRoadName.TextSize = 22;
            //��ת������
            pd = new ProgressDialog(this);
            pd.SetMessage("��ת�С���");
            pd.SetCancelable(true);//���ý������Ƿ���԰��˻ؼ�ȡ�� 

            //��ʼ��Searvice
            mService = PanoramaService.GetInstance(ApplicationContext);
            mCallback = new IPanoramaServiceCallbackImpl(this);
            //����ȫ��ͼ����
            mPanoramaView.SetPanoramaViewListener(this);

            //��������
            ParseInput();
        }

        class IPanoramaServiceCallbackImpl : Java.Lang.Object, Com.Baidu.Mapapi.PanoramaX.PanoramaService.IPanoramaServiceCallback
        {
            PanoramaDemoActivityMain panoramaDemoActivityMain;
            public IPanoramaServiceCallbackImpl(PanoramaDemoActivityMain panoramaDemoActivityMain)
            {
                this.panoramaDemoActivityMain = panoramaDemoActivityMain;
            }

            public void OnGetPanorama(Panorama p, int error)
            {
                //ʹ��pid����ʱ��ӱ�ע
                if (error != 0)
                {
                    Toast.MakeText(panoramaDemoActivityMain,
                            "��Ǹ��δ�ܼ�����ȫ������", ToastLength.Long).Show();
                }
                if (p != null)
                {
                    panoramaDemoActivityMain.mPanoramaView.Panorama = p;
                    panoramaDemoActivityMain.mRoadName.Text = p.StreetName;
                }
            }
        }

        private void ParseInput()
        {
            Intent intent = Intent;
            Bundle b = intent.Extras;
            if (intent.HasExtra("uid"))
            {
                mSrcType = 1;
                mService.RequestPanoramaByPoi(b.GetString("uid"), mCallback);
                return;
            }
            if (intent.HasExtra("lat") && intent.HasExtra("lon"))
            {
                mSrcType = 2;
                mService.RequestPanoramaByGeoPoint(new GeoPoint(b.GetInt("lat"), b.GetInt("lon")), mCallback);
                return;
            }
            if (intent.HasExtra("pid"))
            {
                mSrcType = 3;
                mService.RequestPanoramaById(b.GetString("pid"), mCallback);

            }
        }

        //����button���
        [Java.Interop.Export]
        public void OnButtonClick(View v)
        {
            if (isShowOverlay)
            {
                AddOverlay();
                mBtn.Text = "ɾ����ע";
            }
            else
            {
                RemoveOverlay();
                mBtn.Text = "��ӱ�ע";
            }
            isShowOverlay = !isShowOverlay;

        }

        //��ӱ�ע
        private void AddOverlay()
        {
            //�찲������
            GeoPoint p = new GeoPoint(39914195, 116403928);
            mOverlay = new MyOverlay(this, Resources.GetDrawable(Resource.Drawable.icon_marka),
                    mPanoramaView);
            PanoramaMarker item = new PanoramaMarker(p);
            item.Marker = Resources.GetDrawable(Resource.Drawable.icon_marka);
            mOverlay.AddMarker(item);
            mPanoramaView.Overlays.Add(mOverlay);
            mPanoramaView.Refresh();
        }

        //ɾ����ע
        private void RemoveOverlay()
        {
            if (mOverlay != null)
            {
                mPanoramaView.Overlays.Remove(mOverlay);
                mPanoramaView.Refresh();
            }
        }


        protected override void OnPause()
        {
            base.OnPause();
            mPanoramaView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            mPanoramaView.OnResume();
            if (mSrcType == 3)
            {
                mBtn.Visibility = ViewStates.Visible;
                (FindViewById<ViewGroup>(Resource.Id.layout))
                        .BringChildToFront(mBtn);
            }
        }

        protected override void OnDestroy()
        {
            mPanoramaView.Destroy();
            mService.Destroy();
            base.OnDestroy();
        }

        public class MyOverlay : PanoramaOverlay
        {

            PanoramaDemoActivityMain panoramaDemoActivityMain;

            public MyOverlay(PanoramaDemoActivityMain panoramaDemoActivityMain, Drawable defaultMarker, PanoramaView mapView) :
                base(defaultMarker, mapView)
            {
                this.panoramaDemoActivityMain = panoramaDemoActivityMain;
            }

            protected override bool OnTap(int index)
            {
                Toast.MakeText(panoramaDemoActivityMain,
                        "��ע�ѱ����", ToastLength.Short).Show();
                return true;
            }

        }

        public void BeforeMoveToPanorama(string pId)
        {
            // TODO Auto-generated method stub
            //����������
            pd.Show();
        }

        public void AfterMovetoPanorama(string pId)
        {
            // TODO Auto-generated method stub
            //���ؽ�����
            pd.Dismiss();
        }

        public void OnPanoramaCameraChange(PanoramaViewCamera camera)
        {
            // TODO Auto-generated method stub

        }

        public void OnClickPanoramaLink(PanoramaLink link)
        {
            // TODO Auto-generated method stub

        }

        public void OnPanoramaMoveStart()
        {
            // TODO Auto-generated method stub

        }

        public void OnPanoramaMoveFinish()
        {
            // TODO Auto-generated method stub

        }

        public void OnPanoramaAnimationStart()
        {
            // TODO Auto-generated method stub

        }

        public void OnPanoramaAnimationEnd()
        {
            // TODO Auto-generated method stub

        }

    }
}