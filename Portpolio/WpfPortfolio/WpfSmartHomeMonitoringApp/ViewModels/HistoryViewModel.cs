using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class HistoryViewModel : Screen
    {

        private BindableCollection<DivisionModel> divisions;
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string endDate;
        private string initEndDate;
        private int totalConut;
        private PlotModel smartHomeModel;

        /*Divisions
                DivisionVal
                SelectedDivision
                StartDate
                InitStartDate
                EndDate
                InitEndDate
                TotalCount
                SearchIoTData()
                SmartHomeModel*/
        public BindableCollection<DivisionModel> Divisions
        {
            get => divisions; 
            set
            {
                divisions = value;
                NotifyOfPropertyChange(() => Divisions);
            }
        }
        public DivisionModel SelectedDivision
        {
            get => selectedDivision; 
            set
            {
                selectedDivision = value;
                NotifyOfPropertyChange(() => SelectedDivision);
            }
        }
        public string StartDate
        {
            get => startDate; 
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        public string InitStartDate
        {
            get => initStartDate; 
            set
            {
                initStartDate = value;
                NotifyOfPropertyChange(() => InitStartDate);
            }
        }
        public string EndDate
        {
            get => endDate; 
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        public string InitEndDate
        {
            get => initEndDate; 
            set
            {
                initEndDate = value;
                NotifyOfPropertyChange(() => InitEndDate);
            }
        }
        public int TotalConut
        {
            get => totalConut; 
            set
            {
                totalConut = value;
                NotifyOfPropertyChange(() => TotalConut);
            }
        }
        public PlotModel SmartHomeModel
        {
            get => smartHomeModel; 
            set
            {
                smartHomeModel = value;
                NotifyOfPropertyChange(() => SmartHomeModel);
            }
        }

        public HistoryViewModel()
        {
            Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            InitControl();
        }

        private void InitControl()
        {
            Divisions = new BindableCollection<DivisionModel> // 콤보박스용 데이터 생성
            {
                new DivisionModel {keyVal=0,DivisionVal="-- Select --"},
                new DivisionModel {keyVal=1,DivisionVal="DINNING"},
                new DivisionModel {keyVal=2,DivisionVal="LIVING"},
                new DivisionModel {keyVal=3,DivisionVal="BED"},
                new DivisionModel {keyVal=4,DivisionVal="BATH"},
            };
            // Select를 선택해서 초기화
            SelectedDivision = Divisions.Where(v => v.DivisionVal.Contains("Select")).FirstOrDefault();

            InitStartDate = DateTime.Now.ToShortDateString();               //2022-06-10
            InitEndDate = DateTime.Now.AddDays(1).ToShortDateString();      //2022-06-11
        }

        //검색 메서드
        public void SearchIoTData()
        {
            // Validation check
            if(SelectedDivision.keyVal == 0)        //Select
            {
                MessageBox.Show("검색할 방을 선택하세요.");
                return;
            }

            if(DateTime.Parse(StartDate) > DateTime.Parse(EndDate))
            {
                MessageBox.Show("시작일이 종료일보다 최신일 수 없습니다.");
                return;
            }

            TotalConut = 0;

            using (SqlConnection conn = new SqlConnection(Commons.CONNSTRING))
            {
                string strQuery = @"SELECT id, CurrTime, Temp, Humid
                                        From TblSmartHome
                                    WHERE DevId = @Devid
                                        AND CurrTime BETWEEN @StartDate AND @EndDate
                                    ORDER BY id ASC";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);

                    SqlParameter parmDevId = new SqlParameter("@Devid", selectedDivision.DivisionVal);
                    cmd.Parameters.Add(parmDevId);
                    SqlParameter parmStartDate = new SqlParameter("@StartDate", StartDate);
                    cmd.Parameters.Add(parmStartDate);
                    SqlParameter parmEndDate = new SqlParameter("@EndDate", EndDate);
                    cmd.Parameters.Add(parmEndDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    var i = 0;
                    while (reader.Read())
                    {
                        var temp = reader["Temp"];

                        i++;
                    }

                    TotalConut = i;
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Error {ex.Message}");
                    return;
                }
            }
        }
    }
}
