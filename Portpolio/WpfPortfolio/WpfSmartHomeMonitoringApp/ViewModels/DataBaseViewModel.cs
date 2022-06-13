﻿using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class DataBaseViewModel : Screen
    {
        // Alt+Enter : 필드 캡슐화 활용
        private string brokerUrl;
        private string topic;
        private string connString;
        private string dbLog;
        private bool isConnected;

        public string BrokerUrl
        {
            get => brokerUrl;
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }
        public string Topic
        {
            get => topic;
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }
        public string ConnString
        {
            get => connString;
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }
        public string DbLog
        {
            get => dbLog;
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }
        public bool IsConnected
        {
            get => isConnected; set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "127.0.0.1";   // MQTT Broker IP 설정
            Topic = Commons.PUB_TOPIC = "home/device/#";
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            //Single Level wildcard +
            //Multi Level wildcard #

            if(Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }
        }

        /// <summary>
        /// DB연결 + MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if(IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);

                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        //Commons.MQTT_CLIENT.MqttMsgUnsubscribed += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> MQTT Broker Connected");
                        IsConnected = Commons.IS_CONNECT = true;
                    }
                }
                catch (Exception ex)
                {
                    //pass
                }
            }
            else
            {
                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected...");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception ex)
                {

                    
                }
            }
        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }

        /// <summary>
        /// Subscribe한 메시지 처리해주는 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            UpdateText(message);        //센서데이터 출력

            SetDataBase(message, e.Topic);       //DB에 저장
        }

        private void SetDataBase(string message, string topic)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            var model = new SmartHomeModel();       
            Debug.WriteLine(currDatas);
            model.Devid = currDatas["Devid"];
            model.CurrTime = DateTime.Parse(currDatas["CurrTime"]);
            model.Temp = double.Parse(currDatas["Temp"]);
            model.Humid = double.Parse(currDatas["Humid"]);

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();
                //Verbatim string C#
                string strInQuery = @"INSERT INTO TblSmartHome
                               (Devid
           , CurrTime
           , Temp
           , Humid)
     VALUES
           (@Devid
           , @CurrTime
           , @Temp
           , @Humid)";


                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);
                    SqlParameter parmDevid = new SqlParameter("@Devid", model.Devid);
                    cmd.Parameters.Add(parmDevid);
                    //날짜형으로 변환필요
                    SqlParameter parmCurrTime = new SqlParameter
                        ("@CurrTime", model.CurrTime);
                    cmd.Parameters.Add(parmCurrTime);
                    SqlParameter parmTemp = new SqlParameter("@Temp", model.Temp);
                    cmd.Parameters.Add(parmTemp);
                    SqlParameter parmHumid = new SqlParameter("@Humid", model.Humid);
                    cmd.Parameters.Add(parmHumid);

                    if (cmd.ExecuteNonQuery() == 1)
                        UpdateText(">>> DB Inserted.");         //저장성공
                    else
                        UpdateText(">>> DB Failed!!!!!!!!!!");  //저장실패
                }
                catch (Exception ex)
                {

                    UpdateText($">>>DB Error!  {ex.Message}");  //예외
                }
            }// conn.Close() 불필요

        }
    }
}
