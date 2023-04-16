﻿using CourseWorkRebuild;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CourseWorkRebuild2
{
    public partial class MainForm : Form
    {
        private List<String> values = new List<String>();
        private List<NewProjectForm> openForms = new List<NewProjectForm>();
        private DataTable dataTable = new DataTable();
        private System.Data.SQLite.SQLiteConnection sqlConnection;
        private Repository repository;
        private String oldObjectPicturePath = "";
        private String oldElevatorTablePath = "";
        private Decomposition decomposition = new Decomposition();
        private List<ListBox> lists = new List<ListBox>();
        private ChartDiagramService chartDiagramService = new ChartDiagramService();
        private CheckValuesForm checkValuesForm;
        Calculations calculations = new Calculations();
        private int activeForm = 0;
        private int epochCount = 0;
        public MainForm()
        {
            InitializeComponent();
            disableStartButtons();
            checkValuesForm = new CheckValuesForm();
            lists.Add(checkValuesForm.GetListBox1());
            lists.Add(checkValuesForm.GetListBox2());
            lists.Add(checkValuesForm.GetListBox3());
            lists.Add(checkValuesForm.GetListBox4());
            lists.Add(checkValuesForm.GetListBox5());
            lists.Add(checkValuesForm.GetListBox6());
        }

        private void initEpochCount()
        {
            if (epochCount == 0)
            {
                epochCount = Convert.ToInt32(elevatorTable.Rows[(elevatorTable.Rows.Count - 2)].Cells[0].Value)+1;
            }
        }

        private void reDrawMainForm()
        {
            reDrawElevatorTable();
            initEpochCount();
            reDrawValues();
            reDrawObjectPicture();
            if (values[6] != null & values[6] != "")
            {
                this.Text = values[6];
                this.closeButton.Enabled = true;
                this.saveAsButton.Enabled = true;
                this.closeAllButton.Enabled = true;
                this.saveCopyButton.Enabled = true;
                this.saveButton.Enabled = true;
            }
            else { this.Text = ""; }
        }
        private void firstLevelDecomposition_Enter(object sender, EventArgs e)
        {
            //Если достаточно данных для расчетов
            if (values.Count > 0 && values[0] != "" & values[2] != "" & values[3] != "")
            {
                Int32 epochCount = elevatorTable.RowCount;
                //Тут получаем значения из расчетов
                lists = decomposition.FirstLevel(elevatorTable, dataTable, values, lists);
                //А тут заполняем табличку
                firstLevelOfDecompositionTable = decomposition.DrawTable(firstLevelOfDecompositionTable, lists, epochCount, elevatorTable);
            }
        }

        private void reDrawObjectPicture()
        {
            //Если есть путь к картинке
            if (values[1] != null & values[1] != "")
            {
                objectPicture.Load(values[1]);
                objectPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                this.closeButton.Enabled = true;
                this.saveAsButton.Enabled = true;
                this.closeAllButton.Enabled = true;
                this.saveCopyButton.Enabled = true;
                this.saveButton.Enabled = true;
            }
            else
            {
                objectPicture.Image = null;
            }
        }

        private void reDrawValues()
        {
            //Если есть значение Т
            if (values[2] != null & values[2] != "")
            {
                valueOfTLabel.Text = "T: " + values[2];
                enableButtonsForValues();
            }
            else
            {
                valueOfTLabel.Text = "";
            }
            //Если есть значение А (для сглаживания)
            if (values[3] != null & values[3] != "")
            {
                valueOfALabel.Text = "A: " + values[3];
                enableButtonsForValues();
            }
            else
            {
                valueOfALabel.Text = "";
            }
            //Если известно количество марок
            if (values[4] != null & values[4] != "")
            {
                markCount.Text = "Количество марок: " + values[4];
            }
            else markCount.Text = "";
            //Если известно количество объектов
            if (values[5] != null & values[5] != "")
            {
                buildingCountValue.Text = "Количество объектов: " + values[5];

            }
            else buildingCountValue.Text = "";
        }

        private void reDrawElevatorTable()
        {
            //Если есть путь к БД
            if (values[0] != null & values[0] != "")
            {
                epochCountBox.Items.Clear();
                repository = new Repository(values[0]);
                sqlConnection = repository.GetDbConnection();
                elevatorTable = repository.ShowTable(dataTable, elevatorTable);
                for (int row = 0; row < elevatorTable.Rows.Count - 1; row++)
                {
                    epochCountBox.Items.Add(elevatorTable.Rows[row].Cells[0].Value);
                }
                epochCountBox.SelectedIndex = 0;
                enableButtonsForTable();
            }
            else
            {
                elevatorTable.Rows.Clear();
                elevatorTable.Columns.Clear();
                disableButtonsForTable();
            }
        }

        private void addNewRow_Click(object sender, EventArgs e)
        {
            int newRow = elevatorTable.RowCount - 1;
            elevatorTable.Rows[newRow].Cells[0].Value = epochCount;
            repository.AddNewRow(epochCount);
            calculations.AddNewValuesInRow(elevatorTable, repository, epochCount);
            epochCount++;
            reDrawElevatorTable();
        }

        private void elevatorTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int currentColumn = e.ColumnIndex;
            int currentRow = e.RowIndex;
            Double newValue = Convert.ToDouble(elevatorTable.Rows[currentRow].Cells[currentColumn].Value);
            elevatorTable = repository.UpdateValue(elevatorTable, currentColumn, currentRow, newValue);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (activeForm > 0)
            {
                NewProjectForm newProjectForm = new NewProjectForm();
                openForms.Add(newProjectForm);
                newProjectForm.Show();
                activeForm++;
            }
            else
            {
                OpenProject openProject = new OpenProject();
                values = openProject.Open();
                reDrawMainForm();
                if (!(values[0] == "") & !(values[1] == "") & !(values[7] == ""))
                {
                    activeForm++;
                }
            }
        }

        private void openRarButton_Click(object sender, EventArgs e)
        {
            if (activeForm > 0)
            {
                NewProjectForm newProjectForm = new NewProjectForm();
                openForms.Add(newProjectForm);
                newProjectForm.SetParameter(1);
                newProjectForm.Show();
                activeForm++;
            }
            else
            {
                OpenProject openProject = new OpenProject();
                values = openProject.UnzipRar();
                reDrawMainForm();
                activeForm++;
            }

        }

        private void closeAllButton_Click(object sender, EventArgs e)
        {
            //Очищаем пути ко всем Файлам и все известные значения
            closeButton_Click(sender, e);

            //Дропаем все открытые формы
            foreach (NewProjectForm form in openForms)
            {
                form.Close();
            }
            openForms.Clear();
            reDrawMainForm();
        }
        private void closeButton_Click(object sender, EventArgs e)
        {
            for (int value = 0; value < values.Count; value++)
            {
                values[value] = "";
            }
            decomposition.ClearTable(firstLevelOfDecompositionTable);
            activeForm = 0;
            reDrawMainForm();
        }

        private void deleteEpochButton_Click(object sender, EventArgs e)
        {  
            repository.DeleteRow(epochCountBox.Text);
            elevatorTable.Rows.RemoveAt(epochCountBox.SelectedIndex - 1);
            reDrawElevatorTable();
        }

        private void deleteLastEpoch_Click(object sender, EventArgs e)
        {
            string lastRowIndex = elevatorTable.Rows[(elevatorTable.Rows.Count - 2)].Cells[0].Value.ToString();
            repository.DeleteRow(lastRowIndex);
            reDrawElevatorTable();
        }
        private void deleteSelectedRowsButton_Click(object sender, EventArgs e)
        {
            if (elevatorTable.SelectedRows.Count > 0)
            {
                for (int i = 0; i < elevatorTable.SelectedRows.Count; i++)
                {
                    repository.DeleteRow(elevatorTable.Rows[elevatorTable.SelectedRows[i].Index].Cells[0].Value.ToString());
                }
            }
            reDrawElevatorTable();
        }

        private void infoAboutSystem_Click(object sender, EventArgs e)
        {
            SystemInfo systemInfo = new SystemInfo();
            systemInfo.ShowDialog();
        }

        private void saveAsZipArchieve_Click(object sender, EventArgs e)
        {
            saveAsNewFolder_Click(sender, e);
        }

        private void saveAsRarArchive_Click(object sender, EventArgs e)
        {
            saveAsNewFolder_Click(sender, e);
        }

        private String getPathToSave()
        {
            String sourceDirectory = "";
            if (values[0] != "" & values[0] != null)
            {
                sourceDirectory = Path.GetDirectoryName(values[0]);
            }
            else if (values[1] != "" & values[1] != null)
            {
                sourceDirectory = Path.GetDirectoryName(values[1]);
            }
            else if (values[7] != "" & values[7] != null)
            {
                sourceDirectory = Path.GetDirectoryName(values[7]);
            }
            return sourceDirectory;
        }

        private void saveAsNewFolder_Click(object sender, EventArgs e)
        {
            try
            {
                String sourceDirectory = getPathToSave();
                if (sourceDirectory != "")
                {
                    sqlConnection.Close();
                    Save save = new Save();
                    save.SaveFilesToNewFolder(sourceDirectory);
                    sqlConnection = repository.GetDbConnection();
                    MessageBox.Show("Сохранение успешно!");
                }
                else
                {
                    MessageBox.Show("Не найдены файлы для сохранения");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сохранение не удалось, попробуйте еще раз");
            }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            String tempTableFile = values[0];
            String tempPngFile = values[1];
            values[0] = "";
            values[1] = "";
            reDrawMainForm();
            Save save = new Save();
            save.SaveTxtFile(values[7], Convert.ToDouble(values[2]), Convert.ToInt32(values[5]), Convert.ToInt32(values[4]));
            if (oldObjectPicturePath != "" & oldObjectPicturePath != values[1]) { save.SaveNewFile(values[1], oldObjectPicturePath, "*.png"); }
            if (oldElevatorTablePath != "" & oldElevatorTablePath != tempTableFile) { save.SaveNewFile(tempTableFile, oldElevatorTablePath, "*.sqlite"); }
            values[0] = tempTableFile;
            values[1] = tempPngFile;
            reDrawMainForm();
        }

        private void newBlocksCount_Enter(object sender, EventArgs e)
        {
            values[5] = this.newBlocksCount.Text;
            reDrawValues();
        }

        private void newAValue_Enter(object sender, EventArgs e)
        {
            values[3] = this.newAValue.Text;
            reDrawValues();
        }

        private void newTValue_Enter(object sender, EventArgs e)
        {
            values[2] = this.newTValue.Text;
            reDrawValues();
        }

        private void newTValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) | (e.KeyChar.Equals(',')) | e.KeyChar.Equals('\b')) return;
            else
                e.Handled = true;
        }

        private void newAValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) | (e.KeyChar.Equals(',')) | e.KeyChar.Equals('\b')) return;
            else
                e.Handled = true;
        }

        private void newBlocksCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) | (e.KeyChar.Equals(',')) | e.KeyChar.Equals('\b')) return;
            else
                e.Handled = true;
        }

        private String getNewFilePath(String title, String filter)
        {
            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.Multiselect = false;
            String newFilePath = "";
            chooseFile.Title = title;
            chooseFile.Filter = filter;
            if (chooseFile.ShowDialog() == DialogResult.OK)
            {
                newFilePath = Path.GetFullPath(chooseFile.FileName);

            }
            return newFilePath;
        }

        private void changeElevatorTablePath_Click(object sender, EventArgs e)
        {
            String title = "Выберите таблицу высот";
            String filter = "SQLite files (*.sqlite)|*.sqlite";
            oldElevatorTablePath = values[0];
            values[0] = getNewFilePath(title, filter);
            if (values[0] == "")
            {
                values[0] = oldElevatorTablePath;
            }
            reDrawElevatorTable();
        }

        private void changeObjectPicture_Click(object sender, EventArgs e)
        {
            String title = "Выберите схему объекта";
            String filter = "PNG files (*.png)|*.png";
            oldObjectPicturePath = values[1];
            values[1] = getNewFilePath(title, filter);
            if (values[1] == "")
            {
                values[1] = oldObjectPicturePath;
            }
            reDrawObjectPicture();
        }

        private void chartButton_Click(object sender, EventArgs e)
        {
            ChartForm chartForm = new ChartForm(elevatorTable, dataTable, values);
            chartForm.Show();
        }

        private void checkValues_Click(object sender, EventArgs e)
        {
            checkValuesForm.ShowDialog();
        }

        private void enableButtonsForTable()
        {
            this.AddNewEpochButton.Enabled = true;
            this.deleteEpochButton.Enabled = true;
            this.DeleteLastEpoch.Enabled = true;
            this.redactorMenu.Enabled = true;
            this.addEpochButton.Enabled = true;
            this.chooseEpochToDelete.Enabled = true;
            this.closeButton.Enabled = true;
            this.saveAsButton.Enabled = true;
            this.closeAllButton.Enabled = true;
            this.saveCopyButton.Enabled = true;
            this.saveButton.Enabled = true;
            this.deleteSelectedRowsButton.Enabled = true;
        }

        private void disableButtonsForTable()
        {
            this.AddNewEpochButton.Enabled = false;
            this.DeleteLastEpoch.Enabled = false;
            this.redactorMenu.Enabled = false;
            this.addEpochButton.Enabled = false;
            this.chooseEpochToDelete.Enabled = false;
            this.deleteEpochButton.Enabled = false;
            this.deleteSelectedRowsButton.Enabled = false;
        }

        private void disableStartButtons()
        {
            disableButtonsForTable();
            this.closeAllButton.Enabled = false;
            this.closeButton.Enabled = false;
            this.saveAsButton.Enabled = false;
            this.saveButton.Enabled = false;
            this.saveCopyButton.Enabled = false;
        }

        private void enableButtonsForValues()
        {
            this.closeButton.Enabled = true;
            this.saveAsButton.Enabled = true;
            this.closeAllButton.Enabled = true;
            this.saveCopyButton.Enabled = true;
            this.saveButton.Enabled = true;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
