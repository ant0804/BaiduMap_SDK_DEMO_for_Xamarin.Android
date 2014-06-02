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
     * ��demo����չʾ����ڵ�ͼ����GraphicsOverlay��ӵ㡢�ߡ�����Ρ�Բ
     * ͬʱչʾ����ڵ�ͼ����TextOverlay�������
     *
     */
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Label = "@string/demo_name_geometry", ScreenOrientation = ScreenOrientation.Sensor)]
    public class GeometryDemo : Activity
    {
        //��ͼ���
        MapView mMapView = null;

        //UI���
        Button resetBtn = null;
        Button clearBtn = null;

        class ClearListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            GeometryDemo geometryDemo;
            public ClearListenerImpl(GeometryDemo geometryDemo)
            {
                this.geometryDemo = geometryDemo;
            }
            public void OnClick(View v)
            {
                geometryDemo.ClearClick();
            }
        }
        class RestListenerImpl : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            GeometryDemo geometryDemo;
            public RestListenerImpl(GeometryDemo geometryDemo)
            {
                this.geometryDemo = geometryDemo;
            }
            public void OnClick(View v)
            {
                geometryDemo.ResetClick();
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
            SetContentView(Resource.Layout.activity_geometry);
            ICharSequence titleLable = new String("�Զ�����ƹ���");
            Title = titleLable.ToString();

            //��ʼ����ͼ
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mMapView.Controller.SetZoom(12.5f);
            mMapView.Controller.EnableClick(true);

            //UI��ʼ��
            clearBtn = FindViewById<Button>(Resource.Id.button1);
            resetBtn = FindViewById<Button>(Resource.Id.button2);

            Android.Views.View.IOnClickListener clearListener = new ClearListenerImpl(this);
            Android.Views.View.IOnClickListener restListener = new RestListenerImpl(this);

            clearBtn.SetOnClickListener(clearListener);
            resetBtn.SetOnClickListener(restListener);

            //�������ʱ��ӻ���ͼ��
            AddCustomElementsDemo();
        }

        /**
         * ��ӵ㡢�ߡ�����Ρ�Բ������
         */
        public void AddCustomElementsDemo()
        {
            GraphicsOverlay graphicsOverlay = new GraphicsOverlay(mMapView);
            mMapView.Overlays.Add(graphicsOverlay);
            //��ӵ�
            graphicsOverlay.SetData(DrawPoint());
            //�������
            graphicsOverlay.SetData(DrawLine());
            //��ӻ���
            graphicsOverlay.SetData(DrawArc());
            //��Ӷ����
            graphicsOverlay.SetData(DrawPolygon());
            //���Բ
            graphicsOverlay.SetData(DrawCircle());
            //��������
            TextOverlay textOverlay = new TextOverlay(mMapView);
            mMapView.Overlays.Add(textOverlay);
            textOverlay.AddText(DrawText());
            //ִ�е�ͼˢ��ʹ��Ч
            mMapView.Refresh();
        }

        public void ResetClick()
        {
            //��ӻ���Ԫ��
            AddCustomElementsDemo();
        }

        public void ClearClick()
        {
            //�������ͼ��
            mMapView.Overlays.Clear();
            mMapView.Refresh();
        }

        /**
         * �������ߣ�������״̬���ͼ״̬�仯
         * @return ���߶���
         */
        public Graphic DrawLine()
        {
            double mLat = 39.97923;
            double mLon = 116.357428;

            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);

            mLat = 39.94923;
            mLon = 116.397428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt2 = new GeoPoint(lat, lon);
            mLat = 39.97923;
            mLon = 116.437428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt3 = new GeoPoint(lat, lon);

            //������
            Geometry lineGeometry = new Geometry();
            //�趨���ߵ�����
            GeoPoint[] linePoints = new GeoPoint[3];
            linePoints[0] = pt1;
            linePoints[1] = pt2;
            linePoints[2] = pt3;
            lineGeometry.SetPolyLine(linePoints);
            //�趨��ʽ
            Symbol lineSymbol = new Symbol();
            Symbol.Color lineColor = new Com.Baidu.Mapapi.Map.Symbol.Color(lineSymbol);
            lineColor.Red = 255;
            lineColor.Green = 0;
            lineColor.Blue = 0;
            lineColor.Alpha = 255;
            lineSymbol.SetLineSymbol(lineColor, 10);
            //����Graphic����
            Graphic lineGraphic = new Graphic(lineGeometry, lineSymbol);
            return lineGraphic;
        }

        /**
         * ���ƻ���
         * 
         * @return ���߶���
         */
        public Graphic DrawArc()
        {
            double mLat = 39.97923;
            double mLon = 116.357428;

            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);

            mLat = 39.94923;
            mLon = 116.397428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt2 = new GeoPoint(lat, lon);
            mLat = 39.97923;
            mLon = 116.437428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt3 = new GeoPoint(lat, lon);

            Geometry arcGeometry = new Geometry();

            arcGeometry.SetArc(pt1, pt3, pt2);
            // �趨��ʽ
            Symbol arcSymbol = new Symbol();
            Symbol.Color arcColor = new Com.Baidu.Mapapi.Map.Symbol.Color(arcSymbol);
            arcColor.Red = 255;
            arcColor.Green = 0;
            arcColor.Blue = 225;
            arcColor.Alpha = 255;
            arcSymbol.SetLineSymbol(arcColor, 4);
            // ����Graphic����
            Graphic arcGraphic = new Graphic(arcGeometry, arcSymbol);
            return arcGraphic;
        }

        /**
         * ���ƶ���Σ��ö�������ͼ״̬�仯
         * @return ����ζ���
         */
        public Graphic DrawPolygon()
        {
            double mLat = 39.93923;
            double mLon = 116.357428;
            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);
            mLat = 39.91923;
            mLon = 116.327428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt2 = new GeoPoint(lat, lon);
            mLat = 39.89923;
            mLon = 116.347428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt3 = new GeoPoint(lat, lon);
            mLat = 39.89923;
            mLon = 116.367428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt4 = new GeoPoint(lat, lon);
            mLat = 39.91923;
            mLon = 116.387428;
            lat = (int)(mLat * 1E6);
            lon = (int)(mLon * 1E6);
            GeoPoint pt5 = new GeoPoint(lat, lon);

            //���������
            Geometry polygonGeometry = new Geometry();
            //���ö��������
            GeoPoint[] polygonPoints = new GeoPoint[5];
            polygonPoints[0] = pt1;
            polygonPoints[1] = pt2;
            polygonPoints[2] = pt3;
            polygonPoints[3] = pt4;
            polygonPoints[4] = pt5;
            polygonGeometry.SetPolygon(polygonPoints);
            //���ö������ʽ
            Symbol polygonSymbol = new Symbol();
            Symbol.Color polygonColor = new Com.Baidu.Mapapi.Map.Symbol.Color(polygonSymbol);
            polygonColor.Red = 0;
            polygonColor.Green = 0;
            polygonColor.Blue = 255;
            polygonColor.Alpha = 126;
            polygonSymbol.SetSurface(polygonColor, 1, 5);
            //����Graphic����
            Graphic polygonGraphic = new Graphic(polygonGeometry, polygonSymbol);
            return polygonGraphic;
        }

        /**
         * ���Ƶ��㣬�õ�״̬�����ͼ״̬�仯���仯
         * @return �����
         */
        public Graphic DrawPoint()
        {
            double mLat = 39.98923;
            double mLon = 116.397428;
            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);

            //������
            Geometry pointGeometry = new Geometry();
            //��������
            pointGeometry.SetPoint(pt1, 10);
            //�趨��ʽ
            Symbol pointSymbol = new Symbol();
            Symbol.Color pointColor = new Com.Baidu.Mapapi.Map.Symbol.Color(pointSymbol);
            pointColor.Red = 0;
            pointColor.Green = 126;
            pointColor.Blue = 255;
            pointColor.Alpha = 255;
            pointSymbol.SetPointSymbol(pointColor);
            //����Graphic����
            Graphic pointGraphic = new Graphic(pointGeometry, pointSymbol);
            return pointGraphic;
        }

        /**
         * ����Բ����Բ���ͼ״̬�仯
         * @return Բ����
         */
        public Graphic DrawCircle()
        {
            double mLat = 39.90923;
            double mLon = 116.447428;
            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);

            //����Բ
            Geometry circleGeometry = new Geometry();

            //����Բ���ĵ�����Ͱ뾶
            circleGeometry.SetCircle(pt1, 2500);
            //������ʽ
            Symbol circleSymbol = new Symbol();
            Symbol.Color circleColor = new Com.Baidu.Mapapi.Map.Symbol.Color(circleSymbol);
            circleColor.Red = 0;
            circleColor.Green = 255;
            circleColor.Blue = 0;
            circleColor.Alpha = 126;
            circleSymbol.SetSurface(circleColor, 1, 3, new Com.Baidu.Mapapi.Map.Symbol.Stroke(3, new Com.Baidu.Mapapi.Map.Symbol.Color(circleSymbol, Android.Graphics.Color.ParseColor("#FFFF0000").ToArgb())));
            //����Graphic����
            Graphic circleGraphic = new Graphic(circleGeometry, circleSymbol);
            return circleGraphic;
        }

        /**
         * �������֣����������ͼ�仯��͸��Ч��
         * @return ���ֶ���
         */
        public TextItem DrawText()
        {
            double mLat = 39.86923;
            double mLon = 116.397428;
            int lat = (int)(mLat * 1E6);
            int lon = (int)(mLon * 1E6);
            //��������
            TextItem item = new TextItem();
            //��������λ��
            item.Pt = new GeoPoint(lat, lon);
            //�����ļ�����
            item.Text = "�ٶȵ�ͼSDK";
            //�����ִ�С
            item.FontSize = 40;
            Symbol symbol = new Symbol();
            Symbol.Color bgColor = new Com.Baidu.Mapapi.Map.Symbol.Color(symbol);
            //�������ֱ���ɫ
            bgColor.Red = 0;
            bgColor.Blue = 0;
            bgColor.Green = 255;
            bgColor.Alpha = 50;

            Symbol.Color fontColor = new Com.Baidu.Mapapi.Map.Symbol.Color(symbol);
            //����������ɫ
            fontColor.Alpha = 255;
            fontColor.Red = 0;
            fontColor.Green = 0;
            fontColor.Blue = 255;
            //���ö��뷽ʽ
            item.Align = TextItem.AlignCenter;
            //����������ɫ�ͱ�����ɫ
            item.FontColor = fontColor;
            item.BgColor = bgColor;
            return item;
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