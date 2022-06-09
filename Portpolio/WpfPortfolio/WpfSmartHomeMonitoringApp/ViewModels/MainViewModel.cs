using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel : Conductor<object>  // Screen에는 ActivateItemAsync메서드가 존재하지 않음
    {
        public MainViewModel()
        {
            DisplayName = "SmartHome Monitoring v2.0";  //윈도우 타이틀, 제목
        }

        //TODO
        public void LoadDataBaseView()
        {
            //if (Commons.MQTT_CLIENT != null)
                ActivateItemAsync(new DataBaseViewModel());
            //else
            //    var windowManager = new WindowManager();
            //windowManager.ShowDialog(new ErrorPopupViewModel("Report|MQTT doesn't start, yet"));
        }
        public void LoadRealTimeView()
        {
            ActivateItemAsync(new RealTimeViewModel());
        }

        public void LoadHistoryView()
        {
            ActivateItemAsync(new HistoryViewModel());
        }


        //protected override void OnDeactivateAsync(bool close)
        //{
        //    if(Commons.MQTT_CLIENT.IsConnected)
        //    {
        //        Commons.MQTT_CLIENT.Disconnect();
        //        Commons.MQTT_CLIENT = null;
        //    }

        //    base.OnDeactivateAsync(close);
        //}

        //public async void PopInfoDialog(object o)
        //{
        //    await TaskStart();
        //}
        //public void StartSubscriber()
        //{
        //    TaskStart();
        //}

        //private void TaskStart()
        //{
        //    var windowManager = new WindowManager();
        //    var result = windowManager.ShowDialog(new CustomPipupViewModel("New Network"));

        //    if(result == true)
        //    {
        //        ActivateItemAsync(new DatabaseMonitoringViewModel());
        //    }
        //}






        public void ExitProgram()
        {
            Environment.Exit(0);
        }
    }
}
