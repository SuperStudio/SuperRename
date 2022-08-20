using ChaoControls.Style;
using SuperRename.Core.pojo;
using SuperRename.Core.Utils;
using SuperRename.VieModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SuperStudio.CommonUtils;

namespace SuperRename
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        VieModel_Main vieModel;
        public MainWindow()
        {
            InitializeComponent();
            vieModel = new VieModel_Main();
            this.DataContext = vieModel;
            //ScanDir(@"D:\SuperStudio\SuperRename\SuperRename\Test");
        }

        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            string[] dragdropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (dragdropFiles != null && dragdropFiles.Length > 0)
            {
                for (int i = 0; i < dragdropFiles.Length; i++)
                {
                    vieModel.DataList.Add(new FileData(true, dragdropFiles[i], dragdropFiles[i]));
                }
                vieModel.DataListCountChange();
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetEnable();
        }

        private void SetEnable(bool value = true)
        {
            for (int i = 0; i < vieModel?.DataList.Count; i++)
            {
                vieModel.DataList[i].Enable = value;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetEnable(false);
        }

        private void ChangeExt(object sender, RoutedEventArgs e)
        {
            vieModel.ApplyChanges();
        }

        private void BeginChangeName(object sender, RoutedEventArgs e)
        {


            int breakCount = 0;
            List<FileData> success = new List<FileData>();

            if (vieModel?.DataList.Count > 0)
            {
                // 执行修改
                for (int i = 0; i < vieModel.DataList.Count; i++)
                {
                    FileData data = vieModel.DataList[i];
                    string source = data.Source;
                    string target = data.Target;
                    if (source.Equals(target))
                    {
                        data.StatusMessage = "跳过";
                        continue;
                    }
                    try
                    {
                        if (FileUtil.IsFile(vieModel.DataList[i].Source))
                        {
                            File.Move(vieModel.DataList[i].Source, vieModel.DataList[i].Target);
                            success.Add(data);
                        }
                        else
                        {
                            Directory.Move(vieModel.DataList[i].Source, vieModel.DataList[i].Target);
                            success.Add(data);
                        }


                    }
                    catch (Exception ex)
                    {
                        ChaoControls.Style.MessageCard.Show(ex.Message);
                        breakCount++;
                        if (breakCount >= 10) break;
                    }
                }
                ChaoControls.Style.MessageCard.Show($"修改文件名：{success.Count}/{vieModel.DataList.Count}", MessageCard.MessageCardType.Succes);
            }

            vieModel.CanRun = false;
        }



        private bool CheckNameProper()
        {
            List<int> wrongList = new List<int>();


            for (int i = 0; i < vieModel?.DataList.Count; i++)
            {
                FileData data = vieModel.DataList[i];
                if (data.Enable)
                {
                    string target = data.Target;
                    if (string.IsNullOrEmpty(target) || !FileUtil.IsProperPath(target))
                    {
                        wrongList.Add(i);
                    }
                }
                setDataGridColor(i, Brushes.White);
            }

            for (int i = 0; i < wrongList.Count; i++)
            {
                setDataGridColor(wrongList[i], Brushes.Red);
            }


            return wrongList.Count <= 0;
        }

        private void setDataGridColor(int rowIndex, SolidColorBrush brush)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator
                                               .ContainerFromIndex(rowIndex);
            if (row == null) return;
            var cell = dataGrid.Columns[3];
            var cp = (ContentPresenter)cell.GetCellContent(row);
            TextBox textBox = (TextBox)cp.ContentTemplate.FindName("tb", cp);
            textBox.Foreground = brush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vieModel.DataList.Clear();
            vieModel.DataListCountChange();
        }

        private void TogglePanel(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            SetTogglePanelStatus((bool)btn.IsChecked);
        }

        private void SetTogglePanelStatus(bool status)
        {
            if (status)
            {
                ToolsPanel.Height = 120;
                if (rotateTransform != null)
                    rotateTransform.Angle = 180;

            }
            else
            {
                ToolsPanel.Height = 30;
                if (rotateTransform != null)
                    rotateTransform.Angle = 0;
            }
            if (vieModel != null)
                vieModel.ExpandTogglePanel = status;
        }

        private void ToolsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetTogglePanelStatus(true);
        }

        private void ScanDir(object sender, RoutedEventArgs e)
        {
            string dir = FileUtil.SelectPath(this, "选择需要扫描的文件夹");
            ScanDir(dir);
        }

        private void ScanDir(string dir)
        {
            if (!Directory.Exists(dir)) return;
            IEnumerable<string> lists = FileUtil.GetFileList("*.*", dir);
            foreach (string filePath in lists)
            {
                vieModel.DataList.Add(new FileData(true, filePath, filePath));
            }
            vieModel.DataListCountChange();
        }

        private void PathButton_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            SetEnable(true);
        }

        private void UnSelectAll(object sender, RoutedEventArgs e)
        {
            SetEnable(false);
        }

        private void SelectOppsite(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < vieModel.DataList.Count; i++)
            {
                vieModel.DataList[i].Enable = !vieModel.DataList[i].Enable;
            }
        }

        private void RemoveDir(object sender, RoutedEventArgs e)
        {
            for (int i = vieModel.DataList.Count - 1; i >= 0; i--)
            {
                if (!FileUtil.IsFile(vieModel.DataList[i].Source)) vieModel.DataList.Remove(vieModel.DataList[i]);
            }
        }

        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            for (int i = vieModel.DataList.Count - 1; i >= 0; i--)
            {
                if (FileUtil.IsFile(vieModel.DataList[i].Source)) vieModel.DataList.Remove(vieModel.DataList[i]);
            }
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in vieModel.DataList)
            {
                builder.Append(item.Source + " | " + item.Target + Environment.NewLine);
            }
            System.Windows.Forms.Clipboard.SetDataObject(builder.ToString(), false, 5, 200);
        }

        private void Export(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveSelect(object sender, RoutedEventArgs e)
        {
            for (int i = vieModel.DataList.Count - 1; i >= 0; i--)
            {
                if (vieModel.DataList[i].Enable) vieModel.DataList.Remove(vieModel.DataList[i]);
            }
        }

        private void Preview(object sender, RoutedEventArgs e)
        {
            if (!CheckNameProper())
            {
                ChaoControls.Style.MessageCard.Show("文件名非法，请修改后再执行");
                return;
            }
            vieModel.CanRun = true;
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            FileData data = (FileData)dataGrid.SelectedItem;
            Console.WriteLine(data.Source);
        }

        private void OpenDir(object sender, RoutedEventArgs e)
        {

        }

        private void AddExt(object sender, RoutedEventArgs e)
        {

        }
    }
}
