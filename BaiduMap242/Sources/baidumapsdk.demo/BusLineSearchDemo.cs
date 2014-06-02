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
using Com.Baidu.Mapapi.Search;
using Com.Baidu.Platform.Comapi.Basestruct;
using Java.Lang;
using System.Collections;
using System.Collections.Generic;

namespace baidumapsdk.demo
{
    /**
     * ��demo����չʾ��ν��й�����·�����������ʹ��RouteOverlay�ڵ�ͼ�ϻ���
     * ͬʱչʾ������·�߽ڵ㲢��������
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_bus", ScreenOrientation = ScreenOrientation.Sensor)]
    public class BusLineSearchDemo : Activity
    {
        //UI���
        Button mBtnSearch = null;
        Button mBtnNextLine = null;
        //���·�߽ڵ����
        Button mBtnPre = null;//��һ���ڵ�
        Button mBtnNext = null;//��һ���ڵ�
        int nodeIndex = -2;//�ڵ�����,������ڵ�ʱʹ��
        MKRoute route = null;//����ݳ�/����·�����ݵı�����������ڵ�ʱʹ��
        private PopupOverlay pop = null;//��������ͼ�㣬����ڵ�ʱʹ��
        private TextView popupText = null;//����view
        private View viewCache = null;
        private List<string> busLineIDList = null;
        int busLineIndex = 0;

        //��ͼ��أ�ʹ�ü̳�MapView��MyBusLineMapViewĿ������дtouch�¼�ʵ�����ݴ���
        //���������touch�¼���������̳У�ֱ��ʹ��MapView����
        MapView mMapView = null;	// ��ͼView	
        //�������
        MKSearch mSearch = null;	// ����ģ�飬Ҳ��ȥ����ͼģ�����ʹ��

        class MKMapTouchListenerImpl : Java.Lang.Object, IMKMapTouchListener
        {
            BusLineSearchDemo busLineSearchDemo;

            public MKMapTouchListenerImpl(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnMapClick(GeoPoint point)
            {
                //�ڴ˴����ͼ����¼� 
                //����pop
                if (busLineSearchDemo.pop != null)
                {
                    busLineSearchDemo.pop.HidePop();
                }
            }


            public void OnMapDoubleClick(GeoPoint point)
            {

            }


            public void OnMapLongClick(GeoPoint point)
            {

            }
        }

        class MKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {
            private BusLineSearchDemo busLineSearchDemo;

            public MKSearchListenerImpl(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnGetPoiDetailSearchResult(int type, int error)
            {
            }

            public void OnGetPoiResult(MKPoiResult res, int type, int error)
            {
                // ����ſɲο�MKEvent�еĶ���
                if (error != 0 || res == null)
                {
                    Toast.MakeText(busLineSearchDemo, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }

                // �ҵ�����·��poi node
                MKPoiInfo curPoi = null;
                int totalPoiNum = res.CurrentNumPois;
                //��������poi���ҵ�����Ϊ������·��poi
                busLineSearchDemo.busLineIDList.Clear();
                for (int idx = 0; idx < totalPoiNum; idx++)
                {
                    if (2 == res.GetPoi(idx).EPoiType)
                    {
                        // poi���ͣ�0����ͨ�㣬1������վ��2��������·��3������վ��4��������·
                        curPoi = res.GetPoi(idx);
                        //ʹ��poi��uid���𹫽��������
                        busLineSearchDemo.busLineIDList.Add(curPoi.Uid);
                        JavaSystem.Out.Println(curPoi.Uid);

                    }
                }
                busLineSearchDemo.SearchNextBusline();

                // û���ҵ�������Ϣ
                if (curPoi == null)
                {
                    Toast.MakeText(busLineSearchDemo, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }
                busLineSearchDemo.route = null;
            }
            public void OnGetDrivingRouteResult(MKDrivingRouteResult res,
                    int error)
            {
            }
            public void OnGetTransitRouteResult(MKTransitRouteResult res,
                    int error)
            {
            }
            public void OnGetWalkingRouteResult(MKWalkingRouteResult res,
                    int error)
            {
            }
            public void OnGetAddrResult(MKAddrInfo res, int error)
            {
            }
            /**
                * ��ȡ����·�߽����չʾ������·
                */
            public void OnGetBusDetailResult(MKBusLineResult result, int iError)
            {
                if (iError != 0 || result == null)
                {
                    Toast.MakeText(busLineSearchDemo, "��Ǹ��δ�ҵ����", ToastLength.Long).Show();
                    return;
                }

                RouteOverlay routeOverlay = new RouteOverlay(busLineSearchDemo, busLineSearchDemo.mMapView);

                // �˴���չʾһ��������Ϊʾ��
                routeOverlay.SetData(result.BusRoute);

                //�������ͼ��
                busLineSearchDemo.mMapView.Overlays.Clear();

                //���·��ͼ��
                busLineSearchDemo.mMapView.Overlays.Add(routeOverlay);

                //ˢ�µ�ͼʹ��Ч
                busLineSearchDemo.mMapView.Refresh();

                //�ƶ���ͼ�����
                busLineSearchDemo.mMapView.Controller.AnimateTo(result.BusRoute.Start);

                //��·�����ݱ����ȫ�ֱ���
                busLineSearchDemo.route = result.BusRoute;

                //����·�߽ڵ��������ڵ����ʱʹ��
                busLineSearchDemo.nodeIndex = -1;
                busLineSearchDemo.mBtnPre.Visibility = ViewStates.Visible;
                busLineSearchDemo.mBtnNext.Visibility = ViewStates.Visible;
                Toast.MakeText(busLineSearchDemo,
                                result.BusName,
                                ToastLength.Short).Show();
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

        class ClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private BusLineSearchDemo busLineSearchDemo;

            public ClickListenerImpl(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnClick(View v)
            {
                busLineSearchDemo.SearchButtonProcess(v);
            }
        }

        class NextLineClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private BusLineSearchDemo busLineSearchDemo;

            public NextLineClickListener(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnClick(View v)
            {
                busLineSearchDemo.SearchNextBusline();
            }
        }

        class NodeClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private BusLineSearchDemo busLineSearchDemo;

            public NodeClickListener(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnClick(View v)
            {
                busLineSearchDemo.NodeClick(v);
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

            SetContentView(Resource.Layout.buslinesearch);

            ICharSequence titleLable = new String("������·��ѯ����");

            Title = titleLable.ToString();

            //��ͼ��ʼ��
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.EnableClick(true);
            mMapView.Controller.SetZoom(12);
            busLineIDList = new List<string>();

            //���� ��������ͼ��
            CreatePaopao();

            // �趨������ť����Ӧ
            mBtnSearch = FindViewById<Button>(Resource.Id.search);
            mBtnNextLine = FindViewById<Button>(Resource.Id.nextline);
            mBtnPre = FindViewById<Button>(Resource.Id.pre);
            mBtnNext = FindViewById<Button>(Resource.Id.next);
            mBtnPre.Visibility = ViewStates.Invisible;
            mBtnNext.Visibility = ViewStates.Invisible;

            //��ͼ����¼�����
            mMapView.RegMapTouchListner(new MKMapTouchListenerImpl(this));

            // ��ʼ������ģ�飬ע���¼�����
            mSearch = new MKSearch();
            mSearch.Init(app.mBMapManager, new MKSearchListenerImpl(this));

            Android.Views.View.IOnClickListener clickListener = new ClickListenerImpl(this);
            Android.Views.View.IOnClickListener nextLineClickListener = new NextLineClickListener(this);
            Android.Views.View.IOnClickListener nodeClickListener = new NodeClickListener(this);

            mBtnSearch.SetOnClickListener(clickListener);
            mBtnNextLine.SetOnClickListener(nextLineClickListener);
            mBtnPre.SetOnClickListener(nodeClickListener);
            mBtnNext.SetOnClickListener(nodeClickListener);

            //mBtnSearch.Click += (sender, e) =>
            //{
            //    SearchButtonProcess(mBtnSearch);
            //};


            //mBtnNextLine.Click += (sender, e) =>
            //{
            //    SearchNextBusline();
            //};

            //mBtnPre.Click += (sender, e) =>
            //{
            //    NodeClick(mBtnPre);
            //};

            //mBtnNext.Click += (sender, e) =>
            //{
            //    NodeClick(mBtnNext);
            //};
        }

        /**
         * �������
         * @param v
         */
        void SearchButtonProcess(View v)
        {
            busLineIDList.Clear();
            busLineIndex = 0;
            mBtnPre.Visibility = ViewStates.Invisible;
            mBtnNext.Visibility = ViewStates.Invisible;
            if (mBtnSearch.Equals(v))
            {
                EditText editCity = FindViewById<EditText>(Resource.Id.city);
                EditText editSearchKey = FindViewById<EditText>(Resource.Id.searchkey);
                //����poi�������ӵõ�����poi���ҵ�������·���͵�poi����ʹ�ø�poi��uid���й�����������
                mSearch.PoiSearchInCity(editCity.Text.ToString(), editSearchKey.Text.ToString());
            }

        }

        void SearchNextBusline()
        {
            if (busLineIndex >= busLineIDList.Count)
            {
                busLineIndex = 0;
            }
            if (busLineIndex >= 0 && busLineIndex < busLineIDList.Count && busLineIDList.Count > 0)
            {
                mSearch.BusLineSearch((FindViewById<EditText>(Resource.Id.city)).Text.ToString(), busLineIDList[busLineIndex]);
                busLineIndex++;
            }

        }

        class PopListener : Java.Lang.Object, IPopupClickListener
        {
            private BusLineSearchDemo busLineSearchDemo;

            public PopListener(BusLineSearchDemo busLineSearchDemo)
            {
                this.busLineSearchDemo = busLineSearchDemo;
            }

            public void OnClickedPopup(int index)
            {
                Log.Verbose("click", "clickapoapo");
            }
        }

        /**
         * ������������ͼ��
         */
        public void CreatePaopao()
        {
            viewCache = LayoutInflater.Inflate(Resource.Layout.custom_text_view, null);
            popupText = viewCache.FindViewById<TextView>(Resource.Id.textcache);

            //���ݵ����Ӧ�ص�
            IPopupClickListener popListener = new PopListener(this);

            pop = new PopupOverlay(mMapView, popListener);
        }

        /**
         * �ڵ����ʾ��
         * @param v
         */
        public void NodeClick(View v)
        {

            if (nodeIndex < -1 || route == null || nodeIndex >= route.NumSteps)
                return;
            viewCache = LayoutInflater.Inflate(Resource.Layout.custom_text_view, null);
            popupText = viewCache.FindViewById<TextView>(Resource.Id.textcache);
            //��һ���ڵ�
            if (mBtnPre.Equals(v) && nodeIndex > 0)
            {
                //������
                nodeIndex--;
                //�ƶ���ָ������������
                mMapView.Controller.AnimateTo(route.GetStep(nodeIndex).Point);
                //��������
                popupText.Text = route.GetStep(nodeIndex).Content;
                popupText.SetBackgroundResource(Resource.Drawable.popup);
                pop.ShowPopup(BMapUtil.GetBitmapFromView(popupText),
                        route.GetStep(nodeIndex).Point,
                        5);
            }
            //��һ���ڵ�
            if (mBtnNext.Equals(v) && nodeIndex < (route.NumSteps - 1))
            {
                //������
                nodeIndex++;
                //�ƶ���ָ������������
                mMapView.Controller.AnimateTo(route.GetStep(nodeIndex).Point);
                //��������
                popupText.Text = route.GetStep(nodeIndex).Content;
                popupText.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.popup));
                pop.ShowPopup(BMapUtil.GetBitmapFromView(popupText),
                        route.GetStep(nodeIndex).Point,
                        5);
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
            mMapView.Destroy();
            mSearch.Destory();
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