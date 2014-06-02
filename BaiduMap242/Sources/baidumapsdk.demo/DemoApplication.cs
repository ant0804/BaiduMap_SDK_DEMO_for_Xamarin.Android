using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;

namespace baidumapsdk.demo
{
    [Application]
    public class DemoApplication : Application
    {
        private static DemoApplication mInstance = null;
        public bool m_bKeyRight = true;
        internal BMapManager mBMapManager = null;

        public DemoApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            mInstance = this;

            initEngineManager(this);
        }

        public void initEngineManager(Context context)
        {
            if (mBMapManager == null)
            {
                mBMapManager = new BMapManager(context);
            }

            if (!mBMapManager.Init(new MyGeneralListener()))
            {
                Toast.MakeText(DemoApplication.getInstance().ApplicationContext, "BMapManager ��ʼ������!", ToastLength.Short).Show();
            }
        }

        public static DemoApplication getInstance()
        {
            return mInstance;
        }

        // �����¼���������������ͨ�������������Ȩ��֤�����
        public class MyGeneralListener : Java.Lang.Object, IMKGeneralListener
        {
            public void OnGetNetworkState(int iError)
            {
                if (iError == MKEvent.ErrorNetworkConnect)
                {
                    Toast.MakeText(DemoApplication.getInstance().ApplicationContext, "���������������", ToastLength.Short).Show();
                }
                else if (iError == MKEvent.ErrorNetworkData)
                {
                    Toast.MakeText(DemoApplication.getInstance().ApplicationContext, "������ȷ�ļ���������", ToastLength.Short).Show();
                }
                // ...
            }

            public void OnGetPermissionState(int iError)
            {
                //����ֵ��ʾkey��֤δͨ��
                if (iError != 0)
                {
                    //��ȨKey����
                    Toast.MakeText(DemoApplication.getInstance().ApplicationContext, "���� DemoApplication.java�ļ�������ȷ����ȨKey,������������������Ƿ�������error: " + iError, ToastLength.Short).Show();
                    DemoApplication.getInstance().m_bKeyRight = false;
                }
                else
                {
                    DemoApplication.getInstance().m_bKeyRight = true;
                    Toast.MakeText(DemoApplication.getInstance().ApplicationContext, "key��֤�ɹ�", ToastLength.Short).Show();
                }
            }
        }
    }
}