
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
using Com.Baidu.Mapapi.Navi;

namespace baidumapsdk.demo
{
    /**
     * ��һ��Activity��չʾ�����ͼ
     */
    [Activity(Label = "@string/demo_name_navi")]
    public class NaviDemo : Activity
    {
        //�찲������
        double mLat1 = 39.915291;
        double mLon1 = 116.403857;
        //�ٶȴ�������
        double mLat2 = 40.056858;
        double mLon2 = 116.308194;

        class IOnClickPositiveButtonListenerImpl : Java.Lang.Object, IDialogInterfaceOnClickListener
        {

            NaviDemo naviDemo;

            public IOnClickPositiveButtonListenerImpl(NaviDemo naviDemo) { this.naviDemo = naviDemo; }
            public void OnClick(IDialogInterface dialog, int which)
            {
                dialog.Dismiss();
                BaiduMapNavigation.GetLatestBaiduMapApp(naviDemo);
            }
        }

        class IOnClickNegativeButtonListenerImpl : Java.Lang.Object, IDialogInterfaceOnClickListener
        {

            NaviDemo naviDemo;

            public IOnClickNegativeButtonListenerImpl(NaviDemo naviDemo) { this.naviDemo = naviDemo; }
            public void OnClick(IDialogInterface dialog, int which)
            {
                dialog.Dismiss();

            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_navi_demo);
            TextView text = (TextView)FindViewById(Resource.Id.navi_info);
            text.Text = String.Format("���:(%f,%f)\n�յ�:(%f,%f)", mLat1, mLon1, mLat2, mLon2);
        }

        /**
    * ��ʼ����		
    * @param view
    */
        [Java.Interop.Export]
        public void StartNavi(View view)
        {
            int lat = (int)(mLat1 * 1E6);
            int lon = (int)(mLon1 * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);
            lat = (int)(mLat2 * 1E6);
            lon = (int)(mLon2 * 1E6);
            GeoPoint pt2 = new GeoPoint(lat, lon);
            // ���� ��������
            NaviPara para = new NaviPara();
            para.StartPoint = pt1;
            para.StartName = "�����￪ʼ";
            para.EndPoint = pt2;
            para.EndName = "���������";

            try
            {
                BaiduMapNavigation.OpenBaiduMapNavi(para, this);
            }
            catch (BaiduMapAppNotSupportNaviException e)
            {
                e.PrintStackTrace();

                // ��Ȼ����ԭ������, ������IKVM��bug, Ҳ����������������, ��, �����ָ·
                // ������� RuntimeException �ǲ��Ե�, ��Ȼ�չ˵���
                // Com.Baidu.Mapapi.Navi.BaiduMapAppNotSupportNaviException
                // ���� OpenBaiduMapNavi ���������ܻ���һ��
                // Com.Baidu.Mapapi.Navi.IllegalNaviArgumentException
                // ������û��������
            }
            catch (RuntimeException e)
            {
                e.PrintStackTrace();
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage("����δ��װ�ٶȵ�ͼapp��app�汾���ͣ����ȷ�ϰ�װ��");
                builder.SetTitle("��ʾ");
                builder.SetPositiveButton("ȷ��", new IOnClickPositiveButtonListenerImpl(this));

                builder.SetNegativeButton("ȡ��", new IOnClickNegativeButtonListenerImpl(this));

                builder.Create().Show();
            }
        }

        [Java.Interop.Export]
        public void StartWebNavi(View view)
        {
            int lat = (int)(mLat1 * 1E6);
            int lon = (int)(mLon1 * 1E6);
            GeoPoint pt1 = new GeoPoint(lat, lon);
            lat = (int)(mLat2 * 1E6);
            lon = (int)(mLon2 * 1E6);
            GeoPoint pt2 = new GeoPoint(lat, lon);
            // ���� ��������
            NaviPara para = new NaviPara();
            para.StartPoint = pt1;
            para.EndPoint = pt2;
            BaiduMapNavigation.OpenWebBaiduMapNavi(para, this);
        }
    }
}