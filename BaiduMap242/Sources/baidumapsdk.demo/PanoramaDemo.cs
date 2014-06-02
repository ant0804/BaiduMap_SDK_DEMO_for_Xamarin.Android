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
    [Activity(Label = "@string/title_activity_panorama_demo")]
    public class PanoramaDemo : Activity
    {
        //ͨ��ȫ��ID��ȫ����ʹ�õ�Ĭ��ID��ȫ��ID����ʹ��PanoramaService��ѯ�õ�
        public static readonly string DEFAULT_PANORAMA_ID = "0100220000130817164838355J5";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_panorama_demo);
        }

        //ͨ��poi uid ��ȫ��
        [Java.Interop.Export]
        public void StartPoiSelector(View v)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(PanoramaPoiSelectorActivity));
            StartActivity(intent);
        }

        //ͨ����γ�����꿪��ȫ��
        [Java.Interop.Export]
        public void StartGeoSelector(View vS)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(PanoramaGeoSelectorActivity));
            StartActivity(intent);
        }
        //ͨ��ȫ��ID����ȫ��
        [Java.Interop.Export]
        public void StartIDSelector(View v)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(PanoramaDemoActivityMain));
            intent.PutExtra("pid", DEFAULT_PANORAMA_ID);
            StartActivity(intent);
        }
    }
}