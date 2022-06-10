using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class RealTimeViewModel : Screen
    {
        private double livingTempVal;
        private double diningTempVal;
        private double bedTempVal;
        private double bathTempVal;
        private double livingHumidVal;
        private double diningHumidVal;
        private double bedHumidVal;
        private double bathHumidVal;

        public RealTimeViewModel()
        {
            Commons.BROKERHOST = "127.0.0.1";
            Commons.PUB_TOPIC = "home/device/#";

            LivingTempVal = DiningTempVal = BedTempVal = BathTempVal = 0;
            LivingHumidVal = DiningHumidVal = BedHumidVal = BathHumidVal = 0;

            if (Commons.MQTT_CLIENT != null && Commons.MQTT_CLIENT.IsConnected)
            {
                //접속이 되어있을 경우
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
            }
            else
            {
                //MQTT Broker에 접속
                Commons.MQTT_CLIENT = new MqttClient(Commons.BROKERHOST);
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                Commons.MQTT_CLIENT.Connect("Monitor");

                Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            }
        }

        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            switch (currDatas["DevId"].ToString())
            {
                case "LIVING":
                    LivingTempVal = double.Parse(currDatas["Temp"]);
                    LivingHumidVal = double.Parse(currDatas["Humid"]);
                    break;
                case "DINING":
                    DiningTempVal = double.Parse(currDatas["Temp"]);
                    DiningHumidVal = double.Parse(currDatas["Humid"]);
                    break;
                case "BED":
                    BedTempVal = double.Parse(currDatas["Temp"]);
                    BedHumidVal = double.Parse(currDatas["Humid"]);
                    break;
                case "BATH":
                    BathTempVal = double.Parse(currDatas["Temp"]);
                    BathHumidVal = double.Parse(currDatas["Humid"]);
                    break;
            }
        }

        public double LivingTempVal
        {
            get => livingTempVal; set
            {
                livingTempVal = value;
                NotifyOfPropertyChange(() => LivingTempVal);
            }
        }
        public double DiningTempVal
        {
            get => diningTempVal; set
            {
                diningTempVal = value;
                NotifyOfPropertyChange(() => DiningTempVal);
            }
        }
        public double BedTempVal
        {
            get => bedTempVal; set
            {
                bedTempVal = value;
                NotifyOfPropertyChange(() => BedTempVal);
            }
        }
        public double BathTempVal
        {
            get => bathTempVal; set
            {
                bathTempVal = value;
                NotifyOfPropertyChange(() => BathTempVal);
            }
        }

        public double LivingHumidVal
        {
            get => livingHumidVal;
            set
            {
                livingHumidVal = value;
                NotifyOfPropertyChange(() => LivingHumidVal);
            }
        }
        public double DiningHumidVal
        {
            get => diningHumidVal; 
            set
            {
                diningHumidVal = value;
                NotifyOfPropertyChange(() => DiningHumidVal);
            }
        }
        public double BedHumidVal
        {
            get => bedHumidVal; 
            set
            {
                bedHumidVal = value;
                NotifyOfPropertyChange(() => BedHumidVal);
            }
        }
        public double BathHumidVal
        {
            get => bathHumidVal; 
            set
            {
                bathHumidVal = value;
                NotifyOfPropertyChange(() => BathHumidVal);
            }
        }
    }
}