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
using Android.Content.PM;

namespace baidumapsdk.demo
{
    /**
     * ��demo����չʾ��ν��мݳ������С�����·���������ڵ�ͼʹ��RouteOverlay��TransitOverlay����
     * ͬʱչʾ��ν��нڵ��������������
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_route", ScreenOrientation = ScreenOrientation.Sensor)]
    public class RoutePlanDemo : Activity
    {

        //UI���
        Button mBtnDrive = null;	// �ݳ�����
        Button mBtnTransit = null;	// ��������
        Button mBtnWalk = null;	// ��������
        Button mBtnCusRoute = null; //�Զ���·��
        Button mBtnCusIcon = null; //�Զ������յ�ͼ��

        //���·�߽ڵ����
        Button mBtnPre = null;//��һ���ڵ�
        Button mBtnNext = null;//��һ���ڵ�
        int nodeIndex = -2;//�ڵ�����,������ڵ�ʱʹ��
        MKRoute route = null;//����ݳ�/����·�����ݵı�����������ڵ�ʱʹ��
        TransitOverlay transitOverlay = null;//���湫��·��ͼ�����ݵı�����������ڵ�ʱʹ��
        RouteOverlay routeOverlay = null;
        bool useDefaultIcon = false;
        int searchType = -1;//��¼���������ͣ����ּݳ�/���к͹���
        private PopupOverlay pop = null;//��������ͼ�㣬����ڵ�ʱʹ��
        private TextView popupText = null;//����view
        private View viewCache = null;

        //��ͼ��أ�ʹ�ü̳�MapView��MyRouteMapViewĿ������дtouch�¼�ʵ�����ݴ���
        //���������touch�¼���������̳У�ֱ��ʹ��MapView����
        MapView mMapView = null;	// ��ͼView
        //�������
        MKSearch mSearch = null;	// ����ģ�飬Ҳ��ȥ����ͼģ�����ʹ��


        class ClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private RoutePlanDemo routePlanDemo;

            public ClickListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }

            public void OnClick(View v)
            {
                //��������
                routePlanDemo.SearchButtonProcess(v);
            }
        }

        class NodeClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private RoutePlanDemo routePlanDemo;

            public NodeClickListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }

            public void OnClick(View v)
            {
                //���·�߽ڵ�
                routePlanDemo.NodeClick(v);
            }
        }

        class CustomClickListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private RoutePlanDemo routePlanDemo;

            public CustomClickListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }

            public void OnClick(View v)
            {
                //����·�߻���ʾ��
                routePlanDemo.IntentToActivity();
            }
        }


        class ChangeRouteIconListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private RoutePlanDemo routePlanDemo;

            public ChangeRouteIconListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }

            public void OnClick(View v)
            {
                //���·�߽ڵ�
                routePlanDemo.ChangeRouteIcon();
            }
        }

        class IMKMapTouchListenerImpl : Java.Lang.Object, IMKMapTouchListener
        {


            private RoutePlanDemo routePlanDemo;

            public IMKMapTouchListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }


            public void OnMapClick(GeoPoint point)
            {
                //�ڴ˴����ͼ����¼� 
                //����pop
                if (routePlanDemo.pop != null)
                {
                    routePlanDemo.pop.HidePop();
                }
            }

            public void OnMapDoubleClick(GeoPoint p0)
            {
            }

            public void OnMapLongClick(GeoPoint p0)
            {
            }
        }
        class IMKSearchListenerImpl : Java.Lang.Object, IMKSearchListener
        {


            private RoutePlanDemo routePlanDemo;

            public IMKSearchListenerImpl(RoutePlanDemo routePlanDemo)
            {
                this.routePlanDemo = routePlanDemo;
            }




            public void OnGetDrivingRouteResult(MKDrivingRouteResult res,
                                int error)
            {

                //�����յ������壬��Ҫѡ�����ĳ����б���ַ�б�
                if (error == MKEvent.ErrorRouteAddr)
                {
                    //�������е�ַ
                    //IList<MKPoiInfo> stPois = res.AddrResult.MStartPoiList;
                    //IList<MKPoiInfo> enPois = res.AddrResult.MEndPoiList;
                    //IList<MKCityListInfo> stCities = res.AddrResult.MStartCityList;
                    //IList<MKCityListInfo> enCities = res.AddrResult.MEndCityList;
                    return;
                }
                // ����ſɲο�MKEvent�еĶ���
                if (error != 0 || res == null)
                {
                    Toast.MakeText(routePlanDemo, "��Ǹ��δ�ҵ����", ToastLength.Short).Show();
                    return;
                }

                routePlanDemo.searchType = 0;
                routePlanDemo.routeOverlay = new RouteOverlay(routePlanDemo, routePlanDemo.mMapView);
                // �˴���չʾһ��������Ϊʾ��
                routePlanDemo.routeOverlay.SetData(res.GetPlan(0).GetRoute(0));
                //�������ͼ��
                routePlanDemo.mMapView.Overlays.Clear();
                //���·��ͼ��
                routePlanDemo.mMapView.Overlays.Add(routePlanDemo.routeOverlay);
                //ִ��ˢ��ʹ��Ч
                routePlanDemo.mMapView.Refresh();
                // ʹ��zoomToSpan()���ŵ�ͼ��ʹ·������ȫ��ʾ�ڵ�ͼ��
                routePlanDemo.mMapView.Controller.ZoomToSpan(routePlanDemo.routeOverlay.LatSpanE6, routePlanDemo.routeOverlay.LonSpanE6);
                //�ƶ���ͼ�����
                routePlanDemo.mMapView.Controller.AnimateTo(res.Start.Pt);
                //��·�����ݱ����ȫ�ֱ���
                routePlanDemo.route = res.GetPlan(0).GetRoute(0);
                //����·�߽ڵ��������ڵ����ʱʹ��
                routePlanDemo.nodeIndex = -1;
                routePlanDemo.mBtnPre.Visibility = ViewStates.Visible;
                routePlanDemo.mBtnNext.Visibility = ViewStates.Visible;

            }

            public void OnGetTransitRouteResult(MKTransitRouteResult res,
                                int error)
            {
                //�����յ������壬��Ҫѡ�����ĳ����б���ַ�б�
                if (error == MKEvent.ErrorRouteAddr)
                {
                    //�������е�ַ
                    //IList<MKPoiInfo> stPois = res.AddrResult.MStartPoiList;
                    //IList<MKPoiInfo> enPois = res.AddrResult.MEndPoiList;
                    //IList<MKCityListInfo> stCities = res.AddrResult.MStartCityList;
                    //IList<MKCityListInfo> enCities = res.AddrResult.MEndCityList;
                    return;
                }
                if (error != 0 || res == null)
                {
                    Toast.MakeText(routePlanDemo, "��Ǹ��δ�ҵ����", ToastLength.Short).Show();
                    return;
                }

                routePlanDemo.searchType = 1;
                routePlanDemo.transitOverlay = new TransitOverlay(routePlanDemo, routePlanDemo.mMapView);
                // �˴���չʾһ��������Ϊʾ��
                routePlanDemo.transitOverlay.SetData(res.GetPlan(0));
                //�������ͼ��
                routePlanDemo.mMapView.Overlays.Clear();
                //���·��ͼ��
                routePlanDemo.mMapView.Overlays.Add(routePlanDemo.transitOverlay);
                //ִ��ˢ��ʹ��Ч
                routePlanDemo.mMapView.Refresh();
                // ʹ��zoomToSpan()���ŵ�ͼ��ʹ·������ȫ��ʾ�ڵ�ͼ��
                routePlanDemo.mMapView.Controller.ZoomToSpan(routePlanDemo.transitOverlay.LatSpanE6, routePlanDemo.transitOverlay.LonSpanE6);
                //�ƶ���ͼ�����
                routePlanDemo.mMapView.Controller.AnimateTo(res.Start.Pt);
                //����·�߽ڵ��������ڵ����ʱʹ��
                routePlanDemo.nodeIndex = 0;
                routePlanDemo.mBtnPre.Visibility = ViewStates.Visible;
                routePlanDemo.mBtnNext.Visibility = ViewStates.Visible;
            }

            public void OnGetWalkingRouteResult(MKWalkingRouteResult res,
                                int error)
            {
                //�����յ������壬��Ҫѡ�����ĳ����б���ַ�б�
                if (error == MKEvent.ErrorRouteAddr)
                {
                    //�������е�ַ
                    //IList<MKPoiInfo> stPois = res.AddrResult.MStartPoiList;
                    //IList<MKPoiInfo> enPois = res.AddrResult.MEndPoiList;
                    //IList<MKCityListInfo> stCities = res.AddrResult.MStartCityList;
                    //IList<MKCityListInfo> enCities = res.AddrResult.MEndCityList;
                    return;
                }
                if (error != 0 || res == null)
                {
                    Toast.MakeText(routePlanDemo, "��Ǹ��δ�ҵ����", ToastLength.Short).Show();
                    return;
                }

                routePlanDemo.searchType = 2;
                routePlanDemo.routeOverlay = new RouteOverlay(routePlanDemo, routePlanDemo.mMapView);
                // �˴���չʾһ��������Ϊʾ��
                routePlanDemo.routeOverlay.SetData(res.GetPlan(0).GetRoute(0));
                //�������ͼ��
                routePlanDemo.mMapView.Overlays.Clear();
                //���·��ͼ��
                routePlanDemo.mMapView.Overlays.Add(routePlanDemo.routeOverlay);
                //ִ��ˢ��ʹ��Ч
                routePlanDemo.mMapView.Refresh();
                // ʹ��zoomToSpan()���ŵ�ͼ��ʹ·������ȫ��ʾ�ڵ�ͼ��
                routePlanDemo.mMapView.Controller.ZoomToSpan(routePlanDemo.routeOverlay.LatSpanE6, routePlanDemo.routeOverlay.LonSpanE6);
                //�ƶ���ͼ�����
                routePlanDemo.mMapView.Controller.AnimateTo(res.Start.Pt);
                //��·�����ݱ����ȫ�ֱ���
                routePlanDemo.route = res.GetPlan(0).GetRoute(0);
                //����·�߽ڵ��������ڵ����ʱʹ��
                routePlanDemo.nodeIndex = -1;
                routePlanDemo.mBtnPre.Visibility = ViewStates.Visible;
                routePlanDemo.mBtnNext.Visibility = ViewStates.Visible;
            }

            public void OnGetAddrResult(MKAddrInfo res, int error)
            {
            }
            public void OnGetPoiResult(MKPoiResult res, int arg1, int arg2)
            {
            }
            public void OnGetBusDetailResult(MKBusLineResult result, int iError)
            {
            }

            public void OnGetSuggestionResult(MKSuggestionResult res, int arg1)
            {
            }

            public void OnGetPoiDetailSearchResult(int type, int iError)
            {
                // TODO Auto-generated method stub
            }

            public void OnGetShareUrlResult(MKShareUrlResult result, int type,
                    int error)
            {
                // TODO Auto-generated method stub

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
            SetContentView(Resource.Layout.routeplan);
            ICharSequence titleLable = new String("·�߹滮����");
            Title = titleLable.ToString();
            //��ʼ����ͼ
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.SetBuiltInZoomControls(false);
            mMapView.Controller.SetZoom(12);
            mMapView.Controller.EnableClick(true);

            //��ʼ������
            mBtnDrive = FindViewById<Button>(Resource.Id.drive);
            mBtnTransit = FindViewById<Button>(Resource.Id.transit);
            mBtnWalk = FindViewById<Button>(Resource.Id.walk);
            mBtnPre = FindViewById<Button>(Resource.Id.pre);
            mBtnNext = FindViewById<Button>(Resource.Id.next);
            mBtnCusRoute = FindViewById<Button>(Resource.Id.custombutton);
            mBtnCusIcon = FindViewById<Button>(Resource.Id.customicon);
            mBtnPre.Visibility = ViewStates.Invisible;
            mBtnNext.Visibility = ViewStates.Invisible;

            //��������¼�
            Android.Views.View.IOnClickListener clickListener = new ClickListenerImpl(this);
            Android.Views.View.IOnClickListener nodeClickListener = new NodeClickListenerImpl(this);
            Android.Views.View.IOnClickListener customClickListener = new CustomClickListenerImpl(this);
            Android.Views.View.IOnClickListener changeRouteIconListener = new ChangeRouteIconListenerImpl(this);

            mBtnDrive.SetOnClickListener(clickListener);
            mBtnTransit.SetOnClickListener(clickListener);
            mBtnWalk.SetOnClickListener(clickListener);
            mBtnPre.SetOnClickListener(nodeClickListener);
            mBtnNext.SetOnClickListener(nodeClickListener);
            mBtnCusRoute.SetOnClickListener(customClickListener);
            mBtnCusIcon.SetOnClickListener(changeRouteIconListener);
            //���� ��������ͼ��
            CreatePaopao();

            //��ͼ����¼�����
            mMapView.RegMapTouchListner(new IMKMapTouchListenerImpl(this));
            // ��ʼ������ģ�飬ע���¼�����
            mSearch = new MKSearch();
            mSearch.Init(app.mBMapManager, new IMKSearchListenerImpl(this));
        }
        /**
         * ����·�߹滮����ʾ��
         * @param v
         */
        void SearchButtonProcess(View v)
        {
            //��������ڵ��·������
            route = null;
            routeOverlay = null;
            transitOverlay = null;
            mBtnPre.Visibility = ViewStates.Invisible;
            mBtnNext.Visibility = ViewStates.Invisible;
            // ����������ť��Ӧ
            EditText editSt = FindViewById<EditText>(Resource.Id.start);
            EditText editEn = FindViewById<EditText>(Resource.Id.end);

            // ������յ��name���и�ֵ��Ҳ����ֱ�Ӷ����긳ֵ����ֵ�����򽫸��������������
            MKPlanNode stNode = new MKPlanNode();
            stNode.Name = editSt.Text;
            MKPlanNode enNode = new MKPlanNode();
            enNode.Name = editEn.Text;

            // ʵ��ʹ�����������յ���н�����ȷ���趨
            if (mBtnDrive.Equals(v))
            {
                mSearch.DrivingSearch("����", stNode, "����", enNode);
            }
            else if (mBtnTransit.Equals(v))
            {
                mSearch.TransitSearch("����", stNode, enNode);
            }
            else if (mBtnWalk.Equals(v))
            {
                mSearch.WalkingSearch("����", stNode, "����", enNode);
            }
        }
        /**
         * �ڵ����ʾ��
         * @param v
         */
        public void NodeClick(View v)
        {
            viewCache = LayoutInflater.Inflate(Resource.Layout.custom_text_view, null);
            popupText = viewCache.FindViewById<TextView>(Resource.Id.textcache);
            if (searchType == 0 || searchType == 2)
            {
                //�ݳ�������ʹ�õ����ݽṹ��ͬ���������Ϊ�ݳ����У��ڵ����������ͬ
                if (nodeIndex < -1 || route == null || nodeIndex >= route.NumSteps)
                    return;

                //��һ���ڵ�
                if (mBtnPre.Equals(v) && nodeIndex > 0)
                {
                    //������
                    nodeIndex--;
                    //�ƶ���ָ������������
                    mMapView.Controller.AnimateTo(route.GetStep(nodeIndex).Point);
                    //��������
                    popupText.SetBackgroundResource(Resource.Drawable.popup);
                    popupText.Text = route.GetStep(nodeIndex).Content;
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
                    popupText.SetBackgroundResource(Resource.Drawable.popup);
                    popupText.Text = route.GetStep(nodeIndex).Content;
                    pop.ShowPopup(BMapUtil.GetBitmapFromView(popupText),
                            route.GetStep(nodeIndex).Point,
                            5);
                }
            }
            if (searchType == 1)
            {
                //��������ʹ�õ����ݽṹ��������ͬ����˵�������ڵ����
                if (nodeIndex < -1 || transitOverlay == null || nodeIndex >= transitOverlay.AllItem.Count)
                    return;

                //��һ���ڵ�
                if (mBtnPre.Equals(v) && nodeIndex > 1)
                {
                    //������
                    nodeIndex--;
                    //�ƶ���ָ������������
                    mMapView.Controller.AnimateTo(transitOverlay.GetItem(nodeIndex).Point);
                    //��������
                    popupText.SetBackgroundResource(Resource.Drawable.popup);
                    popupText.Text = transitOverlay.GetItem(nodeIndex).Title;
                    pop.ShowPopup(BMapUtil.GetBitmapFromView(popupText),
                            transitOverlay.GetItem(nodeIndex).Point,
                            5);
                }
                //��һ���ڵ�
                if (mBtnNext.Equals(v) && nodeIndex < (transitOverlay.AllItem.Count - 2))
                {
                    //������
                    nodeIndex++;
                    //�ƶ���ָ������������
                    mMapView.Controller.AnimateTo(transitOverlay.GetItem(nodeIndex).Point);
                    //��������
                    popupText.SetBackgroundResource(Resource.Drawable.popup);
                    popupText.Text = transitOverlay.GetItem(nodeIndex).Title;
                    pop.ShowPopup(BMapUtil.GetBitmapFromView(popupText),
                            transitOverlay.GetItem(nodeIndex).Point,
                            5);
                }
            }

        }

        class IPopupClickListenerImpl : Java.Lang.Object, IPopupClickListener
        {
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

            ////���ݵ����Ӧ�ص�
            IPopupClickListener popListener = new IPopupClickListenerImpl();
            pop = new PopupOverlay(mMapView, popListener);
        }
        /**
         * ��ת����·��Activity
         */
        public void IntentToActivity()
        {
            //��ת������·����ʾdemo
            Intent intent = new Intent();
            intent.SetClass(this, typeof(CustomRouteOverlayDemo));
            StartActivity(intent);
        }

        /**
         * �л�·��ͼ�꣬ˢ�µ�ͼʹ����Ч
         * ע�⣺ ���յ�ͼ��ʹ�����Ķ���.
         */
        protected void ChangeRouteIcon()
        {
            Button btn = FindViewById<Button>(Resource.Id.customicon);
            if (routeOverlay == null && transitOverlay == null)
            {
                return;
            }
            if (useDefaultIcon)
            {
                if (routeOverlay != null)
                {
                    routeOverlay.StMarker = null;
                    routeOverlay.EnMarker = null;
                }
                if (transitOverlay != null)
                {
                    transitOverlay.StMarker = null;
                    transitOverlay.EnMarker = null;
                }
                btn.Text = "�Զ������յ�ͼ��";
                Toast.MakeText(this,
                               "��ʹ��ϵͳ���յ�ͼ��",
                               ToastLength.Short).Show();
            }
            else
            {
                if (routeOverlay != null)
                {
                    routeOverlay.StMarker = Resources.GetDrawable(Resource.Drawable.icon_st);
                    routeOverlay.EnMarker = Resources.GetDrawable(Resource.Drawable.icon_en);
                }
                if (transitOverlay != null)
                {
                    transitOverlay.StMarker = Resources.GetDrawable(Resource.Drawable.icon_st);
                    transitOverlay.EnMarker = Resources.GetDrawable(Resource.Drawable.icon_en);
                }
                btn.Text = "ϵͳ���յ�ͼ��";
                Toast.MakeText(this,
                               "��ʹ���Զ������յ�ͼ��",
                               ToastLength.Short).Show();
            }
            useDefaultIcon = !useDefaultIcon;
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