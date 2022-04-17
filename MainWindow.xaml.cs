using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ExcelDataReader;
using System.Net;
using System.IO;
using System.Data;
using System.Threading.Tasks;

namespace lan2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int pageIndex = 1;
        public bool f1 = false;
        private const int DelayMs = 1000;
        public const int numberOfRecPerPage = 15;
        public string np = "";
        private enum PagingMode { First = 1, Next = 2, Previous = 3, Last = 4, PageCountChange = 5 };
        List<List<string>> before = new List<List<string>>();
        List<List<string>> after = new List<List<string>>();
        List<List<string>> tema = new List<List<string>>();
        DataTable myTable = new DataTable();
        DataTable newTable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
            t2.Visibility = Visibility.Hidden;
            t3.Visibility = Visibility.Hidden;
            Base.Visibility = Visibility.Hidden;
            bm1.Visibility = Visibility.Hidden;
            bm2.Visibility = Visibility.Hidden;
            tx.Visibility = Visibility.Hidden;
            string path = Directory.GetCurrentDirectory();
            string[] pathTo = path.Split('\\');
            for (int i = 0; i < pathTo.Length; i++)
            {
                np += pathTo[i] + "\\";
                if (pathTo[i] == "lan2")
                {
                    break;
                }
            }
            np += "Base.xlsx";
            if (!File.Exists(np))
            {
                bm1.Visibility = Visibility.Visible;
                tx.Visibility = Visibility.Visible;
                bm2.Visibility = Visibility.Visible;
                if (!f1)
                {
                    this.Loaded += MainWindow_Loaded;
                }
            }
            else
            {
                t2.Visibility = Visibility.Visible;
                t3.Visibility = Visibility.Visible;
                Base.Visibility = Visibility.Visible;
                this.Loaded += MainWindow_Loaded1;
            }
        }

        public DataTable File5()
        {
            WebClient wc = new WebClient();
            using (FileStream fs = File.Create(np)) { }
            wc.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", np);
            return ExcelFileReader(np);
        }

        public DataTable ExcelFileReader(string path)
        {
            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            var tables = result.Tables[0];
            tables.Columns.Add("Идентификатор", typeof(String));
            tables.Columns.Add("Наименование угрозы", typeof(String));
            for (int i = 2; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][0].ToString().Length == 1) tables.Rows[i][10] = "00" + tables.Rows[i][0].ToString();
                else if (tables.Rows[i][0].ToString().Length == 2) tables.Rows[i][10] = "0" + tables.Rows[i][0].ToString();
                else tables.Rows[i][10] = tables.Rows[i][0].ToString();

            }
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                tables.Rows[i][11] = tables.Rows[i][1];
            }
            tables.Columns.Remove("Column8");
            tables.Columns.Remove("Column9");
            tables.Columns["Column0"].ColumnName = tables.Rows[1][0].ToString();
            tables.Columns["Column1"].ColumnName = tables.Rows[1][1].ToString();
            tables.Columns["Column2"].ColumnName = tables.Rows[1][2].ToString();
            tables.Columns["Column3"].ColumnName = tables.Rows[1][3].ToString();
            tables.Columns["Column4"].ColumnName = tables.Rows[1][4].ToString();
            tables.Columns["Column5"].ColumnName = tables.Rows[1][5].ToString();
            tables.Columns["Column6"].ColumnName = tables.Rows[1][6].ToString();
            tables.Columns["Column7"].ColumnName = tables.Rows[1][7].ToString();
            for (int i = 2; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][5].ToString() == "1") tables.Rows[i][5] = "да";
                else tables.Rows[i][5] = "нет";
                if (tables.Rows[i][6].ToString() == "1") tables.Rows[i][6] = "да";
                else tables.Rows[i][6] = "нет";
                if (tables.Rows[i][7].ToString() == "1") tables.Rows[i][7] = "да";
                else tables.Rows[i][7] = "нет";
            }
            stream.Close();
            return tables;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            myTable = File5();
            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
            int count = myTable.Rows.Cast<System.Data.DataRow>().Take(numberOfRecPerPage).Count();
            lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
        }
        private void MainWindow_Loaded1(object sender, RoutedEventArgs e)
        {
            myTable = ExcelFileReader(np);
            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
            int count = myTable.Rows.Cast<System.Data.DataRow>().Take(numberOfRecPerPage).Count();
            lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
        }

        public void Navigate(int mode)
        {
            int count;
            switch (mode)
            {
                case (int)PagingMode.Next:
                    btnPrev.IsEnabled = true;
                    btnFirst.IsEnabled = true;
                    if ((myTable.Rows.Count - 2) >= (pageIndex * numberOfRecPerPage))
                    {
                        if (myTable.Rows.Cast<System.Data.DataRow>().Skip(pageIndex *
                        numberOfRecPerPage + 2).Take(numberOfRecPerPage).Count() == 0)
                        {
                            Base.DataContext = null;
                            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip((pageIndex *
                            numberOfRecPerPage + 2) -
                            numberOfRecPerPage).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
                            count = (pageIndex * numberOfRecPerPage) + (myTable.Rows.Cast<System.Data.DataRow>().Skip(pageIndex * numberOfRecPerPage).Take(numberOfRecPerPage)).Count();
                        }
                        else
                        {
                            Base.DataContext = null;
                            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(pageIndex * numberOfRecPerPage + 2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
                            count = Math.Min((pageIndex * numberOfRecPerPage) + (myTable.Rows.Cast<System.Data.DataRow>().Skip(pageIndex * numberOfRecPerPage).Take(numberOfRecPerPage)).Count(), myTable.Rows.Count - 2);
                            pageIndex++;
                        }

                        lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
                    }

                    else
                    {
                        btnNext.IsEnabled = false;
                        btnLast.IsEnabled = false;
                    }

                    break;
                case (int)PagingMode.Previous:
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                    if (pageIndex > 1)
                    {
                        pageIndex -= 1;
                        Base.DataContext = null;
                        if (pageIndex == 1)
                        {
                            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
                            count = myTable.Rows.Cast<System.Data.DataRow>().Take(numberOfRecPerPage).Count();
                            lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
                        }
                        else
                        {
                            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip((pageIndex - 1) * numberOfRecPerPage + 2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
                            count = Math.Min(pageIndex * numberOfRecPerPage, myTable.Rows.Count - 2);
                            lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
                        }
                    }
                    else
                    {
                        btnPrev.IsEnabled = false;
                        btnFirst.IsEnabled = false;
                    }
                    break;

                case (int)PagingMode.First:
                    pageIndex = 2;
                    Navigate((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    pageIndex = ((myTable.Rows.Count - 2) / numberOfRecPerPage);
                    Navigate((int)PagingMode.Next);
                    break;

                case (int)PagingMode.PageCountChange:
                    pageIndex = 1;
                    Base.DataContext = null;
                    Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
                    count = (myTable.Rows.Cast<System.Data.DataRow>().Take(numberOfRecPerPage)).Count();
                    lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                    btnPrev.IsEnabled = true;
                    btnFirst.IsEnabled = true;
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            updateData();
        }

        public void btnFirst_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.First);
        }

        public void btnNext_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Next);

        }

        public void btnPrev_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Previous);

        }

        public void btnLast_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Last);
        }

        public void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Navigate((int)PagingMode.PageCountChange);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn col in Base.Columns)
            {
                switch (col.Header.ToString())
                {
                    case "Идентификатор":
                        col.Visibility = Visibility.Visible;
                        break;
                    case "Наименование угрозы":
                        col.Visibility = Visibility.Visible;
                        break;
                    default:
                        col.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn col in Base.Columns)
            {
                switch (col.Header.ToString())
                {
                    case "Идентификатор":
                        col.Visibility = Visibility.Collapsed;
                        break;
                    case "Наименование угрозы":
                        col.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        col.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void Base_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "Идентификатор УБИ":
                    e.Column.Visibility =
                    Visibility.Visible;
                    break;
                case "Наименование УБИ":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Описание":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Источник угрозы (характеристика и потенциал нарушителя)":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Объект воздействия":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Нарушение конфиденциальности":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Нарушение целостности":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Нарушение доступности":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Идентификатор":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "Наименование угрозы":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                default:
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void updateData()
        {
            string status, error, s = "";
            bool fg = false;
            for (int i = 0; i < 223; i++)
            {
                before.Add(new List<string>());
            }
            for (int i = 0; i < 223; i++)
            {
                after.Add(new List<string>());
            }
            for (int i = 0; i < 223; i++)
            {
                tema.Add(new List<string>());
            }
            DataTable tmpTable = new DataTable();
            tmpTable = myTable.Copy();
            int countNotes = 0;
            newTable = File5();
            if (newTable != null)
            {
                status = "Успешно";
            }
            else
            {
                status = "Ошибка";
                error = "Ошибка чтения файла";
                MessageBox.Show("Статус обновления: " + status + "\n" + "Причина ошибки: " + error);
            }
            for (int i = 2; i < newTable.Rows.Count; i++)
            {
                fg = false;
                for (int j = 0; j < 8; j++)
                {
                    if (newTable.Rows[i][j].ToString() != tmpTable.Rows[i][j].ToString())
                    {
                        before[Convert.ToInt32(newTable.Rows[i][0].ToString())].Add(tmpTable.Rows[i][j].ToString());
                        after[Convert.ToInt32(newTable.Rows[i][0].ToString())].Add(newTable.Rows[i][j].ToString());
                        tema[Convert.ToInt32(newTable.Rows[i][0].ToString())].Add(tmpTable.Rows[1][j].ToString());
                        fg = true;
                    }
                }
                if (fg) countNotes++;
            }
            if (countNotes == 0)
            {
                MessageBox.Show("Статус обновления: " + status + "\n" + "Количество обновлённых записей: " + countNotes);
                return;
            }
            else
            {
                for (int i = 0; i < 223; i++)
                {
                    if (before[i].Count() != 0)
                    {
                        s += "\nИдентификатор угрозы: " + i + "\nИзменения:\n";
                        for (int j = 0; j < before[i].Count(); j++)
                        {
                            s += tema[i][j].ToString() + ": " + before[i][j].ToString() + " -> " + after[i][j].ToString() + "\n";
                        }
                    }
                }
            }
            myTable = tmpTable.Copy();
            Base.DataContext = myTable.Rows.Cast<System.Data.DataRow>().Skip(2).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
            int count = myTable.Rows.Cast<System.Data.DataRow>().Take(numberOfRecPerPage).Count();
            lblpageInformation.Content = count + " of " + (myTable.Rows.Count - 2);
            MessageBox.Show("Статус обновления: " + status + "\n" + "Количество обновлённых записей: " + countNotes + "\n" + s);
        }
        private void Click1(object sender, RoutedEventArgs e)
        {
            f1 = true;
            this.Close();
        }
        private void Click2(object sender, RoutedEventArgs e)
        {
            t2.Visibility = Visibility.Visible;
            t3.Visibility = Visibility.Visible;
            Base.Visibility = Visibility.Visible;
            bm1.Visibility = Visibility.Hidden;
            bm2.Visibility = Visibility.Hidden;
            tx.Visibility = Visibility.Hidden;
        }
    }
}
