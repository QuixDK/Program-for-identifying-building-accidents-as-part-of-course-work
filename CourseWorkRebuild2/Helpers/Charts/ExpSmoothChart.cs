﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkRebuild2.Helpers
{
    public partial class ExpSmoothChart : Form
    {
        private ChartDiagramService chartDiagramService = new ChartDiagramService();
        private List<List<Double>> values;
        private List<Double> epochList = new List<Double>();
        private DataGridView elevatorTable = new DataGridView();
        public ExpSmoothChart(List<List<Double>> values, DataGridView elevatorTable)
        {
            InitializeComponent();
            this.values = values;
            this.elevatorTable = elevatorTable;
        }

        private void MChart_CheckedChanged(object sender, EventArgs e)
        {
            alphaChart.Checked = false;
            String serieName2 = "Длина вектора М (сглаженное)";
            if (expSmooth.Series.IndexOf(serieName2) != -1) chartDiagramService.RemoveLine(expSmooth, serieName2);
            else chartDiagramService.AddXYLine(serieName2, epochList, values[3], expSmooth, elevatorTable); expSmooth.ChartAreas[0].AxisY.Title = "Длина вектора М, м";

            String serieName = "Длина вектора М (реальное)";
            if (expSmooth.Series.IndexOf(serieName) != -1) chartDiagramService.RemoveLine(expSmooth, serieName);
            else chartDiagramService.AddXYLine(serieName, epochList, values[2], expSmooth, elevatorTable); 

            applySettings(sender, e);
        }

        private void alphaChart_CheckedChanged(object sender, EventArgs e)
        {
            MChart.Checked = false;
            String serieName2 = "Угол а (сглаженное)";
            if (expSmooth.Series.IndexOf(serieName2) != -1) chartDiagramService.RemoveLine(expSmooth, serieName2);
            else chartDiagramService.AddXYLine(serieName2, epochList, values[9], expSmooth, elevatorTable); expSmooth.ChartAreas[0].AxisY.Title = "Угол наклона А,°";

            String serieName = "Угол а (реальное)";
            if (expSmooth.Series.IndexOf(serieName) != -1) chartDiagramService.RemoveLine(expSmooth, serieName);
            else chartDiagramService.AddXYLine(serieName, epochList, values[8], expSmooth, elevatorTable);     

            applySettings(sender, e);
        }

        private void ExpSmoothChart_Load(object sender, EventArgs e)
        {
            setDefaultValuesInSettings();
            if (values.Count == 0)
            {
                return;
            }
            for (int i = 0; i < elevatorTable.Rows.Count-1; i++)
            {
                epochList.Add(Convert.ToDouble(elevatorTable.Rows[i].Cells[0].Value));
            }
            epochList.Add(epochList.Last()+1);

        }

        private void setDefaultValuesInSettings()
        {
            chartTypeComboBox.Items.Clear();
            chartTypeComboBox.Items.Add("Линия");
            chartTypeComboBox.SelectedItem = "Линия";
            chartTypeComboBox.Items.Add("Сплайн");
            chartTypeComboBox.Items.Add("Точечная");
            
            markerSizeComboBox.Items.Clear();
            markerSizeComboBox.Items.Add("По умолчанию");
            markerSizeComboBox.SelectedItem = "По умолчанию";
            markerSizeComboBox.Items.Add("6");
            markerSizeComboBox.Items.Add("8");
            markerSizeComboBox.Items.Add("9");
            markerSizeComboBox.Items.Add("10");
            markerSizeComboBox.Items.Add("11");
            markerSizeComboBox.Items.Add("12");
            markerSizeComboBox.Items.Add("14");
            markerSizeComboBox.Items.Add("16");
            markerSizeComboBox.Items.Add("18");
            markerSizeComboBox.Items.Add("20");

            markerTypeComboBox.Items.Clear();
            markerTypeComboBox.Items.Add("Круг");
            markerTypeComboBox.SelectedItem = "Круг";
            markerTypeComboBox.Items.Add("Квадрат");
            markerTypeComboBox.Items.Add("Треугольник");
        }

        private void chartTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartUserSettings chartUserSettings = new ChartUserSettings();
            chartUserSettings.SetChartType(expSmooth, chartTypeComboBox);
        }

        private void markerSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartUserSettings chartUserSettings = new ChartUserSettings();
            chartUserSettings.SetChartMarkerSize(expSmooth, markerSizeComboBox);
        }

        private void markerTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartUserSettings chartUserSettings = new ChartUserSettings();
            chartUserSettings.SetChartMarkerType(expSmooth, markerTypeComboBox);
        }

        private void labelsOnMarksCheckBox_Click(object sender, EventArgs e)
        {
            ChartUserSettings chartUserSettings = new ChartUserSettings();
            chartUserSettings.SetChartLabelsOnMarks(expSmooth, labelsOnMarksCheckBox);
        }
        private void applySettings(object sender, EventArgs e)
        {
            chartTypeComboBox_SelectedIndexChanged(sender, e);
            markerSizeComboBox_SelectedIndexChanged(sender, e);
            markerTypeComboBox_SelectedIndexChanged(sender, e);
            labelsOnMarksCheckBox_Click(sender, e);
        }

    }
}
