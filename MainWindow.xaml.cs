using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExcelDataReader;
using System.Net;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Drawing;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        int pageIndex = 1;
        public const int numberOfRecPerPage = 15;
        private enum PagingMode { First = 1, Next = 2, Previous = 3, Last = 4, PageCountChange = 5 };
        DataTable myTable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public DataTable File5()
        {
            WebClient wc = new WebClient();
            string path = Directory.GetCurrentDirectory();
            string[] pathTo = path.Split('\\');
            string np = "";
            for (int i = 0; i < pathTo.Length; i++)
            {
                np += pathTo[i] + "\\";
                if (pathTo[i] == "WpfApp1")
                {
                    break;
                }
            }
            np += "Base.txt";
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
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][0].ToString().Length == 1) tables.Rows[i][10] = "00" + tables.Rows[i][0].ToString();
                else if (tables.Rows[i][0].ToString().Length == 2) tables.Rows[i][10] = "0" + tables.Rows[i][0].ToString();
                else tables.Rows[i][10] = tables.Rows[i][0].ToString();

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
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][5].ToString() == "1") tables.Rows[i][5] = "да";
                else tables.Rows[i][5] = "нет";
                if (tables.Rows[i][6].ToString() == "1") tables.Rows[i][6] = "да";
                else tables.Rows[i][6] = "нет";
                if (tables.Rows[i][7].ToString() == "1") tables.Rows[i][7] = "да";
                else tables.Rows[i][7] = "нет";
            }
            return tables;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            myTable = File5();
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
                            numberOfRecPerPage + 2) - numberOfRecPerPage).Take(numberOfRecPerPage).CopyToDataTable().AsDataView();
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
            this.Close();
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
                    case "Наименование УБИ":
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
                col.Visibility = Visibility.Visible;
            }
        }

        private void Base_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "Идентификатор УБИ":
                    e.Column.Visibility = Visibility.Visible;
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
                default:
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
