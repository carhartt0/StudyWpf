using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if(Commons.MQTT_CLIENT.IsConnected)
            {
                Commons.MQTT_CLIENT.Disconnect();
                Commons.MQTT_CLIENT = null;
            }   // 비활성화 처리

            return base.OnDeactivateAsync(close, cancellationToken);
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

        public void ExitToolbar()
        {
            Environment.Exit(0);
        }

        // Start 메뉴, 아이콘 눌렀을 때 처리할 이벤트
        public void PopInfoDialog()
        {
            TaskPopup();
            
        }

        public void StartSubscribe()
        {
            TaskPopup();
        }

        private void TaskPopup()
        {
            // CustomPopupView
            var winMannager = new WindowManager();
            var result = winMannager.ShowDialogAsync(new CustomPopupViewModel("New Broker"));

            if (result.Result == true)
            {
                ActivateItemAsync(new DataBaseViewModel());    //화면전환
            }
        }

        public void PopInfoView()
        {
            var winManager = new WindowManager();
            winManager.ShowDialogAsync(new CustomInfoViewModel("About"));
        }
    }
}
