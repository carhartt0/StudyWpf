using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
        private PlotModel historyModel;     // 220613, 임경길 smartHomeModel -> historyModel 변경

        /*Divisions
                DivisionVal
                SelectedDivision
                StartDate
                InitStartDate
                EndDate
                InitEndDate
                TotalCount
                SearchIoTData()
                HistoryModel*/
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
        public PlotModel HistoryModel
        {
            get => historyModel; 
            set
            {
                historyModel = value;
                NotifyOfPropertyChange(() => HistoryModel);
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
                string strQuery = @"SELECT id, Devid, CurrTime, Temp, Humid
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

                    int i = 0;
                    //start of chart procee 220613 추가
                    PlotModel tmp = new PlotModel 
                    { 
                        Title = $"{SelectedDivision.DivisionVal} Histories" 
                    };
                    // 임시 플롯모델

                    var l = new Legend  // 범례, OxyPlot.Wpf > LegendsDemo 참조
                    {
                        LegendBorder = OxyColors.Black,
                        LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                        LegendPosition = LegendPosition.RightTop,
                        LegendPlacement = LegendPlacement.Inside
                    };
                    tmp.Legends.Add(l);

                    LineSeries seriesTemp = new LineSeries
                    {
                        Color = OxyColor.FromRgb(255, 100, 100)
                                                            ,
                        Title = "Temperature"
                                                            ,
                        MarkerSize = 3
                                                            ,
                        MarkerType = MarkerType.Circle
                    };
                    //온도값을 라인차트로 담을 객체
                    LineSeries seriesHumid = new LineSeries
                    {
                        Color = OxyColor.FromRgb(150, 150, 255)
                                                             ,
                        Title = "Humidity"
                                                             ,
                        MarkerType = MarkerType.Triangle
                    };
                    

                    while (reader.Read())
                    {
                        var model = new SmartHomeModel();       //지우는게 나음
                        model.Devid = reader["Devid"].ToString();
                        model.CurrTime = DateTime.Parse(reader["CurrTime"].ToString());
                        model.Temp = Convert.ToDouble(reader["Temp"]);
                        model.Humid = Convert.ToDouble(reader["Humid"]);

                        //var temp = reader["Temp"];
                        // Temp, Humid 차트데이터를 생성
                        seriesTemp.Points.Add(new DataPoint(i, model.Temp));
                        seriesHumid.Points.Add(new DataPoint(i, model.Humid));
                        //Convert.ToDouble(reader["Temp"]) == Double.Parse(reader["Temp"].ToString())

                        i++;
                    }

                    TotalConut = i;     // 검색한 데이터 총 개수

                    tmp.Series.Add(seriesTemp);
                    tmp.Series.Add(seriesHumid);

                    HistoryModel = tmp;
                    //end of chart procee 220613 추가
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
